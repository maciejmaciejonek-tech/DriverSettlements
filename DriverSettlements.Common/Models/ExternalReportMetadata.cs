using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DriverSettlements.Common.Models.ExternalReport;

namespace DriverSettlements.Common.Models
{
    public class ExternalReportMetadata
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public int Id { get; set; }
        [Column(Order = 1)]
        public ExternalReportType ReportType { get; set; } = ExternalReportType.Unknown;
        [Column(Order = 2)]
        public int YearNumber { get; set; }
        [Column(Order = 3)]
        public int WeekNumber { get; set; }
        [Column(Order = 4)]
        public DateTime DateFrom { get; set; }
        [Column(Order = 5)]
        public DateTime DateTo { get; set; }
        [Column(Order = 6)]
        public DateTime RequestDate { get; set; }
        [Column(Order = 7)]
        public DateTime DownloadDate { get; set; }
        [Column(Order = 8)]
        public ExternalReportStatus Status { get; set; }
        [Column(Order = 9)]
        public string ErrorMessage { get; set; } = string.Empty;
        [NotMapped]
        public int NumberOfUnmatchedDrivers { set; get; } = 0;
        [NotMapped]
        public HashSet<int> NotComfirmedDriversIdentifiers { set; get; } = new HashSet<int>();

    }
}
