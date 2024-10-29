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
    public class LegalEntity: BaseEntity
    {
        [Column(Order = 7)]
        public bool ApplicationFeeChargeDriver { get; set; }
        [Column(Order = 8)]
        public double ApplicationFeeBaseValue { get; set; }

    }
}
