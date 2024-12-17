using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using VertivProject.Models;
using VertivProject.Services;

namespace VertivProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OtpController : ControllerBase
    {
        private readonly OtpService _otpService;

        public OtpController(OtpService otpService)
        {
            _otpService = otpService;
        }

        [HttpPost]
        [Route("GenerateOtp")]
        public ActionResult<OtpResponse> GenerateOtpKey([FromBody] OtpGeneratorRequest request)
        {
            return Ok(_otpService.GenerateOtp(request));
        }

        [HttpPost]
        [Route("ValidateOtp")]
        public ActionResult<bool> ValidateOtp([FromBody] OtpValidationRequest request)
        {
            return Ok(_otpService.ValidateOtp(request));
        }
    }
}
