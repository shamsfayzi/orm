namespace Operational_Risk_Management.Models.Interfaces
{
    public interface IAttachment
    {

        public string? FileName { get; set; }
        public string? FileType { get; set; }
        public string? Size { get; set; }
    }
}
