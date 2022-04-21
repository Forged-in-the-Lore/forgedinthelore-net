using forgedinthelore_net.Entities;

namespace forgedinthelore_net.Interfaces;

public interface ITokenService
{
    Task<string> CreateToken(AppUser user);
}