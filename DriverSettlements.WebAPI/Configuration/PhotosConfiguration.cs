using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverSettlements.WebAPI.Configuration
{
    public class PhotosConfiguration
    {
        public const string SectionName = "Photos";
        public string PhotosFolder { get; set; } = string.Empty;
    }
}
