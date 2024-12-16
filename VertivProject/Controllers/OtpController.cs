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
        private readonly AppSettings _appSettings;

        public OtpController(OtpService otpService, IOptions<AppSettings> appSettings)
        {
            _otpService = otpService;
            _appSettings = appSettings.Value;
        }

        [HttpPost]
        [Route("GenerateOtpKey")]
        public ActionResult<OtpResponse> GenerateOtpKey([FromBody] OtpRequest request)
        {
            // Generate OTP with timestamp
            var otp = _otpService.GenerateOtp(request);

            // Create the response with OTP and remaining time
            var response = new OtpResponse()
            {
                Otp = otp,
                RemainingSeconds = _appSettings.KeyValidationPeriod
            };

            return Ok(response);
        }

        [HttpPost]
        [Route("ValidateOtp")]
        public ActionResult<bool> ValidateOtp([FromBody] OtpValidationRequest request)
        {
            var remainingTime = _otpService.GetRemainingTime(request);
            return Ok(_otpService.ValidateOtp(request));
        }
    }
}
