using LojaDoImovel.Application.Services.Interfaces;
using LojaDoImovel.Contracts.DTOs.ApplicationUser;
using LojaDoImovel.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;

namespace LojaDoImovel.Api.Controllers;

[ApiController]
[Route("api/[Controller]")]
public class AuthController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthController(ITokenService tokenService, IConfiguration configuration, ILogger<AuthController> logger, UserManager<ApplicationUser> userManager)
    {
        _tokenService = tokenService;
        _configuration = configuration;
        _logger = logger;
        _userManager = userManager;
    }

    /// <summary>
    /// Authenticates a user based on the provided login credentials and issues a JWT access token and refresh token
    /// upon successful authentication.
    /// </summary>
    /// <remarks>Returns an HTTP 401 Unauthorized response if the user's credentials are invalid or if the
    /// user's status is pending approval. The access token and refresh token should be used for subsequent
    /// authenticated requests and token refresh operations.</remarks>
    /// <param name="loginDto">An object containing the user's email and password used for authentication. Cannot be null.</param>
    /// <returns>An HTTP 200 response containing the access token, refresh token, and token expiration if authentication is
    /// successful; otherwise, an HTTP 401 Unauthorized response.</returns>
    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        _logger.LogInformation("Login attempt for email: {Email}", loginDto.Email);

        var user = await _userManager.FindByEmailAsync(loginDto.Email!);

        if (user is not null && await _userManager.CheckPasswordAsync(user, loginDto.Password!))
        {
            if (user.Status is UserStatus.Pending)
            {
                _logger.LogWarning("Login denied for user {Email}: pending approval.", loginDto.Email);
                return Unauthorized("Aguardando aprovação de um administrador.");
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var token = _tokenService.GenerateAccessToken(authClaims, _configuration);

            var refreshToken = _tokenService.GenerateRefreshToken();

            _ = int.TryParse(_configuration["Jwt:RefreshTokenValidityInMinutes"], out int refreshTokenValidtInMinutes);

            user.RefreshToken = refreshToken;

            user.RefreshTokenExpireTime = DateTime.UtcNow.AddMinutes(refreshTokenValidtInMinutes);

            await _userManager.UpdateAsync(user);

            _logger.LogInformation("Login successful for user {Email}.", loginDto.Email);

            return Ok(new
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = refreshToken,
                Expiration = token.ValidTo
            });
        }

        _logger.LogWarning("Login failed for email: {Email}", loginDto.Email);
        return Unauthorized();
    }

    /// <summary>
    /// Handles user registration by creating a new user account with the provided registration details.
    /// </summary>
    /// <remarks>This action assigns the new user to the 'userunapproved' role upon successful registration.
    /// The endpoint is accessible via HTTP POST at 'register'.</remarks>
    /// <param name="registerDto">The registration information for the new user. Must include a valid email, username, password, and optionally a
    /// phone number. Cannot be null.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an IActionResult indicating the
    /// outcome of the registration: 201 (Created) if successful, 409 (Conflict) if the user already exists, or 500
    /// (Internal Server Error) if registration fails.</returns>
    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        _logger.LogInformation("Register attempt for email: {Email}", registerDto.Email);

        var userExists = await _userManager.FindByEmailAsync(registerDto.Email!);

        if (userExists is not null)
        {
            _logger.LogWarning("Register failed: user already exists for email: {Email}", registerDto.Email);
            return StatusCode((int)HttpStatusCode.Conflict, "Já existe um usuário cadastrado!");
        }

        ApplicationUser user = new()
        {
            Email = registerDto.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = registerDto.UserName,
            PhoneNumber = registerDto.PhoneNumber,
            Status = 0
        };

        var result = await _userManager.CreateAsync(user, registerDto.Password!);

        _ = await _userManager.AddToRoleAsync(user, "userunapproved");

        if (!result.Succeeded)
        {
            _logger.LogError("Register failed for email: {Email}. Errors: {Errors}", registerDto.Email, string.Join(", ", result.Errors.Select(e => e.Description)));
            return StatusCode((int)HttpStatusCode.InternalServerError, $"Erro ao criar usuário. \n{result}");
        }

        _logger.LogInformation("User registered successfully: {Email}", registerDto.Email);
        return StatusCode((int)HttpStatusCode.Created, "Usuário criado com sucesso!");
    }

    /// <summary>
    /// Handles a refresh token request by validating the provided access and refresh tokens, and issues new tokens if
    /// the request is valid.
    /// </summary>
    /// <remarks>The refresh token must match the user's current refresh token and must not be expired. This
    /// endpoint is typically used to obtain new tokens without requiring the user to re-authenticate.</remarks>
    /// <param name="tokenDto">An object containing the expired access token and the associated refresh token. Cannot be null.</param>
    /// <returns>An HTTP response containing new access and refresh tokens if the request is valid; otherwise, a Bad Request
    /// response indicating the failure reason.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="tokenDto"/> is null, or if its AccessToken or RefreshToken properties are null.</exception>
    [HttpPost]
    [Route("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] TokenDto tokenDto)
    {
        _logger.LogInformation("Refresh token attempt.");

        if (tokenDto is null)
        {
            _logger.LogWarning("Refresh token failed: request body is null.");
            return BadRequest("Requisição inválida.");
        }

        string? accessToken = tokenDto.AccessToken ?? throw new ArgumentNullException(nameof(tokenDto));
        string? refreshToken = tokenDto.RefreshToken ?? throw new ArgumentNullException(nameof(tokenDto));

        var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken, _configuration);

        if (principal is null)
        {
            _logger.LogWarning("Refresh token failed: invalid tokens.");
            return BadRequest("Refresh/Access token inválido.");
        }

        string username = principal.Identity.Name;

        var user = await _userManager.FindByNameAsync(username!);

        if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpireTime <= DateTime.UtcNow)
        {
            _logger.LogWarning("Refresh token failed: invalid or expired for user {UserName}.", username);
            return BadRequest("Refresh/Access token inválido.");
        }

        var newAccessToken = _tokenService.GenerateAccessToken(principal.Claims.ToList(), _configuration);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;

        await _userManager.UpdateAsync(user);

        _logger.LogInformation("Refresh token successful for user {UserName}.", username);

        return new ObjectResult(new
        {
            accessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
            refreshToken = newRefreshToken,
        });
    }

    /// <summary>
    /// Approves a user account by updating the user's status to approved.
    /// </summary>
    /// <remarks>This action requires the caller to have the 'admin' role. Only users with administrative
    /// privileges can approve user accounts.</remarks>
    /// <param name="username">The user name of the account to approve. Cannot be null or empty.</param>
    /// <returns>An IActionResult indicating the result of the operation. Returns Ok if the user was approved successfully;
    /// otherwise, returns BadRequest if the user is not found.</returns>
    [HttpPut("approve-user")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> ApproveUser(string username)
    {
        var user = await _userManager.FindByNameAsync(username);

        if (user is null)
        {
            return BadRequest("Usuário não encontrado.");
        }

        user.Status = UserStatus.Approved;

        _ = await _userManager.UpdateAsync(user);

        return Ok("Usuário aprovado com sucesso!");
    }
}