using Microsoft.Extensions.Options;
using Moq;
using VertivProject.Interfaces;
using VertivProject.Models;

namespace VertivProject.Services
{
    [TestFixture]
    public class OtpServiceTests
    {
        private Mock<IOptions<AppSettings>> _mockAppSettings;
        private Mock<IEncriptionService> _mockEncriptionService;
        private OtpService _otpService;

        [SetUp]
        public void CanCreateInstance()
        {
            // Mock the IOptions<AppSettings>
            _mockAppSettings = new Mock<IOptions<AppSettings>>();
            _mockAppSettings.Setup(x => x.Value).Returns(new AppSettings
            {
                OtpSecretKey = "TestSecretKey",
                KeyValidationPeriod = 300,
                KeyValidationPeriodTolerance = 30
            });

            // Mock the EncriptionService
            _mockEncriptionService = new Mock<IEncriptionService>();
            _mockEncriptionService.Setup(x => x.EncryptString(It.IsAny<string>(), It.IsAny<string>())).Returns((string str, string key) => str);
            _mockEncriptionService.Setup(x => x.DecryptString(It.IsAny<string>(), It.IsAny<string>())).Returns((string str, string key) => str);

            // Instantiate the OtpService with the mocked dependencies
            _otpService = new OtpService(_mockAppSettings.Object, _mockEncriptionService.Object);
        }

        [Test]
        public void GenerateOtp_ShouldReturnValidOtpResponse()
        {
            // Arrange
            var request = new OtpGeneratorRequest
            {
                UserId = "TestUser",
                DateTime = DateTime.UtcNow
            };

            // Act
            var result = _otpService.GenerateOtp(request);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.NotNull(result);
                Assert.AreEqual(6, result.OtpCode.Length);
            });
        }

        [Test]
        public void ValidateOtp_ShouldReturnTrueForValidOtp()
        {
            OtpGeneratorRequest generateOtpRequest = new OtpGeneratorRequest
            {
                DateTime = DateTime.Now.AddSeconds(30),
                UserId = "testuser",
            };

            var generateOtpResponse = _otpService.GenerateOtp(generateOtpRequest);

            // Arrange
            var request = new OtpValidationRequest
            {
                Otp = generateOtpResponse.Otp,
                UserInputOtpCode = generateOtpResponse.OtpCode,
                DateTime = DateTime.Now
            };

            // Act
            var result = _otpService.ValidateOtp(request);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void ValidateOtp_ShouldReturnFalseForInvalidOtp()
        {
            // Arrange
            var request = new OtpValidationRequest
            {
                Otp = "123456-1234567890",
                UserInputOtpCode = "654321",
                DateTime = DateTime.UtcNow
            };

            // Act
            var result = _otpService.ValidateOtp(request);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void ValidateOtp_ShouldThrowExceptionForInvalidDecription()
        {
            // Arrange
            var request = new OtpValidationRequest
            {
                Otp = "otpcode",
                UserInputOtpCode = "123456",
                DateTime = DateTime.UtcNow
            };

            _mockEncriptionService.Setup(x => x.DecryptString(It.IsAny<string>(), It.IsAny<string>())).Throws<Exception>();

            // Act & Assert
            Assert.Throws<Exception>(() => _otpService.ValidateOtp(request));
        }
    }
}
