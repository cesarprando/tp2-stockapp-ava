using StockApp.Application.Interfaces;
using StockApp.Domain.Interfaces;

namespace StockApp.Application.Services
{
    public class MfaService : IMfaService
    {
        private readonly IUserRepository _userRepository;

        public MfaService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<string> GenerateOtp(int userId)
        {
            var otp = new Random().Next(100000, 999999).ToString();
            await _userRepository.SetOtpAsync(userId, otp);
            return otp;
        }

        public async Task<bool> ValidateOtp(string userOtp, int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            return user.Otp == userOtp;
        }
    }
}
