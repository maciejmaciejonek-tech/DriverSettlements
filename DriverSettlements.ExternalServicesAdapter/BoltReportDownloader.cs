using DriverSettlements.Common.Helpers;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace DriverSettlements.ExternalServicesAdapter
{
    public class BoltReportDownloader:WebViewReportDownloader
    {
        private readonly string _username;
        private readonly string _password;
        private readonly DateTime _dateFrom;
        private readonly DateTime _dateTo;
        public BoltReportDownloader(string companyId, string username, string password, DateTime dateFrom, DateTime dateTo, string reportLocation) 
            :base($"https://fleets.bolt.eu/v2/{companyId}/reports", reportLocation, true)
        {
            _username = username;
            _password = password;
            _dateFrom = dateFrom;
            _dateTo = dateTo;   
        }

        protected override async Task ExecuteLoginScripts(CoreWebView2 coreWebView2)
        {
            await coreWebView2.ExecuteScriptAsync($"document.getElementById('email').value='';document.getElementById('email').focus(); document.execCommand('insertText', false, '{_username}');");
            await coreWebView2.ExecuteScriptAsync($"document.getElementById('current-password').value='';document.getElementById('current-password').focus(); document.execCommand('insertText', false, '{_password}');");
            await coreWebView2.ExecuteScriptAsync("document.querySelectorAll(\"button[type='submit']\")[0].click();");
        }

        protected override bool IsRaportsPage(string html)
        {
            if (!html.Contains(@"<title>Kierowcy | Bolt</title>")
                && !html.Contains(@"<title>Earnings report | Bolt</title>"))
                return false;
            return true;
        }

        protected override async Task GetReportScripts(string html, CoreWebView2 coreWebView2)
        {
            var script = File.ReadAllText(ApplicationLocationProvider.GetSubDirectoryPath(new string[] { "Scripts", "Bolt-GenerateEarningsReport.txt" }))
                .Replace("{firstYear}", _dateFrom.Year.ToString()).Replace("{firstMonth}", _dateFrom.Month.ToString()).Replace("{firstDay}", _dateFrom.Day.ToString())
                .Replace("{secondYear}", _dateTo.Year.ToString()).Replace("{secondMonth}", _dateTo.Month.ToString()).Replace("{secondDay}", _dateTo.Day.ToString());
            await coreWebView2.ExecuteScriptAsync(script);
        }

        protected override bool CheckReportFile(string reportFilePath)
        {
            return true;
        }

        protected override bool IsPostLoginPage(string html)
        {
            throw new NotImplementedException();
        }

        protected override bool IsLoginPage(string html)
        {
            if (!html.Contains(@"<input id=""current-password"" type=""password"""))
                return false;
            return true;
        }

        protected override Task ExecuteSetReportsParameterScripts(CoreWebView2 coreWebView2)
        {
            throw new NotImplementedException();
        }
    }
}
