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


        public Page1()
        {
            InitializeComponent();

            string connString = "Data Source=lexiconitkonsultdbserver.database.windows.net,1433;Initial Catalog=LexiconITKonsultDB;Persist Security Info=True;User ID=lexicondbadmin;Password=Pa$$w0rd";
            //string connString = Environment.GetEnvironmentVariable("SQLAZURECONNSTR_dbConnectionString");




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
                    selectedUser = new DataSet();
                    da.SelectCommand = cmd;
                    da.Fill(selectedUser);
                    con.Close();
                }
            }
            string test = selectedUser.GetXml();
            txtbxtest.Text = test;
        }

        //private void BtnUserImage_Click(object sender, RoutedEventArgs e)
        //{

        //}

    }
}




