using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverSettlements.Common.Models
{
    [Table("FuelBills")]
    [Index(nameof(Id), IsUnique = true)]
    public class FuelBill
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
        public string ReferenceNumber { get; set; } = string.Empty;
        [Column(Order = 6)]
        public double Cost { get; set; }
        [Column(Order = 7)]
        public DateTime Date { get; set; }
        [Column(Order = 8)]
        public FuelBillStatus Status { get; set; }

        /*
                [ForeignKey("TenantId")]
                public Driver? Tenant { get; set; }
                [ForeignKey("DriverId")]
                public Driver? Driver { get; set; }
        */
    }
}