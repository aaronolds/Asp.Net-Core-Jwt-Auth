using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace aspnetcoreauth.Controllers
{
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IConfiguration _config;

        public AuthController(ILogger<AuthController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        [HttpPost("auth/token")]
        public IActionResult Post()
        {

            var claims = new[]
            {
              new Claim(JwtRegisteredClaimNames.Sub, "ao3155"),
              new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
              new Claim(JwtRegisteredClaimNames.GivenName, "Aaron"),
              new Claim(JwtRegisteredClaimNames.FamilyName, "Olds"),
              new Claim(JwtRegisteredClaimNames.Email, "olds@asdf.com")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
              issuer: _config["Tokens:Issuer"],
              audience: _config["Tokens:Audience"],
              claims: claims,
              expires: DateTime.UtcNow.AddMinutes(60),
              signingCredentials: creds
              );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo
            });
        }
    }
}