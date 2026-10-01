using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace SmartGearWeb.Services
{
    public interface IImageUploadService
    {
        Task<string?> UploadAsync(IFormFile file);
    }

    // Uploads product images to Cloudinary (cloud object storage) instead of
    // saving them on the web server's own disk. This matters because the
    // server's disk is not persistent/shared once deployed (a new deploy, or
    // running multiple server instances, would otherwise lose or miss files).
    public class CloudinaryImageService : IImageUploadService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryImageService(IConfiguration configuration)
        {
            var account = new Account(
                configuration["Cloudinary:CloudName"],
                configuration["Cloudinary:ApiKey"],
                configuration["Cloudinary:ApiSecret"]);

            _cloudinary = new Cloudinary(account);
        }

        public async Task<string?> UploadAsync(IFormFile file)
        {
            if (file.Length == 0) return null;

            await using var stream = file.OpenReadStream();

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = "smartgear-products",
                Transformation = new Transformation().Width(800).Height(800).Crop("limit")
            };

            var result = await _cloudinary.UploadAsync(uploadParams);

            // The secure HTTPS URL is what gets stored on the Product and
            // rendered directly in <img> tags — no local file ever touches
            // our own server's disk.
            return result.SecureUrl?.ToString();
        }
    }
}
