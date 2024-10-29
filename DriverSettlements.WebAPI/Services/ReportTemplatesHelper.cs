using DriverSettlements.Common.Models;
using DriverSettlements.Database;
using Microsoft.EntityFrameworkCore;

namespace DriverSettlements.WebAPI.Services
{
    public class ReportTemplatesHelper
    {
        public static DefReport GetReportTemplate(DatabaseContext dbContext, string templateName, DateTime date)
        {
            var defReportItem = dbContext.DefReports.Include(r => r.DefReportColumns).FirstOrDefault(r => r.ReportName.Equals(templateName) && r.DateFrom <= date && (!r.DateTo.HasValue || r.DateTo > date));
            if (defReportItem == null)
                throw new Exception($"Report template [{templateName}] does not exist");
            return defReportItem;
        }

        private static DefReportColumn GetColumnTemplate(DatabaseContext dbContext, string reportTemplateName, string columnTemplateName, DateTime date)
        {
            var defReportItem = GetReportTemplate(dbContext, reportTemplateName, date);
            var columnTemplate = defReportItem?.DefReportColumns?.FirstOrDefault(c => c.ColumnName.Equals(columnTemplateName));
            if (columnTemplate == null)
                throw new Exception($"Columnt template [{columnTemplateName}] does not exist");
            return columnTemplate;                
        }

        public static void SetReportBodyGlobalTemplates(DatabaseContext dbContext, ExternalReportBody reportBody, ExternalReportType reportType, DateTime date)
        {
            if (reportBody == null) throw new ArgumentNullException("reportBody");
            var driverNameTemplate = GetColumnTemplate(dbContext, "Global templates", "DriverName", date);
            switch (reportType)
            {
                case ExternalReportType.Bolt:
                    if (driverNameTemplate.DefBoltValue != null)
                        reportBody.DriverNameTemplate = driverNameTemplate.DefBoltValue;
                    break;
                case ExternalReportType.Uber:
                    if (driverNameTemplate.DefUberValue != null)
                        reportBody.DriverNameTemplate = driverNameTemplate.DefUberValue;
                    break;
                case ExternalReportType.Freenow:
                    if (driverNameTemplate.DefFreenowValue != null)
                        reportBody.DriverNameTemplate = driverNameTemplate.DefFreenowValue;
                    break;
            }
        }

        public static void SetReportBodyBasicReportTemplate(DatabaseContext dbContext, ExternalReportBody reportBody, ExternalReportType reportType, DateTime date)
        {
            if (reportBody == null) throw new ArgumentNullException("reportBody");
            var basicReportTemplate = GetReportTemplate(dbContext, "Basic report", date);
            if (basicReportTemplate.DefReportColumns == null)
                throw new Exception("Missing basic report columns definitions");
            var valuesTemplates = new Dictionary<string, string>();
            foreach (var defReportColumn in basicReportTemplate.DefReportColumns)
            {
                if (!string.IsNullOrWhiteSpace(defReportColumn.DefValue)) continue;
                switch (reportType)
                {
                    case ExternalReportType.Bolt:
                        if (defReportColumn.DefBoltValue != null)
                            valuesTemplates.Add(defReportColumn.ColumnName, defReportColumn.DefBoltValue);
                        break;
                    case ExternalReportType.Uber:
                        if (defReportColumn.DefUberValue != null)
                            valuesTemplates.Add(defReportColumn.ColumnName, defReportColumn.DefUberValue);
                        break;
                    case ExternalReportType.Freenow:
                        if (defReportColumn.DefFreenowValue != null)
                            valuesTemplates.Add(defReportColumn.ColumnName, defReportColumn.DefFreenowValue);
                        break;
                }
            }
            reportBody.ValuesTemplates = valuesTemplates;
        }
    }
}
