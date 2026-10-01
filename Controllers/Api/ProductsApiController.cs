using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartGearWeb.Models;
using SmartGearWeb.Services;

namespace SmartGearWeb.Controllers.Api
{
    // A separate, JSON-only Web API — distinct from ProductController, which
    // serves HTML views. Reuses the same repository, so both the MVC pages
    // and this API stay consistent with a single source of truth.
    [ApiController]
    [Route("api/products")]
    public class ProductsApiController : ControllerBase
    {
        private readonly IProductRepository _repository;
        private readonly ILogger<ProductsApiController> _logger;

        public ProductsApiController(IProductRepository repository, ILogger<ProductsApiController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        // GET api/products
        [HttpGet]
        public ActionResult<IEnumerable<ProductDto>> GetAll()
        {
            var products = _repository.GetAll().Select(ToDto);
            return Ok(products);
        }

        // GET api/products/5
        [HttpGet("{id:int}")]
        public ActionResult<ProductDto> GetById(int id)
        {
            var product = _repository.GetById(id);
            if (product == null)
                return NotFound(new { message = $"Product {id} not found." });

            return Ok(ToDto(product));
        }

        // POST api/products
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult<ProductDto> Create(ProductDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                CategoryId = dto.CategoryId,
                StockQuantity = dto.StockQuantity
            };

            _repository.Add(product);
            _logger.LogInformation("API created product {ProductId} ({ProductName}).", product.Id, product.Name);

            var result = ToDto(_repository.GetById(product.Id)!);

            // 201 Created, with a Location header pointing to GetById — the
            // correct REST response for a successful POST.
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        // PUT api/products/5
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Update(int id, ProductDto dto)
        {
            if (id != dto.Id) return BadRequest(new { message = "Route id and body id do not match." });
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existing = _repository.GetById(id);
            if (existing == null) return NotFound();

            existing.Name = dto.Name;
            existing.Description = dto.Description;
            existing.Price = dto.Price;
            existing.CategoryId = dto.CategoryId;
            existing.StockQuantity = dto.StockQuantity;

            _repository.Update(existing);
            _logger.LogInformation("API updated product {ProductId}.", id);

            return NoContent();
        }

        // DELETE api/products/5
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var existing = _repository.GetById(id);
            if (existing == null) return NotFound();

            _repository.Delete(id);
            _logger.LogInformation("API deleted product {ProductId}.", id);

            return NoContent();
        }

        private static ProductDto ToDto(Product product) => new()
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            CategoryId = product.CategoryId,
            CategoryName = product.Category?.Name ?? string.Empty,
            StockQuantity = product.StockQuantity
        };
    }
}
