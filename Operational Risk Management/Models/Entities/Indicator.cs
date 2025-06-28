using Operational_Risk_Management.Models.Context;
using Operational_Risk_Management.Models.Interfaces;
using Org.BouncyCastle.Asn1.X509;

namespace Operational_Risk_Management.Models.Entities
{
    public class Indicator :BaseEntity, ISoftDelete
    {
        public Guid TemplateId { get; set; }
        public virtual Template Template { get; set; }

        public string KRIType { get; set; }

        public string RfNo { get; set; }


        public string? Process { get; set; } = "";
        public string? RiskArea { get; set; } = "";
        public string IndicatorName { get; set; }
        public string FrequencyOfReview { get; set; } = "Monthly";

        public int? TotalProcess { get; set; }

        public int TolerableBreaches { get; set; } = 0;

        // Removed UpdatedAt property to rely on BaseEntity.UpdatedDate
        // public DateTime? UpdatedAt { get; set; }

        public bool IsActive { get; set; } = true;

        public virtual ICollection<Submission> Submissions { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedDate { get; set; }
        public string? DeletedBy { get; set; }

    }
}
