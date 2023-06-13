namespace api.Interfaces
{
    public interface IPasswordHashingService
    {
        public PasswordHashResult HashPassword(string password);
        public bool ComparePassword(byte[] passwordSalt,byte[] passwordHash, string password);
    }
    
    public class PasswordHashResult
    {
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
    }
}