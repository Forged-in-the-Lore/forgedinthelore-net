using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace forgedinthelore_net.Helpers;

public static class TokenValidationParameterOptions
{
    public static TokenValidationParameters GetParameters(IConfiguration config)
    {
        return new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetSection("TokenKey").Value)),
            ValidateIssuer = false, //Would validate the server 
            ValidateAudience = false //Would validate the Angular app
        };
    }
}