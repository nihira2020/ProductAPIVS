using Microsoft.AspNetCore.Mvc;
using ProductAPIVS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ProductAPIVS.Container;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;
using ProductAPIVS.Handler;

namespace ProductAPIVS.Controllers;


[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly Learn_DBContext _DBContext;
    private readonly JwtSettings jwtSettings;
    private readonly IRefereshTokenGenerator refereshTokenGenerator;
    public UserController(Learn_DBContext learn_DBContext, IOptions<JwtSettings> options, IRefereshTokenGenerator referesh)
    {
        this._DBContext = learn_DBContext;
        this.jwtSettings = options.Value;
        this.refereshTokenGenerator = referesh;
    }

    [NonAction]
    public async Task<TokenResponse> TokenAuthenticate(string user, Claim[] claims)
    {
        var token = new JwtSecurityToken(
  claims: claims,
  expires: DateTime.Now.AddSeconds(20),
  signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.securitykey)), SecurityAlgorithms.HmacSha256)
        );
        var jwttoken = new JwtSecurityTokenHandler().WriteToken(token);


        return new TokenResponse() { jwttoken = jwttoken, refreshtoken = await refereshTokenGenerator.GenerateToken(user) };

    }


    [HttpPost("Authenticate")]
    public async Task<IActionResult> Authenticate([FromBody] UserCred userCred)
    {
        var user = await this._DBContext.TblUsers.FirstOrDefaultAsync(item => item.Userid == userCred.username && item.Password == userCred.password);
        if (user == null)
            return Unauthorized();
        /// Generate Token
        var tokenhandler = new JwtSecurityTokenHandler();
        var tokenkey = Encoding.UTF8.GetBytes(this.jwtSettings.securitykey);
        var tokendesc = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
    new Claim[] { new Claim(ClaimTypes.Name, user.Userid), new Claim(ClaimTypes.Role, user.Role) }
),
            Expires = DateTime.Now.AddMinutes(20),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenkey), SecurityAlgorithms.HmacSha256)
        };
        var token = tokenhandler.CreateToken(tokendesc);
        string finaltoken = tokenhandler.WriteToken(token);

        var response = new TokenResponse() { jwttoken = finaltoken, refreshtoken = await refereshTokenGenerator.GenerateToken(userCred.username) };

        return Ok(response);
    }

    [HttpPost("RefToken")]
    public async Task<IActionResult> RefToken([FromBody] TokenResponse tokenResponse)
    {

        /// Generate Token
        var tokenhandler = new JwtSecurityTokenHandler();
        var tokenkey = Encoding.UTF8.GetBytes(this.jwtSettings.securitykey);
        SecurityToken securityToken;
        var principal = tokenhandler.ValidateToken(tokenResponse.jwttoken, new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(tokenkey),
            ValidateIssuer = false,
            ValidateAudience = false,

        }, out securityToken);

        var token = securityToken as JwtSecurityToken;
        if (token != null && !token.Header.Alg.Equals(SecurityAlgorithms.HmacSha256))
        {
            return Unauthorized();
        }
        var username = principal.Identity?.Name;

        var user = await this._DBContext.TblRefreshtokens.FirstOrDefaultAsync(item => item.UserId == username && item.RefreshToken == tokenResponse.refreshtoken);
        if (user == null)
            return Unauthorized();

        var response = TokenAuthenticate(username, principal.Claims.ToArray()).Result;

        return Ok(response);
    }
}