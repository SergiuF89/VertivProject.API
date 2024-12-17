using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;
using VertivProject.Interfaces;
using VertivProject.Models;

namespace VertivProject.Services
{
    public class OtpService
    {
        private readonly string _secretKey;
        private readonly long _otpValidationPeriod;
        private readonly long _otpValidationTolerance;
        private readonly IEncriptionService _encriptionService;

        public OtpService(IOptions<AppSettings> appSettings, IEncriptionService encriptionService)
        {
            _secretKey = appSettings.Value.OtpSecretKey;
            _otpValidationPeriod = appSettings.Value.KeyValidationPeriod;
            _encriptionService = encriptionService;
            _otpValidationTolerance = appSettings.Value.KeyValidationPeriodTolerance;
        }

        public OtpResponse GenerateOtp(OtpGeneratorRequest request)
        {
            try
            {
                var key = $"{request.UserId}-{request.DateTime:yyyyMMddHHmmss}";

                using (var hmac = new HMACSHA1(Encoding.UTF8.GetBytes(_secretKey)))
                {
                    var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(key));
                    var otpInt = Math.Abs(BitConverter.ToInt32(hash, 0));
                    var otp = (otpInt % 1000000).ToString("D6");
                    var expirationTime = request.DateTime.AddSeconds(_otpValidationPeriod + _otpValidationTolerance);
                    var timestamp = ((DateTimeOffset)expirationTime).ToUnixTimeSeconds();

                    return new OtpResponse
                    {
                        Otp = _encriptionService.EncryptString($"{otp}-{timestamp}", _secretKey),
                        OtpCode = otp,
                        RemainingSeconds = _otpValidationPeriod
                    };
                }
            }
            catch (Exception)
            {
                throw new Exception("Some error occured");
            }
        }

        public bool ValidateOtp(OtpValidationRequest request)
        {
            try
            {
                var generatedOtp = _encriptionService.DecryptString(request.Otp, _secretKey);
                var generatedOtpCode = generatedOtp.Split('-')[0];

                // Verify if user input otp code matches the generated otp code
                if (generatedOtpCode != request.UserInputOtpCode)
                {
                    return false;
                }

                var generatedOtpParts = generatedOtp.Split('-');

                if (generatedOtpParts.Length != 2) return false;

                var generatedOtpValue = generatedOtpParts[0];
                var generatedOtpTimestamp = long.Parse(generatedOtpParts[1]);

                var otpExpirationTime = DateTimeOffset.FromUnixTimeSeconds(generatedOtpTimestamp);

                return otpExpirationTime >= request.DateTime;
            }
            catch (Exception)
            {
                throw new Exception("Some error occured");
            }
        }
    }
}
