using System.ComponentModel;

namespace Infrastructure.Definitions;

public class Messages
{
    [DisplayName("Middlewares")]
    public static class Middlewares
    {
        public const string IPAddressForbidden = "Mes.Middlewares.IPAddress.Forbidden";
    }

    [DisplayName("Users")]
    public static class Users
    {
        public const string EmailAddressAlreadyExist = "Mes.Users.EmailAddress.AlreadyExist";
        public const string IdNotFound = "Mes.Users.Id.NotFound";
        public const string GetDetailSuccessfully = "Mes.Users.GetDetail.Successfully";
    }
}
