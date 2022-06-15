namespace forgedinthelore_net.Services;

public interface ITokenValidatorService
{
    int? ValidateToken(string token);
}