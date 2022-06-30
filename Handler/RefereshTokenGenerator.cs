using System.Security.Cryptography;
using ProductAPIVS.Models;
using Microsoft.EntityFrameworkCore;
namespace ProductAPIVS.Handler;
public class RefereshTokenGenerator : IRefereshTokenGenerator
{
    private readonly Learn_DBContext _DBContext;
    public RefereshTokenGenerator(Learn_DBContext learn_DBContext)
    {
        this._DBContext = learn_DBContext;
    }
    public async Task<string> GenerateToken(string username)
    {
        var randomnumber = new byte[32];
        using (var ramdomnumbergenerator = RandomNumberGenerator.Create())
        {
            ramdomnumbergenerator.GetBytes(randomnumber);
            string refreshtoken = Convert.ToBase64String(randomnumber);
            var token = await this._DBContext.TblRefreshtokens.FirstOrDefaultAsync(item => item.UserId == username);
            if (token != null)
            {
                token.RefreshToken = refreshtoken;
            }
            else
            {
                this._DBContext.TblRefreshtokens.Add(new TblRefreshtoken()
                {
                    UserId = username,
                    TokenId = new Random().Next().ToString(),
                    RefreshToken = refreshtoken,
                    IsActive = true
                });
            }
            await this._DBContext.SaveChangesAsync();

            return refreshtoken;
        }


    }
}