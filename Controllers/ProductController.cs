using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using SmartGearWeb.Filters;
using SmartGearWeb.Hubs;
using SmartGearWeb.Models;
using SmartGearWeb.Services;

namespace SmartGearWeb.Controllers
{
    // Attribute routing: this controller responds at /products instead of
    // the default /Product, giving a shorter, more user-friendly URL.
    [Route("products")]
    public class ProductController : Controller
    {
        private readonly IProductRepository _repository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILogger<ProductController> _logger;
        private readonly IHubContext<ProductHub> _hubContext;
        private readonly IImageUploadService _imageService;

        public ProductController(
            IProductRepository repository,
            ICategoryRepository categoryRepository,
            ILogger<ProductController> logger,
            IHubContext<ProductHub> hubContext,
            IImageUploadService imageService)
        {
            _repository = repository;
            _categoryRepository = categoryRepository;
            _logger = logger;
            _hubContext = hubContext;
            _imageService = imageService;
        }

        // GET /products
        [HttpGet("")]
        [LogActionFilter]
        public IActionResult Index()
        {
            try
            {
                var products = _repository.GetAll();
                return View(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load product list.");
                return StatusCode(500);
            }
        }

        // GET /products/details/5
        [HttpGet("details/{id:int}")]
        [LogActionFilter]
        public IActionResult Details(int id)
        {
            try
            {
                var product = _repository.GetById(id);
                if (product == null)
                {
                    _logger.LogWarning("Product with id {ProductId} was requested but not found.", id);
                    return NotFound();
                }
                return View(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load product details for id {ProductId}.", id);
                return StatusCode(500);
            }
        }

        // GET /products/create
        [HttpGet("create")]
        [LogActionFilter]
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            PopulateCategoriesDropdown();
            return View();
        }

        // POST /products/create
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        [LogActionFilter]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Product product, IFormFile? imageFile)
        {
            if (!ModelState.IsValid)
            {
                PopulateCategoriesDropdown();
                return View(product);
            }

            try
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    product.ImageUrl = await _imageService.UploadAsync(imageFile);
                }

                _repository.Add(product);
                _logger.LogInformation("Product {ProductName} created with id {ProductId}.", product.Name, product.Id);

                await _hubContext.Clients.All.SendAsync("ProductsChanged", $"'{product.Name}' was added to the catalogue.");

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create product {ProductName}.", product.Name);
                ModelState.AddModelError(string.Empty, "Something went wrong while saving the product. Please try again.");
                PopulateCategoriesDropdown();
                return View(product);
            }
        }

        // GET /products/edit/5
        [HttpGet("edit/{id:int}")]
        [LogActionFilter]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id)
        {
            var product = _repository.GetById(id);
            if (product == null) return NotFound();

            PopulateCategoriesDropdown(product.CategoryId);
            return View(product);
        }

        // POST /products/edit/5
        [HttpPost("edit/{id:int}")]
        [ValidateAntiForgeryToken]
        [LogActionFilter]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, Product product, IFormFile? imageFile)
        {
            if (id != product.Id) return BadRequest();

            if (!ModelState.IsValid)
            {
                PopulateCategoriesDropdown(product.CategoryId);
                return View(product);
            }

            try
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    product.ImageUrl = await _imageService.UploadAsync(imageFile);
                }
                else
                {
                    // No new file chosen: keep whatever image URL was already saved
                    var existing = _repository.GetById(id);
                    product.ImageUrl = existing?.ImageUrl;
                }

                _repository.Update(product);
                _logger.LogInformation("Product {ProductId} updated.", product.Id);

                await _hubContext.Clients.All.SendAsync("ProductsChanged", $"'{product.Name}' was updated.");

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update product {ProductId}.", product.Id);
                ModelState.AddModelError(string.Empty, "Something went wrong while saving the changes. Please try again.");
                PopulateCategoriesDropdown(product.CategoryId);
                return View(product);
            }
        }

        // GET /products/delete/5
        [HttpGet("delete/{id:int}")]
        [LogActionFilter]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var product = _repository.GetById(id);
            if (product == null) return NotFound();
            return View(product);
        }

        // POST /products/delete/5
        [HttpPost("delete/{id:int}")]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [LogActionFilter]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var product = _repository.GetById(id);
                _repository.Delete(id);
                _logger.LogInformation("Product {ProductId} deleted.", id);

                var name = product?.Name ?? $"Product #{id}";
                await _hubContext.Clients.All.SendAsync("ProductsChanged", $"'{name}' was removed from the catalogue.");

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete product {ProductId}.", id);
                return StatusCode(500);
            }
        }

        // GET /products/api-demo
        [HttpGet("api-demo")]
        [LogActionFilter]
        [Authorize(Roles = "Admin")]
        public IActionResult ApiDemo() => View();

        private void PopulateCategoriesDropdown(int? selectedId = null)
        {
            ViewBag.Categories = new SelectList(_categoryRepository.GetAll(), "Id", "Name", selectedId);
        }
    }
}
