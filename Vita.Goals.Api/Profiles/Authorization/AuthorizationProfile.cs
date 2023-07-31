using AutoMapper;
using System.Security.Claims;
using Vita.Goals.Application.Commands.Shared;

namespace Vita.Goals.Api.Profiles.Authorization;
public class AuthorizationProfile : Profile
{
    public AuthorizationProfile()
    {
        CreateMap<ClaimsPrincipal, User>()
            .ForMember(x => x.Id, opt => opt.MapFrom(src => src.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value));

    }
}
