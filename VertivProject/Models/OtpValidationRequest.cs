using Microsoft.AspNetCore.Mvc;

namespace VertivProject.Models
{
    public class OtpValidationRequest : OtpRequest
    {
        public string OtpCode { get; set; }
    }
}
