using Microsoft.AspNetCore.Mvc;
using SmartGearWeb.Services;

namespace SmartGearWeb.ViewComponents
{
    // Shown in the shared layout on every page: warns if any products are
    // running low on stock. Reuses the IsLowStock business rule defined
    // on the Product model in Question 5, so the "low stock" definition
    // lives in exactly one place.
    public class LowStockAlertViewComponent : ViewComponent
    {
        private readonly IProductRepository _productRepository;

        public LowStockAlertViewComponent(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public IViewComponentResult Invoke()
        {
            var lowStockCount = _productRepository.GetAll().Count(p => p.IsLowStock);
            return View(lowStockCount);
        }
    }
}
