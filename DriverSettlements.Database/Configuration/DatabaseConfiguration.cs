using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverSettlements.Database.Configuration
{
    public class DatabaseConfiguration
    {
        public const string SectionName = "Database";
        public string Path { get; set; } = string.Empty;
    }
}
