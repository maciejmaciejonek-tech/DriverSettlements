using DriverSettlements.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverSettlements.App.Helpers
{
    public class Translate
    {
        public static SettlementType TranslateSettlementType(SettlementType settlementType)
        {
            var translatedText = Resources.Localization.ApplicationStrings.ResourceManager.GetString($"SettlementType.{settlementType.TypeName}");
            if (!string.IsNullOrWhiteSpace(translatedText))
                settlementType.TypeName = translatedText;
            return settlementType;
        }

        public static string TranslateSettlementType(string settlementType)
        {
            var translatedText = Resources.Localization.ApplicationStrings.ResourceManager.GetString($"SettlementType.{settlementType}");
            if (!string.IsNullOrWhiteSpace(translatedText))
                return translatedText;
            return settlementType;
        }

        public static SettlementItem TranslateSettlementItem(SettlementItem settlementItem)
        {
            var translatedText = Resources.Localization.ApplicationStrings.ResourceManager.GetString($"SettlementType.{settlementItem.SettlementType.TypeName}");
            if (!string.IsNullOrWhiteSpace(translatedText))
                settlementItem.SettlementType.TypeName = translatedText;
            return settlementItem;
        }

    }
}
