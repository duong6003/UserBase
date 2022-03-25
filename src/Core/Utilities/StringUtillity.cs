namespace Core.Utilities
{
    public static class StringUtillity
    {
        public static string HashPassword(this string password)
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            return hashedPassword;
        }
    }
}