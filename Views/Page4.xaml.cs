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
            FillGrdRef();
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
            PopupAddRef.IsOpen = PopupAddRef.IsOpen == true ? false : true;
            if (refID > 0 && refID != null)
            {
                FillRef((int)refID);
            }
            else
            {
                TxtbxCompanyRef.Text = TxtbxCompany.Text;
            }
        }

        private async Task<int?> Save_Ref()
        {
            //return 1;
            //int tmp = 0;
            //if (TxtbxCompany.Text.Length > 0)
            //{
            //    tmp = await SaveCompany(TxtbxCompany.Text);
            //}
            using (var db = new CVDBContext())
            {
                int? tmp = null;
                Employment_Histories e = db.Employment_Histories.Find(emp_hisID);
                if (e != null)
                {
                    tmp = e.REF_ID;
                }
                if (tmp < 1 || tmp == null)
                {
                    if (TxtbxCompanyRef.Text.Length > 0 &&
                        TxtbxEmailRef.Text.Length > 0 &&
                        TxtbxFirstNameRef.Text.Length > 0 &&
                        TxtbxLastNameRef.Text.Length > 0 &&
                        TxtbxMobileRef.Text.Length > 0 &&
                        TxtbxPhoneRef.Text.Length > 0 &&
                        TxtbxTitleRef.Text.Length > 0)
                    {
                        return null;
                    }
                    else
                    {
                        User_References ur = new User_References
                        {
                            Company_ID = await SaveCompany(TxtbxCompany.Text),
                            First_Name = TxtbxFirstNameRef.Text,
                            Last_Name = TxtbxLastNameRef.Text,
                            Title = TxtbxTitleRef.Text,
                            Phone = TxtbxPhoneRef.Text,
                            Mobile = TxtbxMobileRef.Text,
                            Email = TxtbxEmailRef.Text
                        };
                        db.User_References.Add(ur);
                        db.Entry(ur).State = EntityState.Added;
                        await db.SaveChangesAsync();
                        PopupAddRef.IsOpen = false;
                        return ur.REF_ID;
                    }
                }
                User_References u_r = db.User_References.Find(refID);
                db.Entry(u_r).State = EntityState.Modified;
                db.SaveChanges();

                return u_r.REF_ID;
            }
        }

        private int GetRefID(string str)
        {
            using (var db = new CVDBContext())
            {
                var query = from r in db.User_References
                            where r.Email == str
                            select r;
                return query.FirstOrDefault().REF_ID;
            }
        }

        private void BtnAddRef_Click(object sender, RoutedEventArgs e)
        {
            //Save_Ref();
        }

        int coID;
        private async Task<int> SaveCompany(string str)
        {
            using (var db = new CVDBContext())
            {
                Companies co = db.Companies.Find(GetCompanyID(str));

                if (co == null)
                {
                    co = new Companies
                    {
                        Company_Name = TxtbxCompanyRef.Text
                    };
                    db.Companies.Add(co);
                    db.Entry(co).State = EntityState.Added;
                    await db.SaveChangesAsync();
                    coID = GetCompanyID(co.Company_Name);
                    return coID;
                }
                else
                {
                    coID = co.Company_ID;
                    return coID;
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
                            where exp.User_ID == uid
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
            TxtbxCompanyRef.Text = "";
            TxtbxEmailRef.Text = "";
            TxtbxFirstNameRef.Text = "";
            TxtbxLastNameRef.Text = "";
            TxtbxMobileRef.Text = "";
            TxtbxPhoneRef.Text = "";
            TxtbxTitleRef.Text = "";
        }

        private void TxtbxEmailRef_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TxtbxEmailRef.Text.EmailWithAt())
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

        private async void BtnSaveExp_Click(object sender, RoutedEventArgs e)
        {
            coID = await SaveCompany(TxtbxCompany.Text);
            refID = await Save_Ref();
            using (var db = new CVDBContext())
            {
                if (emp_hisID < 1)
                {
                    Employment_Histories eh = new Employment_Histories
                    {
                        Company_ID = coID,
                        User_ID = uid,
                        Department = TxtbxDepartment.Text,
                        Position = TxtbxPosition.Text,
                        Title = TxtbxTitle.Text,
                        From_Date = (DateTime)DPFrom.SelectedDate,
                        Until_Date = (DateTime?)DPUntil.SelectedDate,
                        Curr_Emp = (bool)ChkbxCurrentEMP.IsChecked ? true : false,
                        REF_ID = refID,
                        Notes = TxtbxNotes.Text
                    };
                    db.Employment_Histories.Add(eh);
                    db.Entry(eh).State = EntityState.Added;
                    db.SaveChanges();
                }
                else
                {
                    Employment_Histories eh = db.Employment_Histories.Find(emp_hisID);
                    if (eh.REF_ID < 1 || eh.REF_ID == null)
                    {
                        int? tmp = await Save_Ref();
                        eh.REF_ID = tmp;
                    }
                    eh.Company_ID = coID;
                    db.Entry(eh).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            ClearFields();
            FillGrdRef();
        }


        int? refID = null;
        int emp_hisID = 0;
        private void GrdEmpHis_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BtnDeleteExp.IsEnabled = false;
            if (GrdEmpHis.SelectedIndex >= 0)
            {
                var obj = GrdEmpHis.SelectedItem;
                System.Type type = obj.GetType();
                emp_hisID = (int)type.GetProperty("EMP_HIS_ID").GetValue(obj, null);
                using (var db = new CVDBContext())
                {
                    Employment_Histories eh = db.Employment_Histories.Find(emp_hisID);
                    TxtbxDepartment.Text = eh.Department;
                    TxtbxNotes.Text = eh.Notes;
                    TxtbxPosition.Text = eh.Position;
                    TxtbxTitle.Text = eh.Title;
                    DPFrom.SelectedDate = eh.From_Date;
                    DPUntil.SelectedDate = eh.Until_Date;
                    if (eh.Curr_Emp)
                        ChkbxCurrentEMP.IsChecked = true;
                    if (eh.REF_ID != null)
                        FillRef((int)eh.REF_ID);
                    if (eh.Company_ID != null)
                        TxtbxCompany.Text = GetCompanyName((int)eh.Company_ID);
                }
                BtnDeleteExp.IsEnabled = true;
            }
        }

        private void BtnDeleteExp_Click(object sender, RoutedEventArgs e)
        {
            ClearFields();
            using (var db = new CVDBContext())
            {
                var query = from exp in db.Employment_Histories
                            where exp.EMP_HIS_ID == emp_hisID
                            select exp;
                Employment_Histories ehToDelete = query.FirstOrDefault();
                db.Employment_Histories.Remove(ehToDelete);
                db.SaveChanges();
            }
            FillGrdRef();
        }
    }

}

