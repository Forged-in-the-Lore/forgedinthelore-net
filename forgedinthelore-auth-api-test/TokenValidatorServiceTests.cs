using System.Collections.Generic;
using System.Threading.Tasks;
using forgedinthelore_net.Entities;
using forgedinthelore_net.Interfaces;
using forgedinthelore_net.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace forgedinthelore_auth_api_test;

public class TokenValidatorServiceTests
{
    private Mock<IConfiguration> _configMock;
    private Mock<UserManager<AppUser>> _userManagerMock;
    private ITokenCreatorService _tokenCreatorService;
    private ITokenValidatorService _tokenValidatorService;

    [SetUp]
    public void Setup()
    {
        _configMock = new Mock<IConfiguration>();
        _configMock.Setup(c => c.GetSection("TokenKey").Value).Returns("3DD164BCFE8D1D8AF122E8CFB86B2");

        _userManagerMock = new Mock<UserManager<AppUser>>(Mock.Of<IUserStore<AppUser>>(), null, null, null, null, null,
            null, null, null);
        _userManagerMock.Setup(u =>
                u.GetRolesAsync(It.IsAny<AppUser>()))
            .ReturnsAsync(new List<string> {"Admin"});

        //Can't mock the token service because we have to have a valid token to test the token validator
        _tokenCreatorService = new TokenCreatorService(_configMock.Object, _userManagerMock.Object);
        _tokenValidatorService = new TokenValidatorService(_configMock.Object);
    }

    [Test]
    public async Task ValidateToken_ValidToken_ReturnsUserId()
    {
        //Arrange
        var appUser = new AppUser
        {
            UserName = "Test User",
            Id = 1
        };
        var token = await _tokenCreatorService.CreateToken(appUser);

        //Act
        var result = _tokenValidatorService.ValidateToken(token);
        Assert.That(result, Is.EqualTo(1));
    }

    [Test]
    public void ValidateToken_TokenFromOtherIssuer_ReturnsNull()
    {
        //Arrange
        const string token =
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c";

        //Act
        var result = _tokenValidatorService.ValidateToken(token);

        //
        Assert.That(result, Is.Null);
    }

    [Test]
    public void ValidateToken_TokenIsNotJwt_ReturnsNull()
    {
        //Arrange
        const string token = "ThisIsNotAJwtToken";

        //Act
        var result = _tokenValidatorService.ValidateToken(token);
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task ValidateToken_TokenChanged_ReturnsNull()
    {
        //Arrange
        var appUser = new AppUser
        {
            UserName = "Test User",
            Id = 1
        };
        var token = await _tokenCreatorService.CreateToken(appUser);

        var tokenParts = token.Split(".");
        //Fix padding so payload is length is divisible by 4 and convert to string. Pad
        var payload =
            System.Text.Encoding.UTF8.GetString(
                System.Convert.FromBase64String(tokenParts[1] + new string('=', tokenParts[1].Length % 4)));
        var payloadObject = JObject.Parse(payload);
        payloadObject["exp"] = (payloadObject["exp"].Value<int>() + 1000000).ToString();
        tokenParts[1] = System.Convert.ToBase64String(
            System.Text.Encoding.UTF8.GetBytes(payloadObject.ToString()));
        token = string.Join(".", tokenParts);


        //Act
        var result = _tokenValidatorService.ValidateToken(token);
        Assert.That(result, Is.Null);
    }
}