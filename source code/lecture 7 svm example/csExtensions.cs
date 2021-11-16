using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lecture_6_svm_example
{
    public static class csExtensions
    {
        public enum enLowerCase
        {
            enUS,
            trTR
        }

        public static string normalizeText(this string srGG)
        {
            if (MainWindow.blEnableLowerCase == true)
            {
                srGG = srGG.lowerCaseText();
            }
            return srGG;
        }

        private static string lowerCaseText(this string srText)
        {
            switch (MainWindow.caseEnum)
            {
                case enLowerCase.enUS:
                    return srText.ToLower(new System.Globalization.CultureInfo("en-US"));
                    break;
                case enLowerCase.trTR:
                    return srText.ToLower(new System.Globalization.CultureInfo("tr-TR"));
                    break;
                default:
                    return srText.ToLower(new System.Globalization.CultureInfo("en-US"));
                    break;  
            }       
        }

    }
}
