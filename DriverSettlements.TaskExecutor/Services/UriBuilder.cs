using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverSettlements.TaskExecutor.Services
{
    public class UriBuilder
    {
        // URL of REST service
        //public static string RestUrl = "https://109.233.95.159/api/{0}/{1}";
        //public static string RestUrl = "https://localhost/webApi/api/{0}/{1}";
        public static string RestUrl = "https://localhost/api/{0}/{1}";

        public static Uri GetUri(string interfaceName, string methodName)
        {
            return new Uri(string.Format(RestUrl, interfaceName, methodName));
        }
    }
}
