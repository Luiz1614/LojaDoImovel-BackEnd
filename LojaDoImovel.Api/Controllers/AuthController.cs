using Fintrack.Contracts.DTOs.User;
using LojaDoImovel.Application.Services.Interfaces;
using LojaDoImovel.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LojaDoImovel.Api.Controllers;

[ApiController]
[Route("api/[Controller]")]
public class AuthController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _configuration;
    private readonly ILogger _logger;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<int>> _roleManager;

    public AuthController(ITokenService tokenService, IConfiguration configuration, ILogger logger, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<int>> roleManager)
    {
        _tokenService = tokenService;
        _configuration = configuration;
        _logger = logger;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [HttpPost]
    [Route("/login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email!);

        if(user is not null && await _userManager.CheckPasswordAsync(user, loginDto.Password!))
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            }
        }
    }
}
