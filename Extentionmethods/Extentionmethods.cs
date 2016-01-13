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

        public static bool StringIsInt(this string str)
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

        public static string GetLexiconHandle(this Journals j)
        {
            using (var db = new CVDBContext())
            {
                var query = from lh in db.Lexicon_Handles
                            where lh.Lexicon_Handle_ID == j.Lexicon_Handle_ID
                            select lh;
                var obj = query.FirstOrDefault();
                return obj.First_Name.CapitalizeFirst() + " " + obj.Last_Name.CapitalizeFirst();
            }
        }

        public static string GetLexiconHandle(this Logs l)
        {
            using (var db = new CVDBContext())
            {
                var query = from lh in db.Lexicon_Handles
                            where lh.Lexicon_Handle_ID == l.Lexicon_Handle_ID
                            select lh;
                var obj = query.FirstOrDefault();
                return obj.First_Name.CapitalizeFirst() + " " + obj.Last_Name.CapitalizeFirst();
            }
        }

        public static int GetEduID(this Educations e)   
        {
            using (var db = new CVDBContext())
            {
                var query = from edu in db.Educations
                            where edu.Year == e.Year && edu.School == e.School && edu.Course == e.Course && edu.Degree == e.Degree && edu.Notes == e.Notes
                            select edu;
                if (query.Count() > 0)
                {
                    Educations newEdu = query.FirstOrDefault();
                    return newEdu.EDU_ID;
                }
                return 0;
            }
        }

    }
}
