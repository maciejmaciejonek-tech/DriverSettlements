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
    public class BaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public int Id { get; set; } 
        [Column(Order = 1)]
        public string? EmailAddress { get; set; } = null;
        [Column(Order = 2)]
        public string CompanyName { get; set; } = string.Empty;
        [Column(Order = 3)]
        public string? FriendlyName { get; set; } = null;
        [Column(Order = 4)]
        public string? InvitationKey { get; set; } = null;
        [Column(Order = 5)]
        public bool Blocked { get; set; } = false;
        [Column(Order = 6)]
        public bool Deleted { get; set; } = false;

    }
}
