using AutoMapper;
using forgedinthelore_net.DTOs;
using forgedinthelore_net.Entities;

namespace forgedinthelore_net.Helpers;

public class AutomapperProfiles : Profile
{
    public AutomapperProfiles()
    {
        CreateMap<AppUser, UserDto>();
    }
}