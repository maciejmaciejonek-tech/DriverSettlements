using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverSettlements.Common.Models
{
    public partial class ExternalReport:ExternalReportMetadata
    {
        [Column(Order = 10)]
        public string? ReportBody { get; set; } = null;
        [Column(Order = 11)]
        public int TenantId { get; set; }
        [ForeignKey("TenantId")]
        public Tenant? Tenant { get; set; }
    }
}
