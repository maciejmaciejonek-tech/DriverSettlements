using DriverSettlements.Common.Helpers;
using DriverSettlements.Common.Models;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace DriverSettlements.ExternalServicesAdapter
{
    public class UberReportDownloader:WebViewReportDownloader
    { 
        private readonly string _companyId;
        private readonly string _username;
        private readonly string _password;
        private readonly DateTime _dateFrom;
        private readonly DateTime _dateTo;
        public UberReportDownloader(string companyId, string username, string password, DateTime dateFrom, DateTime dateTo, string reportLocation) 
            :base($"https://supplier.uber.com/orgs/{companyId}/reports", reportLocation,false,false,true)
        {
            _companyId = companyId;
            _username = username;
            _password = password;
            _dateFrom = dateFrom;
            _dateTo = dateTo;   
        }

        protected override async Task ExecuteLoginScripts(CoreWebView2 coreWebView2)
        {
            await coreWebView2.ExecuteScriptAsync($"document.getElementById('PHONE_NUMBER_or_EMAIL_ADDRESS').value='';document.getElementById('PHONE_NUMBER_or_EMAIL_ADDRESS').focus(); document.execCommand('insertText', false, '{_username}');");
            await coreWebView2.ExecuteScriptAsync("var simulateMouseEvent = function(element, eventName, coordX, coordY){element.dispatchEvent(new MouseEvent(eventName, {view: window,bubbles: true,cancelable: true,clientX: coordX,clientY: coordY,button: 0}));};var button = document.getElementById('forward-button');var box = button.getBoundingClientRect(),coordX = box.left + (box.right - box.left) / 2,coordY = box.top + (box.bottom - box.top) / 2; simulateMouseEvent (button, \"mousedown\", coordX, coordY);");

            //await Task.Delay(1000);
            //await coreWebView2.ExecuteScriptAsync("var simulateMouseEvent = function(element, eventName, coordX, coordY){element.dispatchEvent(new MouseEvent(eventName, {view: window,bubbles: true,cancelable: true,clientX: coordX,clientY: coordY,button: 0}));};var button = document.getElementById('alt-PASSWORD');var box =button.getBoundingClientRect(),coordX = box.left + (box.right - box.left) / 2,coordY = box.top + (box.bottom - box.top) / 2; simulateMouseEvent (button, \"mousedown\", coordX, coordY);");
            //await Task.Delay(1000);

            await Task.Delay(1000);
            await coreWebView2.ExecuteScriptAsync("var simulateMouseEvent = function(element, eventName, coordX, coordY){element.dispatchEvent(new MouseEvent(eventName, {view: window,bubbles: true,cancelable: true,clientX: coordX,clientY: coordY,button: 0}));};var button = document.getElementById('alt-alternate-forms-option-modal');var box = button.getBoundingClientRect(),coordX = box.left + (box.right - box.left) / 2,coordY = box.top + (box.bottom - box.top) / 2; simulateMouseEvent (button, \"mousedown\", coordX, coordY);");
            await Task.Delay(1000);
            await coreWebView2.ExecuteScriptAsync("document.getElementById('alt-more-options-modal-password').click();");
            await Task.Delay(1000);

            await coreWebView2.ExecuteScriptAsync($"document.getElementById('PASSWORD').value='';document.getElementById('PASSWORD').focus(); document.execCommand('insertText', false, '{_password}');");
            await coreWebView2.ExecuteScriptAsync("var simulateMouseEvent = function(element, eventName, coordX, coordY){element.dispatchEvent(new MouseEvent(eventName, {view: window,bubbles: true,cancelable: true,clientX: coordX,clientY: coordY,button: 0}));};var button = document.getElementById('forward-button');var box =button.getBoundingClientRect(),coordX = box.left + (box.right - box.left) / 2,coordY = box.top + (box.bottom - box.top) / 2; simulateMouseEvent (button, \"mousedown\", coordX, coordY);");
        }
        protected override bool IsRaportsPage(string html)
        {
            return html.Contains("<title>Uber Supplier Portal</title>");
        }

        protected override async Task GetReportScripts(string html, CoreWebView2 coreWebView2)
        {
            int numberOfProbes = 20;
            var reports = GetWeeklyReports(html);
            var reportKey = reports.FirstOrDefault(r => r.Value[0].Equals(_dateFrom) && r.Value[1].Equals(_dateTo)).Key;
            if (reportKey == null)
            {
                //request for generating expected report
                var script = File.ReadAllText(ApplicationLocationProvider.GetSubDirectoryPath(new string[] { "Scripts", "GeneratePaymentsDriverReport.txt" }))
                    .Replace("{orgUUID}",_companyId)
                    .Replace("{startDate}",_dateFrom.ToString("yyyy-MM-dd"))
                    .Replace("{endDate}",_dateTo.ToString("yyyy-MM-dd"));
                await coreWebView2.ExecuteScriptAsync(script);
                await Task.Delay(10000);
                var probeNumber = 0;
                coreWebView2.Reload();
                await Task.Delay(10000);
                html = await GetHtml();
                while (IsInProgess(html) && probeNumber++ < numberOfProbes)
                {
                    await Task.Delay(10000);
                    html = await GetHtml();
                }
                if (probeNumber > numberOfProbes)
                    throw new Exception($"Raport from {_dateFrom.ToString("yyyy-MM-dd")} to {_dateTo.ToString("yyyy-MM-dd")} is still during generating");
                reports = GetWeeklyReports(html);
                reportKey = reports.FirstOrDefault(r => r.Value[0].Equals(_dateFrom) && r.Value[1].Equals(_dateTo)).Key;
                if (reportKey == null)
                    throw new Exception($"Raport from {_dateFrom.ToString("yyyy-MM-dd")} to {_dateTo.ToString("yyyy-MM-dd")} does not exist");
            }
            await coreWebView2.ExecuteScriptAsync($"document.querySelectorAll(\"button[data-testid='download-report-button']\")[{reportKey}].click()");
        }

        private bool IsInProgess(string html)
        {
            if (!html.Contains(@"<title>Uber Supplier Portal</title>"))
                throw new Exception("<ERRP1>");
            var idx = html.IndexOf(@"Report generator</h6>");
            if (idx == -1)
                throw new Exception("<ERRP2>");
            html = html.Substring(idx);
            var tab = html.Split(new string[] { @"data-testid=""payment-reporting-table-row""" }, StringSplitOptions.RemoveEmptyEntries);
            if (tab.Length < 2)
                throw new Exception("<ERRP3>");
            var regex = new Regex(@"^data-baseweb=.*>(\d{4})(\d{2})(\d{2})-(\d{4})(\d{2})(\d{2})-(.*)-.*$");
            idx = 0;
            foreach (var item in tab.Skip(1))
            {
                var items = item.Split(new string[] { @"<div " }, StringSplitOptions.RemoveEmptyEntries);
                if (items.Length < 18)
                    throw new Exception("<ERRP4>");
                var match = regex.Match(items[3]);
                if (!match.Success)
                    throw new Exception("<ERRP5>");
                if (match.Groups[7].Value.Equals("payments_driver"))
                { 
                    if ($"{match.Groups[1].Value}{match.Groups[2].Value}{match.Groups[3].Value}".Equals(_dateFrom.ToString("yyyyMMdd"))
                        && $"{match.Groups[4].Value}{match.Groups[5].Value}{match.Groups[6].Value}".Equals(_dateTo.ToString("yyyyMMdd")))
                    {
                        if (item.Contains("In progress", StringComparison.InvariantCultureIgnoreCase))
                            return true;
                        return false;
                    }
                }
                idx++;
            }
            throw new Exception($"Raport from {_dateFrom.ToString("yyyy-MM-dd")} to {_dateTo.ToString("yyyy-MM-dd")} does not exist");
        }

        private Dictionary<string, DateTime[]> GetWeeklyReports(string html)
        {
            if (!html.Contains(@"<title>Uber Supplier Portal</title>"))
                throw new Exception("<ERR1>");
            var idx = html.IndexOf(@"Report generator</h6>");
            if (idx == -1)
                throw new Exception("<ERR2>");
            html = html.Substring(idx);
            var tab = html.Split(new string[] { @"data-testid=""payment-reporting-table-row""" }, StringSplitOptions.RemoveEmptyEntries);
            if (tab.Length < 2)
                throw new Exception("<ERR3>");
            var res = new Dictionary<string, DateTime[]>();
            var regex = new Regex(@"^data-baseweb=.*>(\d{4})(\d{2})(\d{2})-(\d{4})(\d{2})(\d{2})-(.*)-.*$");
            idx = 0;
            foreach (var item in tab.Skip(1))
            {
                var items = item.Split(new string[] { @"<div " }, StringSplitOptions.RemoveEmptyEntries);
                if (items.Length < 18)
                    throw new Exception("<ERR4>");
                var match = regex.Match(items[3]);
                if (!match.Success)
                    throw new Exception("<ERR5>");
                if (match.Groups[7].Value.Equals("payments_driver"))
                    res.Add(idx.ToString(), new DateTime[] {
                        new DateTime(int.Parse(match.Groups[1].Value),int.Parse(match.Groups[2].Value),int.Parse(match.Groups[3].Value)),
                        new DateTime(int.Parse(match.Groups[4].Value),int.Parse(match.Groups[5].Value),int.Parse(match.Groups[6].Value))
                    });
                idx++;
            }
            return res;
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
            throw new NotImplementedException();
        }

        protected async override Task ExecuteSetReportsParameterScripts(CoreWebView2 coreWebView2)
        {
            //set page size
            await coreWebView2.ExecuteScriptAsync("document.querySelectorAll(\"button[data-tracking-name='rows']\")[0].click();");
            await coreWebView2.ExecuteScriptAsync("document.querySelectorAll(\"div[data-tracking-name='page-size-50']\")[0].click();");
            //Thread.Sleep(1000);
            await Task.Delay(1000);
        }
    }
}
