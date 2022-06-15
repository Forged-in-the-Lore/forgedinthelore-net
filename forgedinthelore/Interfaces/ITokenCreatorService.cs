using forgedinthelore_net.Entities;

namespace forgedinthelore_net.Interfaces;

public interface ITokenCreatorService
{
    Task<string> CreateToken(AppUser user);
}