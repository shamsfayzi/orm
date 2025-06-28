using Operational_Risk_Management.Models.Interfaces;
using Operational_Risk_Management.Models.Interfaces.Services;

namespace Operational_Risk_Management.Models.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IApplicationDbContext _context;

        public FileStorageService(IWebHostEnvironment environment, IApplicationDbContext context)
        {
            _environment = environment;
            _context = context;
        }
        public async Task<string> SaveFileAsync(IFormFile file, string folderName)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File cannot be null or empty", nameof(file));
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", folderName);
            Directory.CreateDirectory(uploadsFolder);
            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/uploads/{folderName}/{fileName}";
        }
        public async Task<bool> DeleteFileAsync(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("File path cannot be null or empty", nameof(filePath));
            var fullPath = Path.Combine(_environment.WebRootPath, filePath.TrimStart('/'));
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
                return true;
            }
            return false;
        }

        public async Task<byte[]> ReadFileBytesAsync(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                // Consider throwing ArgumentNullException or returning null based on desired contract
                throw new ArgumentException("File path cannot be null or empty", nameof(filePath));

            var fullPath = Path.Combine(_environment.WebRootPath, filePath.TrimStart('/'));

            if (System.IO.File.Exists(fullPath))
            {
                return await System.IO.File.ReadAllBytesAsync(fullPath);
            }

            // Consider throwing FileNotFoundException or returning null
            return null;
        }
    }
}
