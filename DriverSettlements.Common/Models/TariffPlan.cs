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
    [Table("TariffPlans")]
    [Index(nameof(Id), IsUnique = true)]
    public class TariffPlan
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public int Id { get; set; }
        [Column(Order = 1)]
        public int TenantId { get; set; }
        [Column(Order = 2)]
        public string Name { get; set; } = string.Empty;
        [Column(Order = 3)]
        public double WeeklyRate { get; set; }
        [Column(Order = 4)]
        public bool Enabled { get; set; }

        [ForeignKey("TenantId")]
        public Driver? Tenant { get; set; }
    }
}