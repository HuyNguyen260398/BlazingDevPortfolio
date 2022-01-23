using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Shared.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Server.Controllers;
[Route("api/[controller]")]
[ApiController]
public class SignInController : ControllerBase
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IConfiguration _config;

    public SignInController(
        SignInManager<IdentityUser> signInManager,
        UserManager<IdentityUser> userManager,
        IConfiguration config)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _config = config;
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> SignIn([FromBody] User user)
    {
        string username = user.EmailAddress;
        string password = user.Password;

        Microsoft.AspNetCore.Identity.SignInResult signInResult = await _signInManager.PasswordSignInAsync(username, password, false, false);

        if (signInResult.Succeeded)
        {
            IdentityUser identityUser = await _userManager.FindByNameAsync(username);
            string jsonWebTokenAsString = await GenerateJsonWebToken(identityUser);
            return Ok(jsonWebTokenAsString);
        }
        else
            return Unauthorized(user);
    }

    [NonAction]
    [ApiExplorerSettings(IgnoreApi = true)]
    private async Task<string> GenerateJsonWebToken(IdentityUser identityUser)
    {
        SymmetricSecurityKey symmetricSecurityKey = new(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        SigningCredentials credentials = new(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        List<Claim> claims = new()
        {
            new Claim(JwtRegisteredClaimNames.Sub, identityUser.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, identityUser.Id)
        };

        IList<string> roleNames = await _userManager.GetRolesAsync(identityUser);
        claims.AddRange(roleNames.Select(roleName => new Claim(ClaimsIdentity.DefaultNameClaimType, roleName)));

        JwtSecurityToken jwtSecurityToken = new
        (
            _config["Jwt:Issuer"],
            _config["Jwt:Issuer"],
            claims,
            null,
            expires: DateTime.UtcNow.AddDays(28),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
    }
}
