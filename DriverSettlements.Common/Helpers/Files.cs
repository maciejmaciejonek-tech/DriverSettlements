using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverSettlements.Common.Helpers
{
    public static class Files
    {
        public static FileInfo? GetFileInfoFromFreeNowTempFolder(DirectoryInfo tempDirectoryInfo)
        {
            var fileInfos = tempDirectoryInfo.GetFiles("*.csv");
            var vatWordFileInfos = fileInfos.Where(fi => fi.Name.ToLower().Contains("vat"));
            if (fileInfos.Length == 0)
                throw new Exception("Brak raportów FreeNow w pobranym archiwum zip");
            else if (vatWordFileInfos.Any())
            {
                var withVatWordFileInfo = vatWordFileInfos.FirstOrDefault(fi => fi.Name.ToLower().Contains("with_vat"));
                if (withVatWordFileInfo == null)
                    throw new Exception("Błąd pobrania zawartości raportu z FreeNow z powodu braku pliku z sufiksem 'with_VAT'");
                return withVatWordFileInfo;
            }
            else if (fileInfos.Length > 1)
                throw new Exception("Wiele raportów FreeNow w pobranym archiwum zip");
            else
                return fileInfos.FirstOrDefault();
        }
    }
}
