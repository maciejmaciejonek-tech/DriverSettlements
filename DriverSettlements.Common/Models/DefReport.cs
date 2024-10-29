using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverSettlements.Common.Models
{
    [Table("DefReports")]
    [Index(nameof(Id), IsUnique = true)]
    public class DefReport
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public int Id { get; set; }

        [Column(Order = 1)]
        public string ReportName { get; set; } = string.Empty;

        [Column(Order = 2)]
        public DateTime? DateFrom { get; set; } 

        [Column(Order = 3)]
        public DateTime? DateTo { get; set; }

        [ForeignKey("DefReportId")]
        public ICollection<DefReportColumn>? DefReportColumns { get; set; }
    }
}
