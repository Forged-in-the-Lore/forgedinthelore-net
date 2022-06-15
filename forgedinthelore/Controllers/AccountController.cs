using AutoMapper;
using forgedinthelore_net.DTOs;
using forgedinthelore_net.Entities;
using forgedinthelore_net.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace forgedinthelore_net.Controllers;

public class AccountController : BaseApiController
{
    private readonly ITokenCreatorService _tokenCreatorService;
    private readonly IMapper _mapper;
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly RoleManager<AppRole> _roleManager;

    public AccountController(ITokenCreatorService tokenCreatorService, IMapper mapper, UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager, RoleManager<AppRole> roleManager)
    {
        _tokenCreatorService = tokenCreatorService;
        _mapper = mapper;
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
    }


    [HttpPost("register")]
    //Use Data transfer object instead of string input. Allows for validation and can handle body or url input
    public async Task<ActionResult> Register(RegisterDto registerDto)
    {
        if (await UserExists(registerDto.UserName)) return Conflict("Username is taken");
        if (await EmailInUse(registerDto.Email)) return Conflict("Email in use");

        var user = new AppUser
        {
            UserName = registerDto.UserName,
            Email = registerDto.Email
        };
        
        var result = await _userManager.CreateAsync(user, registerDto.Password);
        if (!result.Succeeded) return BadRequest("Failed to register user");

        var roleResult = await _userManager.AddToRoleAsync(user, "User");

        if (!roleResult.Succeeded) return BadRequest(result.Errors);
        return new OkResult();
    }

    [Authorize(Policy = "IsAdmin")]
    [HttpPut("roles/add")]
    public async Task<ActionResult> SetRoles(RoleDto roleDto)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.NormalizedUserName == roleDto.Username.ToUpper());
        if (user == null) return NotFound("User not found");
        if (await _userManager.IsInRoleAsync(user, roleDto.Role)) return BadRequest("User is already in that role");
        var result = await _userManager.AddToRoleAsync(user, roleDto.Role);
        return result.Succeeded ? Ok() : BadRequest("Something went wrong");
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var user = await _userManager.Users
            .SingleOrDefaultAsync(x => x.NormalizedUserName == loginDto.UserName.ToUpper());
        if (user == null) return Unauthorized();

        var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
        if (!result.Succeeded) return Unauthorized();

        return new UserDto
        {
            UserName = user.UserName,
            Id = user.Id,
            Token = await _tokenCreatorService.CreateToken(user),
            Roles = await _userManager.GetRolesAsync(user)
        };
    }

    private async Task<bool> UserExists(string username)
    {
        return await _userManager.Users.AnyAsync(x => x.NormalizedUserName == username.ToUpper());
    }

    private async Task<bool> EmailInUse(string email)
    {
        return await _userManager.Users.AnyAsync(x => x.NormalizedEmail == email.ToUpper());
    }
}