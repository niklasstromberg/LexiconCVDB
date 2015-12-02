using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
using System.Configuration;
using System.Data;
using Dapper;
using XBAPLexiconCVDBInterface;

namespace XBAPLexiconCVDBInterface
{
    /// <summary>
    /// Interaction logic for Page2.xaml
    /// </summary>
    /// 
    
    public partial class Page2 : Page
    {
        string conn = ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString;
        int uid;

        public Page2(int id)
        {
            InitializeComponent();
            uid = id;

            // Här behöver vi hämta informationen som ska in i fälten ifrån databasen, så att vår page i frmContent
            // alltid visar aktuell information
            using (var context = new Page2Model())
            {
                var query = from user in context.Users where user.User_ID == uid
                            join adress in context.Adresses on user.Adress_ID equals adress.Adress_ID
                            select new
                            {
                                user.First_Name,
                                user.Last_Name,
                                user.Date_of_Birth,
                                user.Drivers_Licence,
                                user.Email,
                                user.English,
                                user.Swedish,
                                user.Title,
                                user.Phone,
                                user.Mobile,
                                user.Personal_Information,
                                adress.Street01,
                                adress.Street02,
                                adress.City,
                                adress.Zipcode
                            };
                foreach(var u in query)
                {
                    TxtbxFirstName.Text = u.First_Name;
                    TxtbxLastName.Text = u.Last_Name;
                    TxtbxEmail.Text = u.Email;
                    TxtbxMobile.Text = u.Mobile;
                    TxtbxPhone.Text = u.Phone;
                    TxtbxPersonalInfo.Text = u.Personal_Information;
                    TxtbxTitle.Text = u.Title;
                    TxtbxStreet01.Text = u.Street01;
                    TxtbxStreet02.Text = u.Street02;
                    TxtbxZip.Text = u.Zipcode.ToString();
                    TxtbxCity.Text = u.City;
                    SldEnglish.Value = (double)u.English;
                    SldSwedish.Value = (double)u.Swedish;
                    if (u.Drivers_Licence)
                        ChkbxLicence.IsChecked = true;
                    DPDate.SelectedDate = u.Date_of_Birth;
                }
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
                using(var model = new Page2Model())
                {
                    Users u = model.Users.Find(uid);
                    u.Date_of_Birth = (u.Date_of_Birth != (DateTime)DPDate.SelectedDate) ? (DateTime)DPDate.SelectedDate : u.Date_of_Birth;
                    u.First_Name = (u.First_Name != TxtbxFirstName.Text) ? TxtbxFirstName.Text : u.First_Name;
                    u.Last_Name = (u.Last_Name != TxtbxLastName.Text) ? TxtbxLastName.Text : u.Last_Name;
                    u.Phone = (u.Phone != TxtbxPhone.Text) ? TxtbxPhone.Text : u.Phone;
                    u.Mobile = (u.Mobile != TxtbxMobile.Text) ? TxtbxMobile.Text : u.Mobile;
                    u.Email = (u.Email != TxtbxEmail.Text) ? TxtbxEmail.Text : u.Email;
                    u.Personal_Information = (u.Personal_Information != TxtbxPersonalInfo.Text) ? TxtbxPersonalInfo.Text : u.Personal_Information;
                    u.Drivers_Licence = ((bool)ChkbxLicence.IsChecked) ? true : false;
                    u.English = (u.English != (byte)SldEnglish.Value) ? (byte)SldEnglish.Value : u.English;
                    u.Swedish = (u.Swedish != (byte)SldSwedish.Value) ? (byte)SldSwedish.Value : u.Swedish;
                    u.Title = (u.Title != TxtbxTitle.Text) ? TxtbxTitle.Text : u.Title;
                    Adresses a = model.Adresses.Find(u.Adress_ID);
                    a.Street01 = (a.Street01 != TxtbxStreet01.Text) ? TxtbxStreet01.Text : a.Street01;
                    a.Street02 = (a.Street02 != TxtbxStreet02.Text) ? TxtbxStreet02.Text : a.Street02;
                    a.Zipcode = (a.Zipcode != Convert.ToInt32(TxtbxZip.Text)) ? Convert.ToInt32(TxtbxZip) : a.Zipcode;
                    a.City = (a.City != TxtbxCity.Text) ? TxtbxCity.Text : a.City;

                    model.SaveChanges();

                }
                Page3 p3 = new Page3(uid);
                pageFrame.Navigate(p3);
            }
        }

        private void txtbxPersonalInfo_TextChanged(object sender, TextChangedEventArgs e)
        {
            LblCounter.Content = "0 out of 1024";
            TextBox txtbx = (TextBox)sender;
            int count = txtbx.Text.Length;
            string content = count + " out of 1024";
            LblCounter.Content = content.ToString();
        }

        private void TxtbxFirstName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TxtbxFirstName.Text.Length > 1)
            {
                TxtbxFirstName.BorderBrush = Brushes.Green;
            }
            else
            {
                TxtbxFirstName.BorderBrush = Brushes.Red;
            }
        }

        private void TxtbxFirstName_MouseEnter(object sender, MouseEventArgs e)
        {
            if (TxtbxFirstName.BorderBrush == Brushes.Red)
            {
                TxtbxFirstName.BorderBrush = Brushes.Red;
            }
            else
            {
                TxtbxFirstName.BorderBrush = Brushes.Green;
            }
        }
    }
}
