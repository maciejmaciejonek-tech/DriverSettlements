using Accessibility;
using DriverSettlements.Common.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Web.WebView2.WinForms;
using System.Drawing.Text;
using System.Globalization;
using System.Runtime.InteropServices;

namespace DriverSettlements.ExternalServicesAdapter
{
    internal static class Program
    {
        public static ExternalServicesAdapter.Configuration.WebView2Configuration? WebView2Configuration;
        [DllImport("kernel32.dll")]
        static extern bool AttachConsole(int dwProcessId);
        private const int ATTACH_PARENT_PROCESS = -1;

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            ExternalReportType? reportType;
            string companyCode; 
            string username; 
            string password;
            DateTime dateFrom;
            DateTime dateTo;
            string reportLocation;
            try
            {
                var cultureInfo = new CultureInfo("en-US");
                Thread.CurrentThread.CurrentCulture = cultureInfo;
                Thread.CurrentThread.CurrentUICulture = cultureInfo;
                CultureInfo.CurrentCulture = cultureInfo;
                AttachConsole(ATTACH_PARENT_PROCESS);
                var builder = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                WebView2Configuration = builder.Build()
                    .GetSection(Configuration.WebView2Configuration.SectionName)
                    .Get<Configuration.WebView2Configuration>();

                if (args == null || args.Length != 7)
                   throw new Exception("Invalid number of parameters");
                reportType = StringToExternalReportType(args[0]);
                companyCode = args[1];
                username = args[2];
                password = args[3];
                DateTime.TryParseExact(args[4], "yyyy-MM-dd",
                   System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dateFrom);
                DateTime.TryParseExact(args[5], "yyyy-MM-dd",
                   System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dateTo);
                reportLocation = args[6];
                ApplicationConfiguration.Initialize();
                PerformAction();
                return;

                //var form1 = new Form1();
                //form1.SetPerformAction(PerformAction);
                //Application.Run(form1);


                bool PerformAction(WebView2? webView = null)
                {
                    try
                    {
                        WebViewReportDownloader? webViewReportDownloader = null;
                        switch (reportType)
                        {
                            case ExternalReportType.Bolt:
                                webViewReportDownloader = new BoltReportDownloader(
                                    companyCode, username, password, dateFrom, dateTo, reportLocation);
                                break;
                            case ExternalReportType.Uber:
                                webViewReportDownloader = new UberReportDownloader(
                                    companyCode, username, password, dateFrom, dateTo, reportLocation);
                                break;
                            case ExternalReportType.Freenow:
                                webViewReportDownloader = new FreenowReportDownloader(
                                    username, password, dateFrom, dateTo, reportLocation);
                                break;
                        }
                        if (webViewReportDownloader == null)
                            throw new Exception("");
                        using (webViewReportDownloader)
                        {
#pragma warning disable CS4014
                            webViewReportDownloader.Execute(webView);
#pragma warning restore CS4014
                            while (!webViewReportDownloader.Performed)
                            {
                                Application.DoEvents();
                                Thread.Sleep(1000);
                            }
                            if (!string.IsNullOrWhiteSpace(webViewReportDownloader.ErrorMessage))
                                throw new Exception(webViewReportDownloader.ErrorMessage);
                        }
                        Console.WriteLine("OK");
                        return true;
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine($"ERR:{ex.Message}");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERR:{ex.Message}");
            }
        }

        private static ExternalReportType StringToExternalReportType(string inputValue)
        {
            switch (inputValue)
            {
                case "1":
                    return ExternalReportType.Bolt;
                case "2":
                    return ExternalReportType.Uber;
                case "3":
                    return ExternalReportType.Freenow;
                default:
                    return ExternalReportType.Unknown;
            }
        }
    }
}