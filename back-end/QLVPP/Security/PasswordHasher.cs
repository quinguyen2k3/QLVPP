using static BCrypt.Net.BCrypt;

namespace QLVPP.Security
{
    public static class PasswordHasher
    {
        private const int WorkFactor = 12;

        public static string HashPassword(string password)
        {
            return EnhancedHashPassword(password, workFactor: WorkFactor);
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            return Verify(password, hashedPassword);
        }
    }
}
