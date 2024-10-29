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
    public class BoltReportDownloader_old:WebViewReportDownloader
    {
        private readonly string _username;
        private readonly string _password;
        private readonly DateTime _dateFrom;
        private readonly DateTime _dateTo;
        public BoltReportDownloader_old(string companyId, string username, string password, DateTime dateFrom, DateTime dateTo, string reportLocation) 
            :base($"https://fleets.bolt.eu/company/{companyId}/reports/weekly", reportLocation, true, true)
        {
            _username = username;
            _password = password;
            _dateFrom = dateFrom;
            _dateTo = dateTo;   
        }

        protected override async Task ExecuteLoginScripts(CoreWebView2 coreWebView2)
        {
            //await coreWebView2.ExecuteScriptAsync($"document.getElementById('username').value='';document.getElementById('username').focus(); document.execCommand('insertText', false, '{_username}');");
            //await coreWebView2.ExecuteScriptAsync($"document.getElementById('password').value='';document.getElementById('password').focus(); document.execCommand('insertText', false, '{_password}');");
            //await coreWebView2.ExecuteScriptAsync("document.getElementById('ember7').click();");
            await coreWebView2.ExecuteScriptAsync($"document.getElementById('email').value='';document.getElementById('email').focus(); document.execCommand('insertText', false, '{_username}');");
            await coreWebView2.ExecuteScriptAsync($"document.getElementById('current-password').value='';document.getElementById('current-password').focus(); document.execCommand('insertText', false, '{_password}');");
            await coreWebView2.ExecuteScriptAsync("document.querySelectorAll(\"button[type='submit']\")[0].click();");
        }

        protected override bool IsRaportsPage(string html)
        {
            return html.Contains(@"<title>Tygodniowe raporty | Bolt</title>") 
                || html.Contains(@"<title>Weekly Reports | Bolt</title>")
                || html.Contains(@"<title>Wekelijks rapport | Bolt</title>");
        }

        protected override async Task GetReportScripts(string html, CoreWebView2 coreWebView2)
        {
            var reports = GetWeeklyReports(html);
            var reportKey = reports.FirstOrDefault(r => r.Value[0].Equals(_dateFrom) && r.Value[1].Equals(_dateTo)).Key;
            if (reportKey == null)
                throw new Exception($"Raport from {_dateFrom.ToString("yyyy-MM-dd")} to {_dateTo.ToString("yyyy-MM-dd")} does not exist");
            await coreWebView2.ExecuteScriptAsync($"document.getElementById('{reportKey}').click();");
        }

        private Dictionary<string, DateTime[]> GetWeeklyReports(string html)
        {
            if (!html.Contains(@"<title>Tygodniowe raporty | Bolt</title>") 
                && !html.Contains(@"<title>Weekly Reports | Bolt</title>")
                && !html.Contains(@"<title>Wekelijks rapport | Bolt</title>"))
                throw new Exception($"<{this.GetType().Name}.ERR1>");
            var idx = html.IndexOf(@"<h3 class=""dashhead-title"">Weekly Reports</h3>");
            if (idx==-1)
                idx = html.IndexOf(@"<h3 class=""dashhead-title"">Tygodniowe raporty</h3>");
            if (idx == -1)
                idx = html.IndexOf(@"<h3 class=""dashhead-title"">Wekelijks rapport</h3>");
            if (idx == -1)
                throw new Exception($"<{this.GetType().Name}.ERR2>");
            html = html.Substring(idx);
            var beginIdx = html.IndexOf(@"<tbody>");
            if (beginIdx < 0)
                throw new Exception($"<{this.GetType().Name}.ERR3>");
            var endIdx = html.IndexOf(@"</tbody>");
            if (endIdx < 0)
                throw new Exception($"<{this.GetType().Name}.ERR4>");
            var tBody = html.Substring(beginIdx, endIdx - beginIdx + 8);
            var tab = tBody.Split(new string[] { @"<tr>" }, StringSplitOptions.RemoveEmptyEntries);
            if (tab.Length < 2)
                throw new Exception($"<{this.GetType().Name}.ERR5>");
            var res = new Dictionary<string, DateTime[]>();
            var regex = new Regex(@"(\d{2})\.(\d{2})\.(\d{4}) - (\d{2})\.(\d{2})\.(\d{4})");
            foreach (var item in tab.Skip(1))
            {
                try
                {
                    var itemTab = item.Split(new string[] { @"<td>" }, StringSplitOptions.RemoveEmptyEntries).Skip(1).ToArray();
                    var key = itemTab[1].Substring(itemTab[1].IndexOf(@" id=") + 5);
                    key = key.Substring(0, key.IndexOf(" class=") - 1);
                    var match = regex.Match(itemTab[0].Substring(0, itemTab[0].IndexOf(@"</td>")));
                    if (!match.Success)
                        throw new Exception($"Bolt.<ERR6>\r\n{item}");
                    res.Add(key, new DateTime[] {
                        new DateTime(int.Parse(match.Groups[3].Value),int.Parse(match.Groups[2].Value),int.Parse(match.Groups[1].Value)),
                        new DateTime(int.Parse(match.Groups[6].Value),int.Parse(match.Groups[5].Value),int.Parse(match.Groups[4].Value))
                    });
                }
                catch (Exception ex)
                {
                    throw new Exception($"{this.GetType().Name}.<ERR7>\r\n{item}\r\n{ex.Message}");
                }
            }
            return res;
        }

        protected override bool CheckReportFile(string reportFilePath)
        {
            return true;
        }

        protected override bool IsPostLoginPage(string html)
        {
            if (!html.Contains(@"<title>Kierowcy | Bolt</title>")
                && !html.Contains(@"<title>Drivers | Bolt</title>"))
                return false;
            return true;
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
