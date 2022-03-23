namespace Core.Utilities
{
    public static class StringUtillity
    {
        public static string HashPassword(this string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}