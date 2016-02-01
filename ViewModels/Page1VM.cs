using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XBAPLexiconCVDBInterface.Views;
using XBAPLexiconCVDBInterface.Extentionmethods;
using System.Windows.Controls;

namespace XBAPLexiconCVDBInterface.ViewModels
{
    class Page1VM
    {
        public void FillJournals(int i)
        {
            using (var db = new CVDBContext())
            {
                var query = from j in db.Journals
                            where j.User_ID == i
                            select new { Journal_ID = j.Journal_ID, Created = j.Created, Notes = j.Notes };
                var tmp = query.ToList();
                Page1 Page1 = new Page1();
                Page1.DGJournals.ItemsSource = tmp;
                
            }
        }

        public void SavePage1(Users user, Adresses adress)
        {

        }
    }


}
