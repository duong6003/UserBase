// using Infrastructure.Modules.Users.Entities;
// using Infrastructure.Persistence.Contexts;

// namespace Infrastructure.Persistence.GlobalValidation
// {
//     public interface IGlobalValidationWrapper
//     {
//         IGlobalValidation<User>? Users { get; }
//         IGlobalValidation<RolePermission>? RolePermissions { get; }
//         IGlobalValidation<Permission>? Permissions { get; }
//         IGlobalValidation<UserPermission>? UserPermissions { get; }
//         IGlobalValidation<Role>? Roles { get; }
//     }
//     public class GlobalValidationWrapper : IGlobalValidationWrapper
//     {
//         private readonly ApplicationDbContext ApplicationDbContext;
//         public GlobalValidationWrapper(ApplicationDbContext applicationDbContext) => ApplicationDbContext = applicationDbContext;

//         private IGlobalValidation<User>? UsersValidation;
//         public IGlobalValidation<User> Users => UsersValidation ??= new GlobalValidation<User>(ApplicationDbContext);

//         private IGlobalValidation<RolePermission>? RolePermissionsValidation;
//         public IGlobalValidation<RolePermission> RolePermissions => RolePermissionsValidation ??= new GlobalValidation<RolePermission>(ApplicationDbContext);

//         private IGlobalValidation<Permission>? PermissionsValidation;
//         public IGlobalValidation<Permission> Permissions => PermissionsValidation ??= new GlobalValidation<Permission>(ApplicationDbContext);

//         private IGlobalValidation<UserPermission>? UserPermissionsValidation;
//         public IGlobalValidation<UserPermission> UserPermissions => UserPermissionsValidation ??= new GlobalValidation<UserPermission>(ApplicationDbContext);

//         private IGlobalValidation<Role>? RolesValidation;
//         public IGlobalValidation<Role> Roles => RolesValidation ??= new GlobalValidation<Role>(ApplicationDbContext);

//     }
// }