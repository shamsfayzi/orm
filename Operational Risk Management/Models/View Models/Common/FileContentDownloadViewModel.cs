namespace Operational_Risk_Management.Models.View_Models.Common
{
    public class FileContentDownloadViewModel
    {
        public byte[] FileContents { get; set; }
        public string ContentType { get; set; }
        public string FileName { get; set; }
    }
}
