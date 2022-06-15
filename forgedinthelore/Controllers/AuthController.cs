using forgedinthelore_net.DTOs;
using forgedinthelore_net.Services;
using Microsoft.AspNetCore.Mvc;

namespace forgedinthelore_net.Controllers;

public class AuthController : BaseApiController
{
    private readonly ITokenValidatorService _tokenValidatorService;

    public AuthController(ITokenValidatorService tokenValidatorService)
    {
        _tokenValidatorService = tokenValidatorService;
    }

    [HttpPost]
    //Use Data transfer object instead of string input. Allows for validation and can handle body or url input
    public ActionResult<int> Verify([FromBody] TokenDto token)
    {
        var userResult = _tokenValidatorService.ValidateToken(token.Token);
        if (userResult == null) return Unauthorized("Token could not be verified");
        return userResult;
    }
}