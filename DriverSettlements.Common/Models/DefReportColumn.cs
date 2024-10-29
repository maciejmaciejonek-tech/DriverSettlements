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
    [Table("DefReportColumns")]
    [Index(nameof(Id), IsUnique = true)]
    public class DefReportColumn
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public int Id { get; set; }

        [Column(Order = 1)]
        public int DefReportId { get; set; }

        [Column(Order = 2)]
        public int Lp { get; set; }

        [Column(Order = 3)]
        public string ColumnName { get; set; } = string.Empty;

        [Column(Order = 4)]
        public string? DefBoltValue{ get; set; } = null;

        [Column(Order = 5)]
        public string? DefUberValue { get; set; } = null;

        [Column(Order = 6)]
        public string? DefFreenowValue { get; set; } = null;

        [Column(Order = 7)]
        public string? DefValue { get; set; } = null;
    }
}
