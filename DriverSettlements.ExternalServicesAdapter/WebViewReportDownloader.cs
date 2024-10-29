using Microsoft.Web.WebView2.Core;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using WebView2 = Microsoft.Web.WebView2.WinForms.WebView2;

namespace DriverSettlements.ExternalServicesAdapter
{
    public abstract class WebViewReportDownloader:IDisposable
    {
        public bool Performed = false;
        public string ErrorMessage = string.Empty;
        private WebView2? _webView;
        private readonly string _reportsUri;
        private readonly string _reportLocation;
        private string? _userDataFolder;
        private readonly bool _beforeLoginPageBehaviour;
        private readonly bool _afterLoginPageBehaviour;
        private readonly bool _setReportsParameters;

        protected WebViewReportDownloader(string reportsUri, string reportLocation, bool beforeLoginPageBehaviour = false, bool afterLoginPageBehaviour = false, bool setReportsParameters = false)
        {
            _reportsUri = reportsUri;
            _reportLocation = reportLocation;
            _beforeLoginPageBehaviour = beforeLoginPageBehaviour;
            _afterLoginPageBehaviour = afterLoginPageBehaviour;
            _setReportsParameters = setReportsParameters;
        }

        public async Task Execute(WebView2? webView = null)
        {
            if (Program.WebView2Configuration == null)
                throw new Exception("");
            try
            {
                if (Program.WebView2Configuration.UseFolders)
                    Environment.SetEnvironmentVariable("WEBVIEW2_BROWSER_EXECUTABLE_FOLDER", Program.WebView2Configuration.BrowserExecutableFolder);
                Environment.SetEnvironmentVariable("WEBVIEW2_ADDITIONAL_BROWSER_ARGUMENTS", "--disable-features=msSmartScreenProtection");
                Environment.SetEnvironmentVariable("WEBVIEW2_ADDITIONAL_BROWSER_ARGUMENTS", "--lang=en");
                Environment.SetEnvironmentVariable("WEBVIEW2_ADDITIONAL_BROWSER_ARGUMENTS", "--accept-lang=en");
//                Environment.SetEnvironmentVariable("WEBVIEW2_ADDITIONAL_BROWSER_ARGUMENTS", "--allow-run-as-system");
//                Environment.SetEnvironmentVariable("WEBVIEW2_ADDITIONAL_BROWSER_ARGUMENTS", "--enable-features=allow-run-as-system");

                _userDataFolder = Path.Combine(Program.WebView2Configuration.UserDataFolder, Guid.NewGuid().ToString());
                if (Directory.Exists(_userDataFolder)) {
                    Directory.Delete(_userDataFolder,true);
                }
                Directory.CreateDirectory(_userDataFolder);
                if (webView == null)
                {
                    _webView = new WebView2();
                    _webView.Size = new Size(1980, 1024);
                }
                else
                    _webView = webView;
                _webView.CoreWebView2InitializationCompleted += _webView_CoreWebView2InitializationCompleted;
                if (Program.WebView2Configuration.UseFolders)
                {
                    var options = new CoreWebView2EnvironmentOptions();
                    options.Language= "en";
                    var environment = await CoreWebView2Environment.CreateAsync(
                        Program.WebView2Configuration.BrowserExecutableFolder, _userDataFolder, options);
                    await _webView.EnsureCoreWebView2Async(environment);
                }
                else
                {
                    await _webView.EnsureCoreWebView2Async();
                }
                var maxEndTime = DateTime.Now.AddSeconds(240);
                while (!File.Exists(_reportLocation) && !Performed && DateTime.Compare(DateTime.Now,maxEndTime)<0)
                {
                    Application.DoEvents();
                    await Task.Delay(1000);
//                    Thread.Sleep(1000);
                }
                if (DateTime.Compare(DateTime.Now, maxEndTime) >= 0)
                    throw new Exception($"{this.GetType().Name}.ERR10");
                if (!CheckReportFile(_reportLocation))
                    throw new Exception($"{this.GetType().Name}.ERR9");
                Performed = true;
            }
            catch (Exception ex)
            {
                ErrorMessage = ExceptionToMessage(ex);
                Performed = true;
            }
        }

        private void CoreWebView2_ProcessFailed(object? sender, CoreWebView2ProcessFailedEventArgs e)
        {
            if (Performed) return;
            ErrorMessage = e.ProcessDescription;
            Performed = true;
        }

        protected abstract Task ExecuteLoginScripts(CoreWebView2 coreWebView2);
        protected abstract Task ExecuteSetReportsParameterScripts(CoreWebView2 coreWebView2);
        protected abstract bool IsLoginPage(string html);
        protected abstract bool IsPostLoginPage(string html);
        protected abstract bool IsRaportsPage(string html);
        protected abstract Task GetReportScripts(string html, CoreWebView2 coreWebView2);
        protected abstract bool CheckReportFile(string reportFilePath);


        private void _webView_CoreWebView2InitializationCompleted(object? sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            if (Performed) return;
            if (e.InitializationException!= null)
            {
                ErrorMessage = ExceptionToMessage(e.InitializationException);
                Performed = true;
                return;
            }
            try
            {
                if (_webView == null)
                    return;
                _webView.CoreWebView2.ProcessFailed += CoreWebView2_ProcessFailed;
                _webView.CoreWebView2InitializationCompleted -= _webView_CoreWebView2InitializationCompleted;
                _webView.CoreWebView2.DownloadStarting += CoreWebView2_DownloadStarting;
                _webView.CoreWebView2.NewWindowRequested += CoreWebView2_NewWindowRequested ;
                _webView.CoreWebView2.NavigationCompleted += CoreWebView2_NavigationCompleted;
                if (File.Exists(_reportLocation))
                    File.Delete(_reportLocation);
                _webView.CoreWebView2.Navigate(_reportsUri);
            }
            catch(Exception ex)
            {
                ErrorMessage = ExceptionToMessage(ex);
                Performed = true;
            }
        }

        private void CoreWebView2_NewWindowRequested(object? sender, CoreWebView2NewWindowRequestedEventArgs e)
        {
            if (Performed || _webView==null) return;
            _webView.CoreWebView2.Navigate(e.Uri);
            e.Handled = true;
        }

        protected async Task<string> GetHtml()
        {
            if (_webView == null)
                return string.Empty;
            return (await _webView.CoreWebView2.ExecuteScriptAsync("document.documentElement.outerHTML")).Replace("\\u003C", "<").Replace(@"\""", @"""");
        }

        private async void CoreWebView2_NavigationCompleted(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            if (Performed || _webView==null) return;
            try
            {
                int numberOfProbes = 10;
                var probeNumber = 0;
                string html;
                _webView.CoreWebView2.NavigationCompleted -= CoreWebView2_NavigationCompleted;
                if (_beforeLoginPageBehaviour)
                {
                    probeNumber = 0;
                    html = await GetHtml();
                    while ((string.IsNullOrWhiteSpace(html) || !IsLoginPage(html)) && probeNumber++ < numberOfProbes)
                    {
                        //Thread.Sleep(1000);
                        await Task.Delay(1000);
                        html = await GetHtml();
                    }
                    if (probeNumber > numberOfProbes)
                        throw new Exception($"{this.GetType().Name}.<ERR01>\r\n{html}");
                }
                await ExecuteLoginScripts(_webView.CoreWebView2);
                if (_afterLoginPageBehaviour)
                {
                    probeNumber = 0;
                    html = await GetHtml();
                    while ((string.IsNullOrWhiteSpace(html) || !IsPostLoginPage(html)) && probeNumber++ < numberOfProbes)
                    {
                        //Thread.Sleep(1000);
                        await Task.Delay(1000);
                        html = await GetHtml();
                    }
                    if (probeNumber > numberOfProbes)
                        throw new Exception($"{this.GetType().Name}.<ERR02>\r\n{html}");
                    _webView.CoreWebView2.Navigate(_reportsUri);
                }
                probeNumber = 0;
                html = await GetHtml();
                while ((string.IsNullOrWhiteSpace(html) || !IsRaportsPage(html)) && probeNumber++ < numberOfProbes)
                {
                    //Thread.Sleep(1000);
                    await Task.Delay(1000);
                    html = await GetHtml();
                }
                if (probeNumber>numberOfProbes)
                    throw new Exception($"{this.GetType().Name}.<ERR03A>\r\n{html}");
                if (_setReportsParameters)
                {
                    await ExecuteSetReportsParameterScripts(_webView.CoreWebView2);
                    probeNumber = 0;
                    html = await GetHtml();
                    while ((string.IsNullOrWhiteSpace(html) || !IsRaportsPage(html)) && probeNumber++ < numberOfProbes)
                    {
                        //Thread.Sleep(1000);
                        await Task.Delay(1000);
                        html = await GetHtml();
                    }
                    if (probeNumber > numberOfProbes)
                        throw new Exception($"{this.GetType().Name}.<ERR03B>\r\n{html}");
                }
                await GetReportScripts(html, _webView.CoreWebView2);
            }
            catch (Exception ex)
            {
                ErrorMessage = ExceptionToMessage(ex);
                Performed = true;
            }
        }

        private void CoreWebView2_DownloadStarting(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2DownloadStartingEventArgs e)
        {
            if (Performed || _webView == null) return;
            _webView.CoreWebView2.DownloadStarting -= CoreWebView2_DownloadStarting;
            e.ResultFilePath = _reportLocation;
        }

        public async void Dispose()
        {
            if (_webView != null && _webView.CoreWebView2!=null)
            {
                _webView.Dispose();
                _webView = null;
                GC.Collect();
            }
            try
            {
                if (Directory.Exists(_userDataFolder))
                    Directory.Delete(_userDataFolder, true);
            }
            catch
            {
                //Thread.Sleep(1000);
                await Task.Delay(1000);
                if (Directory.Exists(_userDataFolder))
                    Directory.Delete(_userDataFolder, true);
            }
        }

        private static string ExceptionToMessage(Exception ex)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"{ex.GetType().Name}: {ex.Message}");
            stringBuilder.AppendLine("---StackTrace---");
            stringBuilder.AppendLine(ex.StackTrace);
            stringBuilder.AppendLine("---Source---");
            stringBuilder.AppendLine(ex.Source);
            if (ex.InnerException != null)
            {
                stringBuilder.AppendLine("---Inner exception---");
                stringBuilder.AppendLine(ExceptionToMessage(ex.InnerException));
            }
            return stringBuilder.ToString();
        }
    }
}
