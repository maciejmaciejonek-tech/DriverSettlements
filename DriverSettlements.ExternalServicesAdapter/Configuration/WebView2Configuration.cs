using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverSettlements.ExternalServicesAdapter.Configuration
{
    public class WebView2Configuration
    {
        public const string SectionName = "WebView2";

        public bool UseFolders { get; set; } = false;
        public string BrowserExecutableFolder { get; set; } = string.Empty;
        public string UserDataFolder { get; set; } = string.Empty;
    }
}
