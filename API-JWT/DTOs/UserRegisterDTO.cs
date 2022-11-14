namespace API_JWT.DTOs
{
    public class UserRegisterDTO
    {
        public string Email { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
