using DriverSettlements.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DriverSettlements.Common.Models
{
    [Table("Drivers")]
    [Index(nameof(Id), IsUnique = true)]
    public class Driver:BaseEntity
    {
        [Column(Order = 7)]
        public string? UberName { get; set; } = null;
        [Column(Order = 8)]
        public string? BoltName { get; set; } = null;
        [Column(Order = 9)]
        public string? FreenowName { get; set; } = null;
        [Column(Order = 10)]
        public bool Confirmed { get; set; } = false;
        [Column(Order = 11)]
        public int TenantId { get; set; }
        [Column(Order = 12)]
        public bool ApplicationFeeChargeDriver { get; set; } = true;
        [Column(Order = 13)]
        public double Deposit { get; set; } = 0.0;

        [ForeignKey("DriverId")]
        public ICollection<FuelBill>? FuelBills { get; set; }

        public int GetNumberOfExternalServices()
        {
            var res = 0;
            if (!string.IsNullOrWhiteSpace(BoltName))
                res++;
            if (!string.IsNullOrWhiteSpace(UberName))
                res++;
            if (!string.IsNullOrWhiteSpace(FreenowName))
                res++;
            return res;
        }

        public HashSet<string> GetDriverNames()
        {
            var driverNames = new HashSet<string>() { this.CompanyName.Trim().ToLower() };
            foreach(var tempName in new string[] { this.BoltName, this.UberName, this.FreenowName })
            {
                if (!string.IsNullOrEmpty(tempName))
                {
                    var tempDriverNames = tempName.Split(';', StringSplitOptions.RemoveEmptyEntries).Select(n => n.Trim().ToLower());
                    foreach (var driverName in tempDriverNames)
                        driverNames.Add(driverName);
                }
            }
            return driverNames;
        }
    }
}
