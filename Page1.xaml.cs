using System;
using System.Collections.Generic;
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
using Microsoft.Win32;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Azure;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Data.SqlClient;
using System.Data;
using System.Xml;

namespace XBAPLexiconCVDBInterface
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class Page1 : Page
    {
        //Uri userImage;
        //string userImageString;
        int selectedUID;
        DataSet selectedUser;
        //UserToDisplay choice = new UserToDisplay();
        //DataObject datao = new DataObject();
        //this.dataContext = choice;


        public Page1()
        {
            InitializeComponent();

            string connString = "Data Source=lexiconitkonsultdbserver.database.windows.net,1433;Initial Catalog=LexiconITKonsultDB;Persist Security Info=True;User ID=lexicondbadmin;Password=Pa$$w0rd";
            //string connString = Environment.GetEnvironmentVariable("SQLAZURECONNSTR_dbConnectionString");
            if (dgContentList.SelectedIndex < 0)
            {
                LblPersonalInformation.Visibility = Visibility.Hidden;
            }



            using (SqlConnection con = new SqlConnection(connString))
            {
                string SqlQ = string.Format("select user_ID as ID, first_name as Förnamn, last_name as Efternamn from users");

                con.Open();
                SqlCommand com = new SqlCommand(SqlQ, con);
                //com.Parameters.AddWithValue("@Forename", txtSearch.Text);

                using (SqlDataAdapter adapter = new SqlDataAdapter(com))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dgContentList.ItemsSource = dt.DefaultView;
                }
            }
        }

        // Helpermethods

        public async void GetImage(int userID)
        {
            if (await (App.Current as App).checkConnection())
            {
                //if (dgContentList.SelectedItems)
                //if(userID = 1)
                //{
                //userImage = new Uri("user.jpg", UriKind.Absolute);
                //BtnUserImage.
                //}
            }
            //BtnUserImage.Content = userImage;
            //image.Source = userImage;
            //BtnUserImage.Foreground = Brushes.AliceBlue;
        }

        //public ImageSource userImage
        //{
        //    get
        //    {
        //        Uri userImage = new Uri("user.jpg", UriKind.Absolute);
        //        return userImage;             }
        //    set
        //    {
        //        if (value != null)
        //        {
        //            userImage = value;
        //            //RaisePropertyChanged(() => PlayPauseImg);
        //        }
        //    }
        //}

        private async void BtnUserImage_Click(object sender, RoutedEventArgs e)
        {
            GetImage(1);
            //var cofd = new CommonOpenFileDialog();
            //if (cofd.ShowDialog() == CommonFileDialogResult.Ok)
            //{
            //    string name = System.IO.Path.GetFileName(cofd.FileName);
            //    CloudBlockBlob blockBlob = (App.Current as App).blobcontainer.GetBlockBlobReference(name);
            //    using (var fileStream = System.IO.File.OpenRead(name))
            //    {
            //        await blockBlob.UploadFromStreamAsync(fileStream);
            //    }
            //}
            //else
            //{
            //    // sätt button background till user.png
            //}
        }


        private void dgContentList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            (App.Current as App).choice = new UserToDisplay();
            
            selectedUser = new DataSet();
            selectedUser.Clear();



            DataGrid dg = sender as DataGrid;
            DataRowView selected = (DataRowView)dg.SelectedItems[0];
            selectedUID = selected.Row.Field<int>(0);
            LblPersonalInformation.Content = (selected.Row.Field<string>("Förnamn")) + " " + (selected.Row.Field<string>("Efternamn"));


            string connString = "Data Source=lexiconitkonsultdbserver.database.windows.net,1433;Initial Catalog=LexiconITKonsultDB;Persist Security Info=True;User ID=lexicondbadmin;Password=Pa$$w0rd";


            //string connString = Environment.GetEnvironmentVariable("SQLAZURECONNSTR_dbConnectionString");

            string SqlQ = string.Format("procGetUser");// {0}", selectedUID);
            using (SqlConnection con = new SqlConnection(connString))
            {
                using (SqlCommand cmd = new SqlCommand(SqlQ, con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@uID", selectedUID);
                    con.Open();
                    //cmd.ExecuteNonQuery();
                    //cmd.ExecuteReader();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    //datao.SetData(selectedUser);
                    da.SelectCommand = cmd;
                    da.Fill(selectedUser);
                    con.Close();
                }
            }
            string test1 = selectedUser.GetXmlSchema();
            string test = selectedUser.GetXml();
            LblPersonalInformation.Visibility = Visibility.Visible;
            using (DataTableReader reader = selectedUser.CreateDataReader())
            {
                string test5 = "";
                string test6 = "";
                string s = "";
                while (reader.Read())
                {
                    string test3 = reader.GetName(2); // ger Last_Name
                    int test2 = reader.FieldCount; // ger antal fields
                    (App.Current as App).choice.user_id = (int)reader.GetValue(0);
                    (App.Current as App).choice.first_name = reader.GetValue(1).ToString();
                    (App.Current as App).choice.last_name = reader.GetValue(2).ToString();
                    (App.Current as App).choice.title = reader.GetValue(3).ToString();
                    (App.Current as App).choice.date_of_birth = DateTime.Today;//(DateTime)reader.GetValue(4);
                    //5 adress id
                    //6 phone
                    //7 mobile
                    //8 email
                    (App.Current as App).choice.swedish = Convert.ToInt32(reader.GetValue(9));
                    (App.Current as App).choice.english = Convert.ToInt32(reader.GetValue(10));
                    (App.Current as App).choice.driver = (bool)reader.GetValue(11);


                    for (int i = 0; i < test2; i++)
                    {
                        s += reader.GetName(i) + ": " + reader.GetValue(i).ToString() + "\n";
                        //string str = reader.GetValue(2).ToString();

                        //test5 += s;
                        //test6 += str;
                    }
                }

                txtbxtest.Text = (App.Current as App).choice.first_name + " " + (App.Current as App).choice.last_name + "\n\n" + s;
                //FrmContent.NavigationService.Navigate("/XBAPLexiconCVDBInterface;component/page2.xaml");
                //Uri p2 = new Uri("/XBAPLexiconCVDBInterface;component/page2.xaml");
                Uri p2 = new Uri("/XBAPLexiconCVDBInterface;component/page2.xaml", UriKind.Relative);
                FrmContent.Source = p2;
                //Frame frmContent = "/XBAPLexiconCVDBInterface;component/page2.xaml";
            }
            //txtbxtest.Text = test + test1 + test2.ToString() + test3 + test4;
            //private void BtnUserImage_Click(object sender, RoutedEventArgs e)
            //{

            //}

        }

        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            Uri p2 = new Uri("/XBAPLexiconCVDBInterface;component/page2.xaml", UriKind.Relative);
            FrmContent.Source = p2;
        }
    }
}




