using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverSettlements.Common.Models
{
    [Table("ApplicationInstances")]
    [Index(nameof(Id), IsUnique = true)]
    public class ApplicationInstance
    {
        [Key, Column(Order = 0)]
        public string Id { get; set; } = string.Empty;
        [Column(Order = 1)]
        public DateTime RegistrationDate { get; set; }
        [Column(Order = 2)]
        public DateTime LastLoginDate { get; set; }
        [Column(Order = 3)]
        public bool Locked { get; set; } = false;
        [Column(Order = 4)]
        public int? DriverId { get; set; } = null;
        [Column(Order = 5)]
        public int? TenantId { get; set; } = null;
        [Column(Order = 6)]
        public bool MasterAdmin { get; set; } = false;

        [ForeignKey("TenantId")]
        public Tenant? Tenant { get; set; }

        [ForeignKey("DriverId")]
        public Driver? Driver { get; set; }
    }
}
