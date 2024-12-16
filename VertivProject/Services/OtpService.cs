using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;
using VertivProject.Models;

namespace VertivProject.Services
{
    public class OtpService
    {
        private readonly string _secretKey;
        private readonly long _otpValidationPeriod;
        private const int OtpValiditySeconds = 30;

        public OtpService(IOptions<AppSettings> appSettings)
        {
            _secretKey = appSettings.Value.OtpKey;
            _otpValidationPeriod = appSettings.Value.KeyValidationPeriod;
        }

        public string GenerateOtp(OtpRequest request)
        {
            // Ensure the DateTime is in UTC
            var currentTime = request.DateTime;

            // Format: UserId + DateTime (up to seconds)
            var key = $"{request.UserId}-{currentTime:yyyyMMddHHmmss}";

            using (var hmac = new HMACSHA1(Encoding.UTF8.GetBytes(_secretKey)))
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(key));

                // Ensure the OTP is always positive and 6 digits long
                var otpInt = Math.Abs(BitConverter.ToInt32(hash, 0));  // Use Math.Abs to ensure a positive value
                var otp = (otpInt % 1000000).ToString("D6");  // Modulo 1,000,000 to ensure 6 digits

                // Add 30 seconds to the current DateTime to set the OTP validity
                var expirationTime = currentTime.AddSeconds(_otpValidationPeriod);
                var timestamp = ((DateTimeOffset)expirationTime).ToUnixTimeSeconds();

                // Return the OTP with the expiration timestamp
                return $"{otp}-{timestamp}";
            }
        }

        public bool ValidateOtp(OtpValidationRequest request)
        {
            var generatedOtp = request.OtpCode ;//GenerateOtp(optRequest);

            var generatedOtpParts = generatedOtp.Split('-');
            if (generatedOtpParts.Length != 2) return false;

            var generatedOtpValue = generatedOtpParts[0];
            var generatedOtpTimestamp = long.Parse(generatedOtpParts[1]);

            // Convert the OTP timestamp to DateTime
            var otpExpirationTime = DateTimeOffset.FromUnixTimeSeconds(generatedOtpTimestamp);

            // Check if the time difference between request DateTime and OTP expiration is within valid range (e.g., 30 seconds)
            return otpExpirationTime >= request.DateTime;
        }


        public int GetRemainingTime(OtpValidationRequest request)
        {
            // Split the OTP to get the expiration timestamp
            var parts = request.OtpCode.Split('-');
            if (parts.Length != 2 || !long.TryParse(parts[1], out long expirationTimestamp))
            {
                // Invalid format; return 0 as the remaining time
                return 0;
            }

            // Convert the expiration timestamp to DateTime
            var expirationTime = DateTimeOffset.FromUnixTimeSeconds(expirationTimestamp).UtcDateTime;

            // Ensure we're working with UTC time
            var currentUtcTime = request.DateTime;

            // Calculate the remaining time in seconds
            var remainingTime = (int)(expirationTime - currentUtcTime).TotalSeconds;

            // Ensure the remaining time is not negative
            return Math.Max(remainingTime, 0);
        }
    }
}
