using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using XBAPLexiconCVDBInterface.Extentionmethods;

namespace XBAPLexiconCVDBInterface.Views
{
    /// <summary>
    /// Interaction logic for Page4.xaml
    /// </summary>
    public partial class Page4 : Page
    {
        int uid;

        public Page4(int id)
        {
            InitializeComponent();
            uid = id;
        }

        private void BtnPage5_Click(object sender, RoutedEventArgs e)
        {
            // Find the frame.
            Frame pageFrame = null;
            DependencyObject currParent = VisualTreeHelper.GetParent(this);
            while (currParent != null && pageFrame == null)
            {
                pageFrame = currParent as Frame;
                currParent = VisualTreeHelper.GetParent(currParent);
            }

            // Change the page of the frame.
            if (pageFrame != null)
            {
                Page5 p5 = new Page5(uid);
                pageFrame.Navigate(p5);
                //pageFrame.Source = new Uri("Page5.xaml", UriKind.Relative);
            }
        }

        private void BtnPage3_Click(object sender, RoutedEventArgs e)
        {
            // Find the frame.
            Frame pageFrame = null;
            DependencyObject currParent = VisualTreeHelper.GetParent(this);
            while (currParent != null && pageFrame == null)
            {
                pageFrame = currParent as Frame;
                currParent = VisualTreeHelper.GetParent(currParent);
            }

            // Change the page of the frame.
            if (pageFrame != null)
            {
                Page3 p3 = new Page3(uid);
                pageFrame.Navigate(p3);
                //pageFrame.Source = new Uri("Page3.xaml", UriKind.Relative);
            }
        }


        private void BtnReference_Click(object sender, RoutedEventArgs e)
        {
            ClearFields();
            PopupAddRef.IsOpen = PopupAddRef.IsOpen == true ? false : true;
            FillRef(refID);
        }

        private void BtnAddRef_Click(object sender, RoutedEventArgs e)
        {
            if (TxtbxCompanyRef.Text.Length > 0)
            {
                SaveCompany(TxtbxCompanyRef.Text);
            }
            using (var db = new CVDBContext())
            {
                User_References ur = new User_References
                {
                    Company_ID = coID,
                    First_Name = TxtbxFirstNameRef.Text,
                    Last_Name = TxtbxLastNameRef.Text,
                    Title = TxtbxTitleRef.Text,
                    Phone = TxtbxPhoneRef.Text,
                    Mobile = TxtbxMobileRef.Text,
                    Email = TxtbxEmailRef.Text
                };
                db.User_References.Add(ur);
                db.Entry(ur).State = EntityState.Added;
                db.SaveChanges();
            }
            PopupAddRef.IsOpen = false;
        }

        int coID;
        private async void SaveCompany(string str)
        {
            using (var db = new CVDBContext())
            {
                Companies co = db.Companies.Find(GetCompanyID(str));
                //var query = from co in db.Companies
                //            where co.Company_Name == str
                //            select co;
                if (co == null)
                //if (query.Count() < 1)
                {
                    co = new Companies
                    {
                        Company_Name = TxtbxCompanyRef.Text
                    };
                    db.Companies.Add(co);
                    db.Entry(co).State = EntityState.Added;
                    await db.SaveChangesAsync();
                    coID = GetCompanyID(co.Company_Name);
                }
                else
                {
                    coID = co.Company_ID;
                }
            }
        }

        private int GetCompanyID(string str)
        {
            using (var db = new CVDBContext())
            {
                var query = from comp in db.Companies
                            where comp.Company_Name == str
                            select comp;
                if (query.Count() == 1)
                {
                    return query.FirstOrDefault().Company_ID;
                }
                else return 0;
            }
        }

        private void FillGrdRef()
        {
            using (var db = new CVDBContext())
            {
                var query = from exp in db.Employment_Histories
                            join com in db.Companies on exp.Company_ID equals com.Company_ID
                            join refe in db.User_References on exp.REF_ID equals refe.REF_ID
                            select new { exp.EMP_HIS_ID, exp.From_Date, exp.Until_Date, com.Company_Name, exp.Position, refe.First_Name, refe.Last_Name };
                GrdEmpHis.ItemsSource = query.OrderBy(x => x.From_Date).ToList();
            }
        }

        private void FillRef(int id)
        {
            using (var db = new CVDBContext())
            {
                User_References ur = db.User_References.Find(id);
                TxtbxFirstNameRef.Text = ur.First_Name;
                TxtbxLastNameRef.Text = ur.Last_Name;
                TxtbxTitleRef.Text = ur.Title;
                if (ur.Company_ID != null)
                    TxtbxCompanyRef.Text = GetCompanyName((int)ur.Company_ID);
                TxtbxPhoneRef.Text = ur.Phone;
                TxtbxMobileRef.Text = ur.Mobile;
                TxtbxEmailRef.Text = ur.Email;
            }
        }

        private string GetCompanyName(int id)
        {
            using (var db = new CVDBContext())
            {
                Companies c = db.Companies.Find(id);
                return c.Company_Name;
            }
        }

        private void ClearFields()
        {
            TxtbxCompany.Text = "";
            TxtbxDepartment.Text = "";
            TxtbxNotes.Text = "";
            TxtbxPosition.Text = "";
            TxtbxTitle.Text = "";
        }

        private void TxtbxEmailRef_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(TxtbxEmailRef.Text.EmailWithAt())
            {
                TxtbxEmailRef.BorderBrush = Brushes.Green;
                BtnAddRef.IsEnabled = true;
            }
            else
            {
                TxtbxEmailRef.BorderBrush = Brushes.Red;
                BtnAddRef.IsEnabled = false;
            }
        }

        private void BtnSaveExp_Click(object sender, RoutedEventArgs e)
        {

        }

        int refID;
        int emp_hisID;
        private void GrdEmpHis_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

    }
}
