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
    [Table("SettlementItems")]
    [Index(nameof(Id), IsUnique = true)]
    public class SettlementItem
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
        public int SettlementTypeId { get; set; }
        [Column(Order = 6)]
        public bool Periodic { get; set; } = false;
        [Column(Order = 7)]
        public double Value { get; set; }
        [Column(Order = 8)]
        public string? Description { get; set; } = null;

        [ForeignKey("TenantId")]
        public Driver? Tenant { get; set; }
        [ForeignKey("DriverId")]
        public Driver? Driver { get; set; }
        [ForeignKey("SettlementTypeId")]
        public SettlementType? SettlementType { get; set; }
    }
}