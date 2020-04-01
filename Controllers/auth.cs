using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
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

            var clientPermissionClaims = new ClientPermissionClaims();
            clientPermissionClaims.ClientPermissions.Add(new ClientPermission("1", new List<string>() { "WeatherReader", "WeatherAdmin" }));
            clientPermissionClaims.ClientPermissions.Add(new ClientPermission("2", new List<string>() { "WeatherAdmin" }));
            clientPermissionClaims.ClientPermissions.Add(new ClientPermission("3", new List<string>() { "WeatherReader" }));
            clientPermissionClaims.ClientPermissions.Add(new ClientPermission("4", new List<string>() { "WeatherReader", "WeatherAdmin" }));
            clientPermissionClaims.ClientPermissions.Add(new ClientPermission("9", new List<string>() { "asdf" }));
            string jsonString;
            jsonString = JsonSerializer.Serialize(clientPermissionClaims);

            var claims = new[]
            {
              new Claim(JwtRegisteredClaimNames.Sub, "ao3155"),
              new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
              new Claim(JwtRegisteredClaimNames.GivenName, "Aaron"),
              new Claim(JwtRegisteredClaimNames.FamilyName, "Olds"),
              new Claim(JwtRegisteredClaimNames.Email, "olds@asdf.com"),
              new Claim("clients", "1,2,3,4,9"),
              new Claim("clientpermisions", jsonString)

            //   new Claim("dmps.1.roles", "\"GRP_DOWNLOADS\",\"GRP_EMPINFO\",\"GRP_LIBRARY\",\"GRP_HUMANRESOURCES\",\"GRP_CORRECTIONS\",\"GRP_REPORTS\",\"GRP_TRANSMITTAL\",\"GRP_EMPMASTER\""),
            //   new Claim("dmps.2.roles", "\"GRP_W4I9\",\"GRP_EMPINFO\",\"GRP_LIBRARY\",\"GRP_HUMANRESOURCES\",\"GRP_CORRECTIONS\",\"GRP_REPORTS\",\"GRP_TRANSMITTAL\",\"GRP_EMPMASTER\""),
            //   new Claim("dmps.4.roles", "\"GRP_W4I9\",\"GRP_DOWNLOADS\",\"GRP_LIBRARY\",\"GRP_HUMANRESOURCES\",\"GRP_CORRECTIONS\",\"GRP_REPORTS\",\"GRP_TRANSMITTAL\",\"GRP_EMPMASTER\""),
            //   new Claim("dmps.4.roles", "\"GRP_W4I9\",\"GRP_DOWNLOADS\",\"GRP_EMPINFO\",\"GRP_HUMANRESOURCES\",\"GRP_CORRECTIONS\",\"GRP_REPORTS\",\"GRP_TRANSMITTAL\",\"GRP_EMPMASTER\""),
            //   new Claim("dmps.9.roles", "\"GRP_W4I9\",\"GRP_DOWNLOADS\",\"GRP_EMPINFO\",\"GRP_LIBRARY\",\"GRP_CORRECTIONS\",\"GRP_REPORTS\",\"GRP_TRANSMITTAL\",\"GRP_EMPMASTER\"")
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