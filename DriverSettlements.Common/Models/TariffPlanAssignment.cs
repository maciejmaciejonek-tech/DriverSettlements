using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverSettlements.Common.Models
{
    [Table("TariffPlanAssignments")]
    [Index(nameof(Id), IsUnique = true)]
    public class TariffPlanAssignment
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public int Id { get; set; }
        [Column(Order = 1)]
        public int TenantId { get; set; }
        [Column(Order = 2)]
        public int DriverId { get; set; }
        [Column(Order = 3)]
        public int YearNumber { get; set; }
        [Column(Order = 4)]
        public int WeekNumber { get; set; }
        [Column(Order = 5)]
        public int TariffPlanId { get; set; }
        [Column(Order = 6)]
        public double Cost { get; set; }
        [Column(Order = 7)]
        public bool Monday { get; set; } = true;
        [Column(Order = 8)]
        public bool Tuesday { get; set; } = true;
        [Column(Order = 9)]
        public bool Wednesday { get; set; } = true;
        [Column(Order = 10)]
        public bool Thursday { get; set; } = true;
        [Column(Order = 11)]
        public bool Friday { get; set; } = true;
        [Column(Order = 12)]
        public bool Saturday { get; set; } = true;
        [Column(Order = 13)]
        public bool Sunday { get; set; } = true;
        [Column(Order = 14)]
        public PeriodicType PeriodicType { get; set; } = PeriodicType.TariffPlanWithoutDetails;

        [ForeignKey("TenantId")]
        public Driver? Tenant { get; set; }
        [ForeignKey("DriverId")]
        public Driver? Driver { get; set; }
        [ForeignKey("TariffPlanId")]
        public TariffPlan? TariffPlan { get; set; }
    }
}