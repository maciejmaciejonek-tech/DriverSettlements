using DriverSettlements.Common.Helpers;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace DriverSettlements.ExternalServicesAdapter
{
    public class FreenowReportDownloader:WebViewReportDownloader
    {
        private readonly string _username;
        private readonly string _password;
        private readonly DateTime _dateFrom;
        private readonly DateTime _dateTo;
        public FreenowReportDownloader(string username, string password, DateTime dateFrom, DateTime dateTo, string reportLocation) 
            :base("https://portal.free-now.com/earnings",reportLocation)
        {
            _username = username;
            _password = password;
            _dateFrom = dateFrom;
            _dateTo = dateTo;   
        }

        protected override async Task ExecuteLoginScripts(CoreWebView2 coreWebView2)
        {
            await coreWebView2.ExecuteScriptAsync($"document.getElementById('username').value='';document.getElementById('username').focus(); document.execCommand('insertText', false, '{_username}');");            
            await coreWebView2.ExecuteScriptAsync($"document.getElementById('password').value='';document.getElementById('password').focus(); document.execCommand('insertText', false, '{_password}');");
            await coreWebView2.ExecuteScriptAsync($"document.querySelectorAll(\"button[data-testid='login-button']\")[0].click()");
        }

        protected override bool IsRaportsPage(string html)
        {
            return html.Contains(">Earnings</h1>");
        }

        protected override async Task GetReportScripts(string html, CoreWebView2 coreWebView2)
        {
            //set report type
            //document.querySelectorAll("div[data-baseweb='select']")[0].childNodes[0].click();
            //Array.from(document.querySelectorAll('div[aria-selected]')).find(el => el.textContent === 'Payments Driver').click();

            //set page size
            //document.querySelectorAll("button[data-tracking-name='rows']")[0].click();
            //document.querySelectorAll("div[data-tracking-name='page-size-50']")[0].click()

            //set settlement
            //document.querySelectorAll("button[data-tracking-name='settlement-view']")[0].click();

            //set custom report
            //document.querySelectorAll("button[data-tracking-name='custom-date-range']")[0].click();

            //set start date
            //document.evaluate("//div[contains(text(),'Start date')]",document).iterateNext();


            //set last dates period
            //document.querySelectorAll("div[data-baseweb='select']")[1].firstChild.click();
            //document.querySelectorAll("ul[role='listbox']")[0].lastChild.firstChild.click();

            //Thread.Sleep(1000);
            await Task.Delay(1000);
            await coreWebView2.ExecuteScriptAsync($"document.querySelectorAll(\"input[data-testid='start-date-input']\")[0].focus();");
            await coreWebView2.ExecuteScriptAsync($"document.querySelectorAll(\"input[data-testid='start-date-input']\")[0].select();");
            await coreWebView2.ExecuteScriptAsync($"document.execCommand('insertText', false, '{_dateFrom.ToString("dd/MM/yyyy").Replace('.', '/')}');");
            //Thread.Sleep(1000);
            await Task.Delay(1000);
            await coreWebView2.ExecuteScriptAsync($"document.querySelectorAll(\"input[data-testid='end-date-input']\")[0].focus();");
            await coreWebView2.ExecuteScriptAsync($"document.querySelectorAll(\"input[data-testid='end-date-input']\")[0].select();");
            await coreWebView2.ExecuteScriptAsync($"document.execCommand('insertText', false, '{_dateTo.ToString("dd/MM/yyyy").Replace('.', '/')}');");
            //Thread.Sleep(1000);
            await Task.Delay(1000);
            await coreWebView2.ExecuteScriptAsync($"document.querySelectorAll(\"button[data-testid='download-csv-file']\")[0].click();");
        }

        protected override bool CheckReportFile(string reportFilePath)
        {
            var directoryName = Path.GetDirectoryName(reportFilePath);
            if (directoryName == null)
                throw new Exception("");
            var subDirectoryInfo = new DirectoryInfo(Path.Combine(directoryName, "temp"));
            GC.Collect();
            if (subDirectoryInfo.Exists)
                subDirectoryInfo.Delete(true);
            subDirectoryInfo.Create();
            try
            {
                ZipFile.ExtractToDirectory(reportFilePath, subDirectoryInfo.FullName);
                //var fileInfo = subDirectoryInfo.GetFiles("*.csv").FirstOrDefault();
                var fileInfo = Files.GetFileInfoFromFreeNowTempFolder(subDirectoryInfo);
                if (fileInfo == null || !fileInfo.Exists)
                    return false;
                if (!fileInfo.Name.Contains(_dateFrom.ToString("yyyy-MM-dd")) || !fileInfo.Name.Contains(_dateTo.ToString("yyyy-MM-dd")))
                    return false;
                return true;
            }
            finally
            {
                GC.Collect();
                subDirectoryInfo.Delete(true);
            }
        }

        protected override bool IsPostLoginPage(string html)
        {
            throw new NotImplementedException();
        }

        protected override bool IsLoginPage(string html)
        {
            throw new NotImplementedException();
        }

        protected override Task ExecuteSetReportsParameterScripts(CoreWebView2 coreWebView2)
        {
            throw new NotImplementedException();
        }
    }
}
