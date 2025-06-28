namespace Operational_Risk_Management.Models.Extensions
{
    public static class FileExtensions
    {
        /// <summary>
        /// saves a file in wwwroot path only
        /// </summary>
        /// <param name="file">file to be saved</param>
        /// <param name="path">fullpath only without filename</param>
        /// <returns>save file name</returns>
        public static async Task<string> SaveToAsync(this IFormFile file, string path)
        {
            path = Path.Combine("wwwroot", path);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            var filename = string.Concat(Path.GetFileNameWithoutExtension(file.FileName), DateTime.Now.Second, Path.GetExtension(file.FileName));
            var fullPath = Path.Combine(path, filename);
            using (var stream = File.Create(fullPath))
            {
                await file.CopyToAsync(stream);
            }
            return filename;
        }
    }
}
