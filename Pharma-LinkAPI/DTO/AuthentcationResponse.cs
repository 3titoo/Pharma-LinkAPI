namespace Pharma_LinkAPI.DTO
{
    public class AuthentcationResponse
    {
        public string? Token { get; set; } = string.Empty;
        public string? UserName { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string? Role { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }
    }
}
