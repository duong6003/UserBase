using System.ComponentModel;

namespace Infrastructure.Definitions;

public class Messages
{
    [DisplayName("Middlewares")]
    public static class Middlewares
    {
        public const string IPAddressForbidden = "Mes.Middlewares.IPAddress.Forbidden";
    }

    [DisplayName("Files")]
    public static class Files
    {
        public const string InValid = "Mes.Files.InValid";
        public const string OverSize = "Mes.Files.OverSize";
    }

    [DisplayName("Users")]
    public static class Users
    {
        public const string EmailAddressAlreadyExist = "Mes.Users.EmailAddress.AlreadyExist";
        public const string EmailAddressInvalid = "Mes.Users.EmailAddress.Invalid";

        public const string UserNameAlreadyExist = "Mes.Users.UserName.AlreadyExist";
        public const string UserNameNotExist = "Mes.Users.UserName.DoesNotExist";
        public const string UserNameInValidLength = "Mes.Users.UserName.InValidLength";
        public const string UserNameInvalid = "Mes.Users.UserName.Invalid";
        public const string UserNameEmpty = "Mes.Users.UserName.Empty";

        public const string IdNotFound = "Mes.Users.Id.NotFound";
        public const string IdIsRequired = "Mes.Users.Id.IsRequired";
        public const string ResetCodeNotValid = "Mes.Users.ResetCode.NotValid";
        public const string ResetPasswordSuccesfully = "Mes.Users.ResetPassword.Succesfully";

        public const string PasswordInvalid = "Mes.Users.Password.Invalid";
        public const string PasswordNotMatch = "Mes.Users.Password.NotMatch";
        public const string PasswordLengthInvalid = "Mes.Users.PasswordLength.LengthInvalid";
        public const string PasswordConfirmNotMatch = "Mes.Users.PasswordConfirm.NotMatch";

        public const string UpdateUserSuccessfully = "Mes.Users.Update.Successfully";
        public const string DeleteUserSuccessfully = "Mes.Users.Delete.Successfully";
        public const string CreateSuccessfully = "Mes.Users.Create.Successfully";
        public const string GetAllSuccessfully = "Mes.Users.GetAll.Successfully";
        public const string SendEmailSuccessfully = "Mes.Roles.SendEmail.Successfully";
        public const string LoginSuccess = "Mes.Users.Login.Success";
        public const string UserNameNotFound = "Mes.Users.UserName.NotFound";
        public const string UserIsLocked = "Mes.Users.IsLocked";
        public const string GetDetailSuccessfully = "Mes.Users.GetDetail.Successfully";

        public const string UserResetCodeValid = "Mes.Users.UserResetCode.Valid";
        public const string UserResetCodeInvalid = "Mes.Users.UserResetCode.Invalid";
        public const string UserResetSuccesfully = "Mes.Users.UserReset.Succesfully";
    }
    [DisplayName("Roles")]
    public static class Roles
    {
        public const string NameEmpty = "Mes.Roles.Name.NameIsEmpty";
        public const string NameInValid = "Mes.Roles.Name.InValid";

        public const string IdNotFound = "Mes.Roles.Id.NotFound";
        public const string IdIsRequired = "Mes.Roles.Id.IsRequired";
        public const string UpdateRoleSuccessfully = "Mes.Roles.Update.Successfully";
        public const string DeleteRoleSuccessfully = "Mes.Roles.Delete.Successfully";
        public const string CreateSuccessfully = "Mes.Roles.Create.Successfully";
        public const string GetAllSuccessfully = "Mes.Roles.GetAll.Successfully";
        public const string GetDetailSuccessfully = "Mes.Roles.GetDetail.Successfully";
    }
    [DisplayName("RolePermissions")]
    public static class RolePermissions
    {
        public const string NameInValid = "Mes.RolePermissions.Name.InValid";
         public const string IdNotFound = "Mes.RolePermissions.Id.NotFound";
        public const string IdIsRequired = "Mes.RolePermissions.Id.IsRequired";
        public const string UpdateRolePermissionSuccessfully = "Mes.RolePermissions.Update.Successfully";
        public const string DeleteRolePermissionSuccessfully = "Mes.RolePermissions.Delete.Successfully";
        public const string CreateSuccessfully = "Mes.RolePermissions.Create.Successfully";
        public const string GetAllSuccessfully = "Mes.RolePermissions.GetAll.Successfully";
        public const string GetDetailSuccessfully = "Mes.RolePermissions.GetDetail.Successfully";
    }
    [DisplayName("Permissions")]
    public static class Permissions
    {
        public const string IdNotFound = "Mes.Permissions.Id.NotFound";
        public const string IdIsRequired = "Mes.Permissions.Id.IsRequired";
        public const string NameEmpty = "Mes.Permissions.Name.NameIsEmpty";
        public const string NameInValid = "Mes.Permissions.Name.InValid";
        public const string GetAllSuccessfully = "Mes.Permissions.GetAll.Successfully";

    }
}
