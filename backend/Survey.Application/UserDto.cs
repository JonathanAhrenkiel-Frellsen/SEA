namespace Survey.Application
{
    public class UserDto
    {
        public int? UserId { get; set; }
        public string? UserName { get; set; }
        public string? UserEmail { get; set; }
        public string? UserPassword { get; set; }
        public int? UserTypeId { get; set; }
    }
    
    public class RegisterUserDto
    {
        public string? UserName { get; set; }
        public string? UserEmail { get; set; }
        public string? UserPassword { get; set; }
        public int? UserTypeId { get; set; }
    }
}
