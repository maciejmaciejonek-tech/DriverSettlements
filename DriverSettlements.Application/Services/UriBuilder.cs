using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverSettlements.App.Services
{
    public class UriBuilder
    {
        // URL of REST service
        public static string RestUrl = "https://driversettlements.macmac.pl/api/{0}/{1}";


        // URL of REST service (Android does not use localhost)
        // Use http cleartext for local deployment. Change to https for production
        public static string LocalhostUrl = DeviceInfo.Platform == DevicePlatform.Android ? "127.0.0.1" : "localhost";
        public static string Scheme = "https"; // or http
        public static string Port = "5001";

        //public static string RestUrl = $"{Scheme}://{LocalhostUrl}:{Port}/api/{{0}}/{{1}}";
        //public static string RestUrl = "https://localhost/webApi/api/{0}/{1}";
        //public static string RestUrl = "https://23.100.7.130/api/{0}/{1}";
        //public static string RestUrl = "https://driversettlementssystem.azurewebsites.net/api/{0}/{1}";

        public static Uri GetUri(string interfaceName, string methodName)
        {
            return new Uri(string.Format(RestUrl, interfaceName, methodName));
        }
    }
}
