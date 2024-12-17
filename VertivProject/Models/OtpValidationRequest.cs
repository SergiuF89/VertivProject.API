using Microsoft.AspNetCore.Mvc;

namespace VertivProject.Models
{
    public class OtpValidationRequest : OtpGeneratorRequest
    {
        #nullable disable
        public string OtpCode { get; set; }
        public string Otp { get; set; }
        public string UserInputOtpCode { get; set; }
        #nullable enable
    }
}
