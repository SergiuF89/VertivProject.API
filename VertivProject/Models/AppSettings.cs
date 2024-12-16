namespace VertivProject.Models
{
    public class AppSettings
    {
        #nullable disable
        public string OtpSecretKey { get; set; }
        #nullable enable
        public long KeyValidationPeriod { get; set; }
        public long KeyValidationPeriodTolerance { get; set; }
    }
}
