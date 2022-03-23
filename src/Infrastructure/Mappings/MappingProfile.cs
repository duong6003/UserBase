using AutoMapper;
using Infrastructure.Modules.Users.Entities;
using Infrastructure.Modules.Users.Requests;

namespace Infrastructure.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<UserSignUpRequest, User>()
            .ForMember(dest=> dest.Password, opt => opt.PreCondition(x=> x.));
    }
}
