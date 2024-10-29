using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverSettlements.TaskExecutor.Configuration
{
    public class ServiceConfiguration
    {
        public const string SectionName = "Service";
        public string Uri { get; set; } = string.Empty;
    }
}
