namespace VertivProject.Models
{
    public class OtpResponse
    {
        #nullable disable
        public string OtpCode { get; set; }
        public string Otp { get; set; }
        #nullable enable
        public long RemainingSeconds { get; set; }
    }
}
