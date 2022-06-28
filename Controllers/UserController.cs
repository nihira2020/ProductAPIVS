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

namespace ProductAPIVS.Controllers;


[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly Learn_DBContext _DBContext;
    private readonly JwtSettings jwtSettings;
    public UserController(Learn_DBContext learn_DBContext, IOptions<JwtSettings> options)
    {
        this._DBContext = learn_DBContext;
        this.jwtSettings = options.Value;
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
    new Claim[] { new Claim(ClaimTypes.Name, user.Userid) }
),
            Expires = DateTime.Now.AddSeconds(20),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenkey), SecurityAlgorithms.HmacSha256)
        };
        var token = tokenhandler.CreateToken(tokendesc);
        string finaltoken = tokenhandler.WriteToken(token);

        return Ok(finaltoken);
    }
}