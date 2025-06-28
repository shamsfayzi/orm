using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata.Ecma335;

namespace Operational_Risk_Management.Models.Context
{
    public class BaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; } 
        public DateTime CreateDate { get; set; }  = DateTime.Now;
        public string? CreateBy { get; set; }
        public string? CreatedByFullName { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdateBy { get; set; }
        public string? UpdatedByFullName { get; set; }
    }
}
