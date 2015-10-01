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
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;
using Dapper;
using XBAPLexiconCVDBInterface;
using System.Configuration;
using System.Reflection;
using System.Drawing;

namespace XBAPLexiconCVDBInterface
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class Page1 : Page
    {

        public bool isCreate;
        public int selectedUID;
        string conn = ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString;

        public Page1()
        {
            InitializeComponent();

            if (dgContentList.SelectedIndex < 0)
            {
                LblPersonalInformation.Visibility = Visibility.Hidden;
            }

            using (SqlConnection con = new SqlConnection(conn))
            {
                string SqlQ = string.Format("select user_ID as ID, first_name as Förnamn, last_name as Efternamn from users");

                con.Open();
                SqlCommand com = new SqlCommand(SqlQ, con);
                using (SqlDataAdapter adapter = new SqlDataAdapter(com))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dgContentList.ItemsSource = dt.DefaultView;
                }
            }
        }

        // Helpermethods

        public async void GetImage()
        {
            if (await (App.Current as App).checkConnection())
            {
                string name = "User" + selectedUID + "img";
                if ((App.Current as App).blobcontainer.Exists())
                {
                    CloudBlockBlob blockBlob = (App.Current as App).blobcontainer.GetBlockBlobReference(name);
                    if (blockBlob.Exists())
                    {
                        ImageSource imageSource = new BitmapImage(new Uri(blockBlob.Uri.ToString()));
                        image.Source = imageSource;
                    }
                    else
                    {
                        System.Drawing.Bitmap bitmap1 = XBAPLexiconCVDBInterface.Properties.Resources.user;
                        image.Source = BitmapToImageSource(bitmap1);
                        //ImageSource imgs = new BitmapImage(new Uri("XBAPLexiconCVDBInterface.Properties.Resources.user.jpg", UriKind.Absolute));
                        //ImageSource i = new Uri(bitmap1, UriKind.Absolute);
                        //image.Source = new BitmapImage(new Uri("XBAPLexiconCVDBInterface.Properties.Resources.user.jpg", UriKind.RelativeOrAbsolute));
                        //image.Source = new BitmapImage(new Uri("XBAPLexiconCVDBInterface.Properties.Resources.user", UriKind.Absolute));
                    }
                }
            }
        }

        private async void BtnUserImage_Click(object sender, RoutedEventArgs e)
        {
            GetImage();
            var cofd = new CommonOpenFileDialog();
            if (cofd.ShowDialog() == CommonFileDialogResult.Ok)
            {
                //string name = System.IO.Path.GetFileName(cofd.FileName);
                string name = "User" + selectedUID + "img";
                CloudBlockBlob blockBlob = (App.Current as App).blobcontainer.GetBlockBlobReference(name);
                blockBlob.Properties.ContentType = "image/jpg";
                using (var fileStream = System.IO.File.OpenRead(cofd.FileName))
                {
                    await blockBlob.UploadFromStreamAsync(fileStream);
                }
            }
            //else
            //{
            //    // sätt button background till user.png
            //}
        }


        private void dgContentList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GetImage();

            DisplayUser displayUser = new DisplayUser();

            DataGrid dg = sender as DataGrid;
            DataRowView selected = (DataRowView)dg.SelectedItems[0];
            selectedUID = selected.Row.Field<int>(0);
            LblPersonalInformation.Content = (selected.Row.Field<string>("Förnamn")) + " " + (selected.Row.Field<string>("Efternamn"));

            using (IDbConnection connection = new SqlConnection(conn))
            {
                string query = "select first_name, last_name from users where user_id = " + selectedUID;
                displayUser = connection.Query<DisplayUser>(query).FirstOrDefault();
                
            }

            LblPersonalInformation.Visibility = Visibility.Visible;

            Page2 p2 = new Page2(selectedUID);
            FrmContent.Navigate(p2);

        }

        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            selectedUID = 0;
            Page2 p2 = new Page2(selectedUID);
            FrmContent.Navigate(p2);
            LblPersonalInformation.Visibility = Visibility.Hidden;
        }

        BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }
    }
}




