using DriverSettlements.Common.Helpers;
using DriverSettlements.Common.Models;
using System;
using System.Diagnostics;
using System.Text;

namespace DriverSettlements.WebAPI.Helpers
{
    public class ExternalServicesWrapper
    {
        private List<string> _receivedData = new List<string>();
        private List<string> _receivedErrorData = new List<string>();
        private readonly Process _process;

        //"BOLT" "57616" "evelcars@gmail.com" "Evelbolt123!" "2023-07-24" "2023-07-30" "C:\Temp\bolt.csv"
        public ExternalServicesWrapper(string fileName, ExternalReportType reportType, string? companyId, string? username, string? password, DateTime dateFrom, DateTime dateTo, string reportPath)
        {
            var startInfo = new ProcessStartInfo(fileName);
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.Arguments = $"\"{(int)reportType}\" \"{companyId}\" \"{username}\" \"{password}\" \"{dateFrom.ToString("yyyy-MM-dd")}\" \"{dateTo.ToString("yyyy-MM-dd")}\" \"{reportPath}\"";
            _process = new Process();
            _process.StartInfo = startInfo;
            _process.StartInfo.RedirectStandardOutput = true;
            _process.StartInfo.RedirectStandardError = true;
            _process.OutputDataReceived += _process_OutputDataReceived;
            _process.ErrorDataReceived += _process_ErrorDataReceived;
        }
        public async Task<bool> Execute()
        {
            _process.Start();
            _process.BeginOutputReadLine();
            _process.BeginErrorReadLine();
            await _process.WaitForExitAsync();
            if (_receivedData.Count == 1 && _receivedData.First().Equals("OK", StringComparison.InvariantCultureIgnoreCase))
                return true;
            return false;
        }

        public IList<string> ReceivedData => _receivedData;

        public IList<string> ReceivedErrorData => _receivedErrorData;   

        private void _process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null)
                return;
            _receivedErrorData.Add(e.Data); 
        }

        private void _process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null)
                return;
            _receivedData.Add(e.Data);   
        }
    }
}
