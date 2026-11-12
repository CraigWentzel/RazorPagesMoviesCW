using Microsoft.AspNetCore.Http;

namespace RazorPagesMovie.Models
{
    public static class PosterStorage
    {
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        private const long MaxBytes = 2 * 1024 * 1024;   // 2 MB

        // Returns the saved file name, or null if the file is not allowed.
        public static async Task<string?> SaveAsync(IFormFile file, string webRootPath)
        {
            string extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension) || file.Length > MaxBytes)
            {
                return null;
            }

            string folder = Path.Combine(webRootPath, "posters");
            Directory.CreateDirectory(folder);

            string fileName = Guid.NewGuid().ToString("N") + extension;
            string fullPath = Path.Combine(folder, fileName);

            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return fileName;
        }
    }
}