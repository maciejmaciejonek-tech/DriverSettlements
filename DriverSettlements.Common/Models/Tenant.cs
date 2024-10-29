using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverSettlements.Common.Models
{
    [Table("Tenants")]
    [Index(nameof(Id), IsUnique = true)]
    public class Tenant: LegalEntity
    {
        [Column(Order = 9)]
        public string? Contacts { get; set; } = null;
        [Column(Order = 10)]
        public string? DriversInfos { get; set; } = null;
        [Column(Order = 11)]
        public string? UberCompanyId { get; set; } = null;
        [Column(Order = 12)]
        public string? UberUserName { get; set; } = null;
        [Column(Order = 13)]
        public string? UberPassword { get; set; } = null;
        [Column(Order = 14)]
        public string? BoltCompanyId { get; set; } = null;
        [Column(Order = 15)]
        public string? BoltUserName { get; set; } = null;
        [Column(Order = 16)]
        public string? BoltPassword { get; set; } = null;
        [Column(Order = 17)]
        public string? FreenowUserName { get; set; } = null;
        [Column(Order = 18)]
        public string? FreenowPassword { get; set; } = null;
        [Column(Order = 19)]
        public string? ReportDefinitionIds { get; set; } = null;
    }
}
