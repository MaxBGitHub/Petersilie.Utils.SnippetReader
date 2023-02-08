using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace Petersilie.Utils.SnippetReader.OCR
{
    internal static class CultureSettings
    {
        public static string GetCurrentLanguage()
        {
            return CultureInfo.CurrentCulture.ThreeLetterISOLanguageName.ToLower();
        }


        public static string GetUILanguage()
        {
            return CultureInfo.CurrentUICulture.ThreeLetterISOLanguageName.ToLower();
        }


        public static string GetLanguage(CultureInfo culture)
        {
            return culture.ThreeLetterISOLanguageName.ToLower();
        }
    }
}
