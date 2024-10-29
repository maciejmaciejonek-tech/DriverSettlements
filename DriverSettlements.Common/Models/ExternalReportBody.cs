using NCalc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DriverSettlements.Common.Models
{
    public class ExternalReportBody
    {
        private string _driverNameTemplate = string.Empty;
        public string[]? ColumnNames { get; set;}
        public object[][]? Values { get; set;}
        public string[]? SubReportColumnNames { get; set; }
        public object[][]? SubReportValues { get; set; }

        private string _driverNameFormatString = string.Empty;
        private Dictionary<string, string> _valuesTemplates = new Dictionary<string, string>();
        public string DriverNameTemplate
        {
            get => _driverNameTemplate;
            set
            {
                if (ColumnNames == null)
                    return;
                _driverNameTemplate = value;
                _driverNameFormatString = _driverNameTemplate.ToLower();
                for(int i = 0; i<ColumnNames.Length; i++)
                {
                    var columnName = ColumnNames[i].ToLower();
                    _driverNameFormatString = _driverNameFormatString.Replace($"{{{columnName}}}", $"{{{i}}}");
                }
            }
        }

        private Dictionary<string, string> _valuesFormatStrings = new Dictionary<string, string>();

        public Dictionary<string, string> ValuesTemplates
        {
            get => _valuesTemplates;
            set
            {
                _valuesTemplates = value;
                _valuesFormatStrings = new Dictionary<string, string>();
                if (_valuesTemplates == null || _valuesTemplates.Count == 0 || ColumnNames==null)
                    return;
                foreach (var valueTemplateKvp in _valuesTemplates)
                {
                    var valueFormatString = valueTemplateKvp.Value.ToLower();
                    for (int i = 0; i < ColumnNames.Length; i++)
                    {
                        var columnName = ColumnNames[i].ToLower();
                        valueFormatString = valueFormatString.Replace($"{{{columnName}}}", $"{{{i}}}");
                    }
                    _valuesFormatStrings.Add(valueTemplateKvp.Key, valueFormatString);
                }
            }
        }

        public static string Serialize(ExternalReportBody externalReportBody)
        {
            return JsonSerializer.Serialize<ExternalReportBody>(externalReportBody);
        }

        public static ExternalReportBody? Deserialize(string? externalReportBodyJson)
        {
            if (externalReportBodyJson == null)
                return null;
            return JsonSerializer.Deserialize<ExternalReportBody>(externalReportBodyJson);
        }

        public HashSet<string>? GetDrivers()
        {
            if (Values == null)
                return null;
            if (string.IsNullOrWhiteSpace(DriverNameTemplate) || ColumnNames == null)
                return null;
            var res = new HashSet<string>();
            foreach (var values in Values)
                res.Add(string.Format(_driverNameFormatString, values).ToLower());
            return res;
        }

        public Dictionary<string, List<double>>? GetValues()
        {
            if (Values == null)
                return null;
            if (string.IsNullOrWhiteSpace(DriverNameTemplate) || ColumnNames==null)
                return null;
            var res = new Dictionary<string, List<double>>();
            foreach(var values in Values)
            {
                var driverName = string.Format(_driverNameFormatString, values).ToLower();
                var list = new List<double>();
                foreach (var valueFormatStringKvp in _valuesFormatStrings) 
                {
                    var expression = new Expression(string.Format(valueFormatStringKvp.Value, values));
                    list.Add(Convert.ToDouble(expression.Evaluate()));
                }
                if (res.TryGetValue(driverName, out var existingList))
                {
                    for (int i = 0; i < existingList.Count; i++)
                        existingList[i] += list[i];
                }
                else
                    res.Add(driverName, list);
            }
            return res;
        }

        public Dictionary<string,List<double>>? GetSubReportValues()
        {
            if (Values == null)
                return null;
            if (string.IsNullOrWhiteSpace(DriverNameTemplate) || ColumnNames == null)
                return null;
            var res = new Dictionary<string, List<double>>();
            foreach (var values in Values)
            {
                var driverName = string.Format(_driverNameFormatString, values).ToLower();
                var list = new List<double>();
                foreach (var valueFormatStringKvp in _valuesFormatStrings)
                {
                    var expression = new Expression(string.Format(valueFormatStringKvp.Value, values));
                    list.Add(Convert.ToDouble(expression.Evaluate()));
                }
                if (res.TryGetValue(driverName, out var existingList))
                {
                    for (int i = 0; i < existingList.Count; i++)
                        existingList[i] += list[i];
                }
                else
                    res.Add(driverName, list);
            }
            return res;
        }
    }
}
