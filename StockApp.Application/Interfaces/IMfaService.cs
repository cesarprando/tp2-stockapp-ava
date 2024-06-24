namespace StockApp.Application.Interfaces
{
    public interface IMfaService
    {
        Task<string> GenerateOtp(int userId);
        Task<bool> ValidateOtp(string userOtp, int userId);
    }
}
