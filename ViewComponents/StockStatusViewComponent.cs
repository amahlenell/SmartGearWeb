using Microsoft.AspNetCore.Mvc;

namespace SmartGearWeb.ViewComponents
{
    // Reusable UI piece: renders a stock-level badge wherever it's invoked.
    // Called from Views with: @await Component.InvokeAsync("StockStatus", new { stockQuantity = ... })
    public class StockStatusViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(int stockQuantity)
        {
            return View(stockQuantity);
        }
    }
}
