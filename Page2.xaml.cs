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

            // Här behöver vi hämta informationen som ska in i fälten ifrån databasen, så att vår page i frmContent
            // alltid visar aktuell information
            DisplayUser displayUser = new DisplayUser();
            uid = id;
            using (IDbConnection connection = new SqlConnection(conn))
            {
                string query = "select first_name, last_name from users where user_id = " +id;
                displayUser = connection.Query<DisplayUser>(query).FirstOrDefault();

            }

            if (id > 0)
            {
                TxtbxFirstName.Text = displayUser.First_Name;//(App.Current as App).choice.First_Name;
                TxtbxLastName.Text = displayUser.Last_Name;//(App.Current as App).choice.Last_Name;
                //TxtbxTitle.Text = (App.Current as App).choice.Title;
                //DPDate.SelectedDate = (App.Current as App).choice.Date_of_Birth;
                //SldSwedish.Value = (App.Current as App).choice.Swedish;
                //SldEnglish.Value = (App.Current as App).choice.English;
            }
        }

        private void BtnPage3_Click(object sender, RoutedEventArgs e)
        {
            //string SqlQ = string.Format("procInsertUser");
            //using (SqlConnection con = new SqlConnection(connString))
            //{
            //    using (SqlCommand cmd = new SqlCommand(SqlQ, con))
            //    {
            //        cmd.CommandType = CommandType.StoredProcedure;
            //        cmd.Parameters.AddWithValue("@first_name", TxtbxFirstName.Text);
            //        cmd.Parameters.AddWithValue("@last_name", TxtbxLastName.Text);
            //        cmd.Parameters.AddWithValue("@title", TxtbxTitle.Text);
            //        cmd.Parameters.AddWithValue("@date_of_birth", DPDate.SelectedDate);
            //        cmd.Parameters.AddWithValue("@phone", TxtbxPhone.Text);
            //        cmd.Parameters.AddWithValue("@mobile", TxtbxMobile.Text);
            //        cmd.Parameters.AddWithValue("@email", TxtbxEmail.Text);
            //        cmd.Parameters.AddWithValue("@swedish", SldSwedish.Value);
            //        cmd.Parameters.AddWithValue("@english", SldEnglish.Value);
            //        cmd.Parameters.AddWithValue("@drivers_licence", ChkbxLicence.IsChecked);
            //        cmd.Parameters.AddWithValue("@personal_information", TxtbxPersonalInfo.Text);
            //        cmd.Parameters.AddWithValue("@synopsis", "");
            //        cmd.Parameters.AddWithValue("@linkedin", "");
            //        cmd.Parameters.AddWithValue("@photo", "");
            //        cmd.Parameters.AddWithValue("@salary_interval_id", 5);
            //        cmd.Parameters.AddWithValue("@street01", TxtbxStreet01.Text);
            //        cmd.Parameters.AddWithValue("@street02", TxtbxStreet02.Text);
            //        cmd.Parameters.AddWithValue("@zipcode", TxtbxZip.Text);
            //        cmd.Parameters.AddWithValue("@city", TxtbxCity.Text);
            //        con.Open();
            //        cmd.ExecuteNonQuery();
            //        con.Close();
            //    }
            //}

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
                // This method also needs to save the information in the fields of page2
                // to the database
                Page3 p3 = new Page3(uid);
                pageFrame.Navigate(p3);
                //pageFrame.Source = new Uri("Page3.xaml", UriKind.Relative);
            }
        }

        private void txtbxPersonalInfo_TextChanged(object sender, TextChangedEventArgs e)
        {
            LblCounter.Content = "0 out of 1024";
            TextBox txtbx = (TextBox)sender;
            //var textBox = sender as TextBox;
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
            //TxtbxFirstName.BorderBrush = (TxtbxFirstName.BorderBrush = Brushes.Red ? TxtbxFirstName.BorderBrush = Brushes.Red : TxtbxFirstName.BorderBrush = Brushes.Green);
        }




    }
}
