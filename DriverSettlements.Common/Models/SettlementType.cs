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
    [Table("SettlementTypes")]
    [Index(nameof(Id), IsUnique = true)]

    public class SettlementType
    {
        [Key, Column(Order = 0)]
        public int Id { get; set; }
        [Column(Order = 1)]
        public string TypeName { get; set; } = string.Empty;
        [Column(Order = 2)]
        public bool Debit { get; set; } = false;
        [Column(Order = 3)]
        public double Factor { get; set; } = 0;
        [Column(Order = 4)]
        public bool Enabled { get; set; } = false;
        [Column(Order = 5)]
        public bool IncludeInReport { get; set; } = false;
        [Column(Order = 6)]
        public int? TenantId { get; set; } = null;

        [ForeignKey("TenantId")]
        public Tenant? Tenant { get; set; }
    }
}
