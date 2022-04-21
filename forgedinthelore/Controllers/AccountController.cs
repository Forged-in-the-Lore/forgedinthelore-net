using AutoMapper;
using forgedinthelore_net.DTOs;
using forgedinthelore_net.Entities;
using forgedinthelore_net.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace forgedinthelore_net.Controllers;

public class AccountController : BaseApiController
{
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly RoleManager<AppRole> _roleManager;

    public AccountController(ITokenService tokenService, IMapper mapper, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<AppRole> roleManager)
    {
        _tokenService = tokenService;
        _mapper = mapper;
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
    }
    
    
    [HttpPost("register")]
    //Use Data transfer object instead of string input. Allows for validation and can handle body or url input
    public async Task<ActionResult> Register(RegisterDto registerDto)
    {
        if (await UserExists(registerDto.UserName)) return BadRequest("Username is taken");
        if (!await _roleManager.RoleExistsAsync(registerDto.Role)) return BadRequest("Role not found");
        
        var user = new AppUser
        {
            UserName = registerDto.UserName.ToLower()
        };
        

        var result = await _userManager.CreateAsync(user, registerDto.Password);
        if (!result.Succeeded) return BadRequest("Failed to register user");

        var roleResult = await _userManager.AddToRoleAsync(user, registerDto.Role);

        if (!roleResult.Succeeded) return BadRequest(result.Errors);
        return new OkResult();
    }
    
    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var user = await _userManager.Users
            .SingleOrDefaultAsync(x => x.UserName == loginDto.UserName.ToLower());
        if (user == null) return Unauthorized();

        var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
        if (!result.Succeeded) return Unauthorized();
        
        return new UserDto
        {
            UserName = user.UserName,
            Token = await _tokenService.CreateToken(user),
        };
    }

    private async Task<bool> UserExists(string username)
    {
        return await _userManager.Users.AnyAsync(x => x.UserName == username.ToLower());
    }
}