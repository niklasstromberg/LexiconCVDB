using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XBAPLexiconCVDBInterface.Extentionmethods
{
    public static class Extentionmethods
    {
        public static bool EmailWithAt(this string str)
        {
            if (str.Contains('@'))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool StringTooShort(this string str)
        {
            if(str.Length < 3)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool StingIsInt(this string str)
        {
            int i;
            if(Int32.TryParse(str, out i))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static int GetAdressId(this Adresses a)
        {
            return a.Adress_ID;
        }

        public static string GetFirst25(this string s)
        {
            if(s.Length < 30)
            {
                return s;
            }
            return s.Substring(0, 30);
        }

        public static string CapitalizeFirst(this string str)
        {
            if (str == null)
            {
                return "";
            }
            string tmp = str.Substring(0, 1);
            return str.Remove(0, 1).Insert(0, tmp.ToUpper());
        }

    }
}
