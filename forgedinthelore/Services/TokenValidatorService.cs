using System.IdentityModel.Tokens.Jwt;
using forgedinthelore_net.Helpers;
using Microsoft.IdentityModel.Tokens;

namespace forgedinthelore_net.Services;

public class TokenValidatorService : ITokenValidatorService
{
    private readonly IConfiguration _config;

    public TokenValidatorService(IConfiguration config)
    {
        _config = config;
    }

    public int? ValidateToken(string token)
    {
        if (token == null)
            return null;

        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            tokenHandler.ValidateToken(token, TokenValidationParameterOptions.GetParameters(_config),
                out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken) validatedToken;
            var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "nameid").Value);

            // return user id from JWT token if validation successful
            return userId;
        }
        catch
        {
            // return null if validation fails
            return null;
        }
    }
}