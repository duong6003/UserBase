using AutoMapper;
using Core.Utilities;
using Infrastructure.Modules.Users.Entities;
using Infrastructure.Modules.Users.Requests.UserRequests;
using Infrastructure.Persistence.Enums;

namespace Infrastructure.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<UserSignUpRequest, User>()
            .ForMember(dest => dest.Password, opt => opt.MapFrom((src, dest) => dest.Password = src.Password!.HashPassword()))
            .ForMember(dest => dest.Status, opt => opt.MapFrom((src, dest) => dest.Status = (byte)Status.Active));
    }
}
