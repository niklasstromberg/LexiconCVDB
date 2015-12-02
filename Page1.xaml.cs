using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using Dapper;
using System.Configuration;
using System.Drawing;

namespace XBAPLexiconCVDBInterface
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class Page1 : Page
    {
        // Variables
        public bool isCreate;
        public int selectedUID;
        string conn = ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString;

        public Page1()
        {
            InitializeComponent();
            ShowHide();

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

        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            selectedUID = 0;
            Page2 p2 = new Page2(selectedUID);
            FrmContent.Navigate(p2);
            LblPersonalInformation.Visibility = Visibility.Hidden;
        }

        // Lets the user choose and upload an image from file to the blob storage.
        // The file is saved under the name "User[id]img".
        // The method then calls the GetImage() method to display the uploaded image.
        private async void BtnUserImage_Click(object sender, RoutedEventArgs e)
        {
            var cofd = new CommonOpenFileDialog();
            cofd.Filters.Add(new CommonFileDialogFilter("JPEG Files", "*.jpg"));
            cofd.Filters.Add(new CommonFileDialogFilter("PNG Files", "*.png"));

            if (cofd.ShowDialog() == CommonFileDialogResult.Ok)
            {
                string name = "User" + selectedUID + "img";
                CloudBlockBlob blockBlob = (App.Current as App).blobcontainer.GetBlockBlobReference(name);
                blockBlob.Properties.ContentType = "image/jpg";
                using (var fileStream = System.IO.File.OpenRead(cofd.FileName))
                {
                    await blockBlob.UploadFromStreamAsync(fileStream);
                    {
                        GetImage();
                    }
                }
            }
        }
        

        private void dgContentList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GetImage();
            ShowHide();

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
            Page2 p2 = new Page2(selectedUID);
            FrmContent.Navigate(p2);
        }

        // Helpermethods

        // Shows or hides elements on the screen depending on whether or not a
        // person has been selected in the datagrid.
        public void ShowHide()
        {
            if (dgContentList.SelectedIndex < 1)
            {
                LblPersonalInformation.Visibility = Visibility.Hidden;
                BtnDetails.Visibility = Visibility.Hidden;
                BtnJournal.Visibility = Visibility.Hidden;
                BtnLog.Visibility = Visibility.Hidden;
                BtnUserImage.Visibility = Visibility.Hidden;
                image.Visibility = Visibility.Hidden;
                BtnDelete.Visibility = Visibility.Hidden;
            }
            else
            {
                LblPersonalInformation.Visibility = Visibility.Visible;
                BtnDetails.Visibility = Visibility.Visible;
                BtnJournal.Visibility = Visibility.Visible;
                BtnLog.Visibility = Visibility.Visible;
                BtnUserImage.Visibility = Visibility.Visible;
                image.Visibility = Visibility.Visible;
                BtnDelete.Visibility = Visibility.Visible;
            }
        }

        // Gets the users image from the azure blob storage and shows it in the interface
        // If no image exists in the blob storage for the chosen user, the default image
        // is displayed
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
                        System.Drawing.Bitmap bitmap = XBAPLexiconCVDBInterface.Properties.Resources.user;
                        image.Source = BitmapToImageSource(bitmap);
                    }
                }
            }
        }

        // Converts parameter bitmap to ImageSource
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




