using DriverSettlements.Common.Helpers;
using DriverSettlements.Common.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.ObjectPool;
using System.Data;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace DriverSettlements.WebAPI.Helpers
{
    public class ExternalReportParser
    {
        ExternalReportBody? _reportBody;

        public bool Parse(string reportFile, ExternalReportType reportType)
        {
            switch(reportType)
            {
                case ExternalReportType.Bolt:
                    _reportBody = ParseBoltReportByEarningsReport(reportFile);
                    return true;
                case ExternalReportType.Uber:
                    _reportBody = ParseUberReport(reportFile);
                    return true;
                case ExternalReportType.Freenow:
                    _reportBody = ParseFreenowReport(reportFile);
                    return true;
                default:
                    return false;
            }
        }

        public string Serialize()
        {
            if (_reportBody == null)
                return string.Empty;
            return ExternalReportBody.Serialize(_reportBody);
        }

        private static ExternalReportBody? ParseBoltReportByEarningsReport(string reportFile)
        {
            /*
"Kierowca","Telefon","E-mail","Zarobki brutto|ZŁ","Zarobki brutto z płatności w aplikacji|ZŁ","Zarobki brutto gotówkowe|ZŁ","Gotówka na stanie|ZŁ","Napiwki|ZŁ","Bonusy|ZŁ","Rekompensaty|ZŁ","Opłata za anulowanie|ZŁ","Opłaty drogowe|ZŁ","Opłata rezerwacyjna|ZŁ","Koszty ogółem|ZŁ","Koszty związane z prowizją|ZŁ","Zwroty dla pasażerów|ZŁ","Inne koszty|ZŁ","Zarobki netto|ZŁ","Wypłaty|ZŁ","Zarobki na godzinę (brutto)|ZŁ","Zarobki na godzinę (netto)|ZŁ","Unikalny identyfikator kierowcy","Kod osobisty kierowcy"
            */
            if (string.IsNullOrWhiteSpace(reportFile) || !File.Exists(reportFile))
                return null;
            var lines = File.ReadAllLines(reportFile);
            if (lines.Length < 2)
                return null;
            var reportBody = new ExternalReportBody();
            reportBody.ColumnNames = lines.First().Split(',').Select(i=>i.Substring(1,i.Length-2)).ToArray();
            var items = new List<object[]>();
            foreach (var line in lines.Skip(1))
            {
                var tab = line.Split(',').Select(i => i.Substring(1, i.Length - 2)).ToArray();
                var values = new List<object>();
                values.AddRange(tab.Take(3));
                values.AddRange(tab.Skip(3).Take(tab.Length-5).Select(v => double.Parse(v, CultureInfo.InvariantCulture) as object));
                items.Add(values.ToArray());
            }
            reportBody.Values = items.ToArray();
            return reportBody;
        }

        private static ExternalReportBody? ParseBoltReportByWeeklyReport(string reportFile)
        {
            /*
"Kierowca","Telefon","Okres","Wartość brutto","Opłata za anulowanie","Opłata rezerwacyjna (płatność)","Opłata rezerwacyjna (potrącenie)","Opłata drogowa","Opłata Bolt","Przejazdy gotówkowe (przyjęta gotówka)","Zniżki gotówkowe zrekompensowane przez Bolt ","Bonus","Rekompensaty","Zwroty","Napiwek","Tygodniowy bilans"
            
                dataTable.Columns[0].ColumnName
                "Kierowca"
                dataTable.Columns[9].ColumnName
                "Przejazdy gotówkowe (przyjęta gotówka)" (-1)
                dataTable.Columns[15].ColumnName
                "Tygodniowy bilans"
                dataTable.Columns[3].ColumnName
                "Wartość brutto"

                DriverName = values[0],
                ReceivedCash = -1.0 * double.Parse(values[9].Replace(',', '.'), CultureInfo.InvariantCulture),
                CompanyAccount = double.Parse(values[15].Replace(',', '.'), CultureInfo.InvariantCulture),
                GrossValue = double.Parse(values[3].Replace(',', '.'), CultureInfo.InvariantCulture)
            */

            if (string.IsNullOrWhiteSpace(reportFile) || !File.Exists(reportFile))
                return null;
            var lines = File.ReadAllLines(reportFile);
            if (lines.Length < 3)
                return null;
            var subReportsLines = Common.Helpers.Strings.SplitLines(lines, ",,,,,,,,,,,,,,,");
            if (subReportsLines == null)
                return null;                        
            var reportBody = ParseBoltReportFromSubReportLines(subReportsLines.FirstOrDefault());
            if (reportBody==null)
                return null;
            ParseBoltSubReportFromSubReportLines(reportBody, subReportsLines.Skip(1).ToArray());
            return reportBody;
        }



        private static ExternalReportBody? ParseBoltReportFromSubReportLines(string[]? lines)
        {
            if (lines == null)
                return null;
            var reportBody = new ExternalReportBody();
            reportBody.ColumnNames = lines.First().Split(',').Select(c => c.Substring(1, c.Length - 2)).ToArray();
            var items = new List<object[]>();

            foreach (var line in lines.Skip(2))
            {
                if (line.StartsWith(",,,,,,,,,,,,,,,"))
                {
                    reportBody.Values = items.ToArray();
                    return reportBody;
                }
                var correctedLine = line.Replace("\",\"", "\";\"").Replace(',', '.').Replace("\";\"", "\",\"");
                var tab = correctedLine.Split(',').Select(v => v.Substring(1, v.Length - 2)).ToArray();
                List<object> values = new List<object>();
                values.AddRange(tab.Take(3));
                values.AddRange(tab.Skip(3).Select(v => (string.IsNullOrWhiteSpace(v) ? 0 : double.Parse(v.Replace(',', '.'), CultureInfo.InvariantCulture) as object)));
                items.Add(values.ToArray());
            }
            reportBody.Values = items.ToArray();
            return reportBody;
        }

        private static void ParseBoltSubReportFromSubReportLines(ExternalReportBody reportBody, string[][] subLines)
        {
            if (subLines.Length<2)
                return;
            var subReportsLines = new List<string[]>();
            for (int i = 1; i < subLines.Length; i=i+2)
                subReportsLines.Add(subLines[i]);
            if (subReportsLines.Count < 1)
                return;
            var columnNames = subReportsLines.First().First().Split(',').Select(c => c.Substring(1, c.Length - 2)).ToList();
            var checkColumnNames = new string[] { "Imię kierowcy", "Telefon", "Data", "Płatność potwierdzona", "Odbiór", "Metoda płatności", "Zamówione", "Cena przejazdu", "Opłata rezerwacyjna", "Opłata drogowa", "Opłata za anulowanie", "Napiwek", "Status zamówienia", "Model samochodu", "Numer samochodu" };
            var missingColumnNames = checkColumnNames.Where(cn => !columnNames.Contains(cn)).ToArray();
            if (missingColumnNames.Length > 0)
                throw new Exception($"Missing column names {string.Join(", ",missingColumnNames)}");
            var kierowcaIdx = columnNames.IndexOf("Imię kierowcy");
            var telefonIdx = columnNames.IndexOf("Telefon");
            var data1Idx = columnNames.IndexOf("Data");
            var data2Idx = columnNames.IndexOf("Płatność potwierdzona");
            var odbiorIdx = columnNames.IndexOf("Odbiór");
            var metodaPlatnosciIdx = columnNames.IndexOf("Metoda płatności");
            var statusZamowieniaIdx = columnNames.IndexOf("Status zamówienia");
            var modelSamochoduIdx = columnNames.IndexOf("Model samochodu");
            var numerSamochoduIdx = columnNames.IndexOf("Numer samochodu");
            var cenaPrzejazduIdx = columnNames.IndexOf("Cena przejazdu");
            var oplataRezerwacyjnaIdx = columnNames.IndexOf("Opłata rezerwacyjna");
            var oplataDrogowaIdx = columnNames.IndexOf("Opłata drogowa");
            var oplataZaAnulowanieIdx = columnNames.IndexOf("Opłata za anulowanie");
            var napiwekIdx = columnNames.IndexOf("Napiwek");

            reportBody.SubReportColumnNames = new string[] {
                "Kierowca","Telefon","Data",
                "Odbiór","Metoda płatności","Status zamówienia","Model samochodu","Numer samochodu", 
                "Cena przejazdu","Opłata rezerwacyjna",
                "Opłata drogowa","Opłata za anulowanie",
                "Napiwek"
            };
            var items = new List<object[]>();
            foreach (var subReportLines in subReportsLines)
            {
                foreach(var subReportLine in subReportLines.Skip(1))
                {
                    var correctedLine = subReportLine.Replace("\",\"", "\";\"").Replace(',', '.').Replace("\";\"", "\",\"");
                    var tab = correctedLine.Split(',').Select(v => v.Substring(1, v.Length - 2)).ToArray();
                    List<object> values = new List<object>();
                    items.Add(new object[] {
                        tab[kierowcaIdx],tab[telefonIdx], DateTime.ParseExact($"{tab[data1Idx]} {tab[data2Idx]}","yyyy-MM-dd HH:mm",CultureInfo.InvariantCulture),
                        tab[odbiorIdx],tab[metodaPlatnosciIdx],tab[statusZamowieniaIdx],tab[modelSamochoduIdx],tab[numerSamochoduIdx],
                        double.Parse(tab[cenaPrzejazduIdx].Replace(',', '.'),CultureInfo.InvariantCulture),double.Parse(tab[oplataRezerwacyjnaIdx].Replace(',', '.'),CultureInfo.InvariantCulture),
                        double.Parse(tab[oplataDrogowaIdx].Replace(',', '.'),CultureInfo.InvariantCulture),double.Parse(tab[oplataZaAnulowanieIdx].Replace(',', '.'),CultureInfo.InvariantCulture),
                        double.Parse(tab[napiwekIdx].Replace(',', '.'),CultureInfo.InvariantCulture)
                    }); 
                }
            }
            reportBody.SubReportValues = items.ToArray();
        }

        private static ExternalReportBody? ParseUberReport(string reportFile)
        {
            /*
 Identyfikator UUID kierowcy,Imię kierowcy,Nazwisko kierowcy,Wypłacono Ci,Wypłacono Ci : Twój przychód,Wypłacono Ci : Bilans przejazdu : Wypłaty : Odebrana gotówka,Wypłacono Ci : Twój przychód : Opłata,Wypłacono Ci : Twój przychód : Podatki,Wypłacono Ci:Twój przychód:Opłata:Opłata,Wypłacono Ci:Twój przychód:Opłata:Podatek od opłaty,Wypłacono Ci:Twój przychód:Opłata:Czas oczekiwania w miejscu odbioru,Wypłacono Ci:Twój przychód:Opłata za usługę,Wypłacono Ci:Twój przychód:Podatki:Podatek od opłaty za usługę,Wypłacono Ci:Twój przychód:Opłata:Mnożnik,Wypłacono Ci:Twój przychód:Promocja:Program Quest,Wypłacono Ci:Twój przychód:Podatki:Podatek,Wypłacono Ci:Twój przychód:Napiwek,Wypłacono Ci:Twój przychód:Opłata:Odwołanie,Wypłacono Ci:Twój przychód:Opłata:Dopłata Uber Pets,Wypłacono Ci:Bilans przejazdu:Wypłaty:Przelano na konto bankowe,Wypłacono Ci:Twój przychód:Inne przychody:Korekta,Wypłacono Ci:Korekty naliczanie po przejeździe,Wypłacono Ci:Twój przychód:Promocja:Nagroda za polecenie,Wypłacono Ci:Bilans przejazdu:Podatki:Podatek od opłaty,Wypłacono Ci:Twój przychód:Promocja:Promocja

                dataTable.Columns[1].ColumnName
                "Imię kierowcy"
                dataTable.Columns[2].ColumnName
                "Nazwisko kierowcy"
                dataTable.Columns[5].ColumnName (-1)
                "Wypłacono Ci : Bilans przejazdu : Wypłaty : Odebrana gotówka"
                dataTable.Columns[3].ColumnName
                "Wypłacono Ci"
                dataTable.Columns[4].ColumnName
                "Wypłacono Ci : Twój przychód"

                DriverName - column 1+2 (B+C)
                ReceivedCash - column 5 (F) 
                WeeklyBalance - column 3 (D)
                GrossValue - column 4 (E)
            */

            if (string.IsNullOrWhiteSpace(reportFile) || !File.Exists(reportFile))
                return null;
            var lines = File.ReadAllLines(reportFile);
            if (lines.Length < 2)
                return null;
            var reportBody = new ExternalReportBody();
            reportBody.ColumnNames = lines.First().Split(',').ToArray();
            var items = new List<object[]>();
            foreach (var line in lines.Skip(1))
            {
                var tab = line.Split(',').ToArray();
                var values = new List<object>();
                values.AddRange(tab.Take(3));
                values.AddRange(tab.Skip(3).Select(v => (string.IsNullOrWhiteSpace(v) ? 0 : double.Parse(v.Replace(',', '.'), CultureInfo.InvariantCulture) as object)));
                items.Add(values.ToArray());
            }
            reportBody.Values = items.ToArray();
            return reportBody;
        }

        private static ExternalReportBody? ParseFreenowReport(string reportFile)
        {
            /*
Driver ID,Driver,Total trips,"Completed trips ","Paid by app ",Non-app payments,Trip adjustment,Incentives,Tips,Tolls,Cancellations,Waiting time,Congestion charge,Airport fee,Hotel fee,Others,Earnings before commission,Commission,Total earnings,FREENOW balance

                dataTable.Columns[1].ColumnName
                "Driver"
                dataTable.Columns[5].ColumnName
                "Non-app payments"
                dataTable.Columns[19].ColumnName
                "FREENOW balance"
                dataTable.Columns[16].ColumnName
                "Earnings before commission" 
 
                DriverName = values[1],
                ReceivedCash = double.Parse(values[5], CultureInfo.InvariantCulture),
                CompanyAccount = double.Parse(values[19], CultureInfo.InvariantCulture),
                GrossValue = double.Parse(values[16], CultureInfo.InvariantCulture)

            */
            if (string.IsNullOrWhiteSpace(reportFile) || !File.Exists(reportFile))
                return null;
            var directoryPath = Path.GetDirectoryName(reportFile);
            if (directoryPath == null)
                throw new Exception("");
            var subDirectoryInfo = new DirectoryInfo(Path.Combine(directoryPath, "temp"));
            if (subDirectoryInfo.Exists)
                subDirectoryInfo.Delete(true);
            subDirectoryInfo.Create();
            ZipFile.ExtractToDirectory(reportFile, subDirectoryInfo.FullName);
            //var fileInfo = subDirectoryInfo.GetFiles("*.csv").FirstOrDefault();
            var fileInfo = Files.GetFileInfoFromFreeNowTempFolder(subDirectoryInfo);
            if (fileInfo == null || !fileInfo.Exists)
                return null;
            var lines = File.ReadAllLines(fileInfo.FullName);
            GC.Collect();
            subDirectoryInfo.Delete(true);
            if (lines.Length < 2)
                return null;
            var reportBody = new ExternalReportBody();
            reportBody.ColumnNames = lines.First().Split(',').ToArray();
            var items = new List<object[]>();
            foreach (var line in lines.Skip(1))
            {
                var tab = line.Split(',').ToArray();
                var values = new List<object>();
                values.AddRange(tab.Take(2));
                values.AddRange(tab.Skip(2).Select(v => (string.IsNullOrWhiteSpace(v) ? 0 : double.Parse(v.Replace(',', '.'), CultureInfo.InvariantCulture) as object)));
                items.Add(values.ToArray());
            }
            reportBody.Values = items.ToArray();
            return reportBody;
        }
    }
}
