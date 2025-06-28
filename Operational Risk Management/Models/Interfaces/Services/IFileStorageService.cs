namespace Operational_Risk_Management.Models.Interfaces.Services
{
    public interface IFileStorageService
    {
        
        Task<string> SaveFileAsync(IFormFile file,string folderName);
       
        Task<bool> DeleteFileAsync(string fileUrl);

        Task<byte[]> ReadFileBytesAsync(string filePath);
    }
}
