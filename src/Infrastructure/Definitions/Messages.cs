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
        public const string UserNameInValidLength = "Mes.Users.UserName.InValidLength";
        public const string UserNameInvalid = "Mes.Users.UserName.Invalid";

        public const string PasswordInvalid = "Mes.Users.Password.Invalid";
        public const string PasswordNotMatch= "Mes.Users.Password.NotMatch";
        public const string PasswordLengthInvalid = "Mes.Users.PasswordLength.LengthInvalid";
        public const string PasswordConfirmNotMatch = "Mes.Users.PasswordConfirm.NotMatch";


        public const string LoginSuccess = "Mes.Users.Login.Success";
        public const string CreateSuccess = "Mes.Users.Create.Success";
        public const string UserNameNotFound = "Mes.Users.UserName.NotFound";
        public const string IdNotFound = "Mes.Users.Id.NotFound";
        public const string GetDetailSuccessfully = "Mes.Users.GetDetail.Successfully";
    }
}
