using AutoMapper;
using Core.Utilities;
using Infrastructure.Modules.Users.Entities;
using Infrastructure.Modules.Users.Requests.RolePermissionRequests;
using Infrastructure.Modules.Users.Requests.RoleRequests;
using Infrastructure.Modules.Users.Requests.UserPermissionRequests;
using Infrastructure.Modules.Users.Requests.UserRequests;
using Infrastructure.Persistence.Definitions;

namespace Infrastructure.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // user
        CreateMap<UserSignUpRequest, User>()
            .ForMember(dest => dest.Avatar, opt => opt.Ignore())
            .ForMember(dest => dest.Password, opt => opt.MapFrom((src, dest) => dest.Password = src.Password!.HashPassword()))
            .ForMember(dest => dest.Status, opt => opt.MapFrom((src, dest) => dest.Status = (byte)Status.Active));
        CreateMap<UserUpdateRequest, User>();
        CreateMap<UpdateUserPermissionRequest, UserPermission>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
        CreateMap<CreateUserPermissionRequest, UserPermission>();
        // role
        CreateMap<CreateRoleRequest,Role>();
        CreateMap<UpdateRoleRequest,Role>();

        CreateMap<UpdateRolePermissionRequest, RolePermission>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
        CreateMap<CreateRolePermissionRequest, RolePermission>();
    }
}
