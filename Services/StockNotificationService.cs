namespace SmartGearWeb.Services
{
    // Interface so the service can be registered and injected by abstraction,
    // not by concrete type — this is what makes it testable/swappable.
    public interface IStockNotificationService
    {
        string GetLowStockMessage(string productName, int quantity);
    }

    // Concrete implementation registered in Program.cs via
    // builder.Services.AddScoped<IStockNotificationService, StockNotificationService>();
    public class StockNotificationService : IStockNotificationService
    {
        public string GetLowStockMessage(string productName, int quantity)
        {
            return quantity <= 5
                ? $"Warning: {productName} is low on stock ({quantity} left)."
                : $"{productName} stock level is healthy ({quantity} left).";
        }
    }
}
