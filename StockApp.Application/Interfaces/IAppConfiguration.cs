namespace StockApp.Application.Interfaces
{
    public interface IAppConfiguration
    {
        string this[string key] { get; }
    }
}
