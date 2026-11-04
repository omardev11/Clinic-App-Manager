namespace ClinicAppManagerDataLayer.DTO
{
    public class DataDTO
    {
        public int? Id { get; set; }
        public string AppUserId { get; set; }
        public string RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public DateTime? RevokedDate { get; set; }
    }
}
