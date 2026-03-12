using Fintrack.Contracts.DTOs.User;
using LojaDoImovel.Application.Services.Interfaces;
using LojaDoImovel.Infrastructure.Identity;
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
    /// Autentica usuário e retorna access e refresh tokens.
    /// </summary>
    /// <param name="loginDto">Email e senha.</param>
    /// <returns>200 com tokens ou 401.</returns>
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
    /// Registra novo usuário e adiciona ao papel 'userunapproved'.
    /// </summary>
    /// <param name="registerDto">Dados do usuário.</param>
    /// <returns>201, 409 ou 500.</returns>
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
    /// Gera novo access token a partir de refresh token válido.
    /// </summary>
    /// <param name="tokenDto">Access token expirado e refresh token.</param>
    /// <returns>200 com novos tokens ou 400.</returns>
    /// <exception cref="ArgumentNullException" />
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
}