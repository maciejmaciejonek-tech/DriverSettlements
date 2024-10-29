using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DriverSettlements.Common.Models
{
    [Table("BillingWeeks")]
    [Index(nameof(Id), IsUnique = true)]
    public class BillingWeek
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public int Id { get; set; }
        [Column(Order = 1)]
        public int YearNumber { get; set; }
        [Column(Order = 2)]
        public int WeekNumber { get; set; }
        [Column(Order = 3)]
        public BillingWeekStatus Status { get; set; } = BillingWeekStatus.NotExists;
        [Column(Order = 4)]
        public int? NumberOfDrivers { get; set; } = null;
        [Column(Order = 5)]
        public double? ApplicationFeeUnitPrice { get; set; } = null;

        [Column(Order = 6)]
        public int TenantId { get; set; }
        [ForeignKey("TenantId")]
        public Tenant? Tenant { get; set; }

        [NotMapped]
        public Dictionary<ExternalReportType, List<double>>? DriverReportsValues { get; set; }
        [NotMapped]
        public List<string>? DriverReportsValueNames { get; set; }
        [NotMapped]
        public Dictionary<string,double> WeeklySettlementComponents { get; set;}


    }
}
