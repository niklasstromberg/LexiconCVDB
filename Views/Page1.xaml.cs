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
using XBAPLexiconCVDBInterface;
using XBAPLexiconCVDBInterface.Views;
using System.Data.Entity;
using XBAPLexiconCVDBInterface.Extentionmethods;
using System.Collections.Generic;

namespace XBAPLexiconCVDBInterface.Views
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class Page1 : Page
    {
        // Variables
        public bool isCreate;
        public int selectedUID = -1;
        string conn = ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString;

        public Page1()
        {
            InitializeComponent();
            ShowHide();

            using (SqlConnection con = new SqlConnection(conn))
            {
                string SqlQ = string.Format("select * from view_user_list");
                //select user_ID as ID, first_name as Förnamn, last_name as Efternamn from users
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
            selectedUID = -1;
            Page2 p2 = new Page2(selectedUID);
            FrmContent.Navigate(p2);
            LblPersonalInformation.Visibility = Visibility.Hidden;
            ShowHide();
            GetImage();
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
            DataGrid dg = sender as DataGrid;
            DataRowView selected = (DataRowView)dg.SelectedItems[0];
            selectedUID = selected.Row.Field<int>(0);
            FillDetails(selectedUID);
            LblPersonalInformation.Content = selected.Row.Field<string>(1) + " " + selected.Row.Field<string>(2);
            txtbxtest.Text = selectedUID.ToString();
            GetImage();
            ShowHide();
            Page2 p2 = new Page2(selectedUID);
            FrmContent.Navigate(p2);
        }

        // Helpermethods

        // Shows or hides elements on the screen depending on whether or not a
        // person has been selected in the datagrid.
        public void ShowHide()
        {
            if (selectedUID < 0)
            {
                LblPersonalInformation.Visibility = Visibility.Hidden;
                BtnDetails.Visibility = Visibility.Hidden;
                BtnJournal.Visibility = Visibility.Hidden;
                BtnLog.Visibility = Visibility.Hidden;
                BtnUserImage.Visibility = Visibility.Hidden;
                image.Visibility = Visibility.Hidden;
                BtnDelete.Visibility = Visibility.Hidden;
                PopupDelete.IsOpen = false;
                PopupDetails.IsOpen = false;
                PopupEditJournal.IsOpen = false;
                PopupJournal.IsOpen = false;
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
                PopupDetails.IsOpen = false;
                PopupDelete.IsOpen = false;
                PopupEditJournal.IsOpen = false;
                PopupJournal.IsOpen = false;
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

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            PopupDelete.IsOpen = false;
        }

        private void BtnYes_Click(object sender, RoutedEventArgs e)
        {
            using (CVDBContext db = new CVDBContext())
            {
                Users userToDelete = db.Users.Find(selectedUID);
                db.Users.Remove(userToDelete);
                db.SaveChanges();
            }
            //delete user, never to return!
            txtbxtest.Text = "Användare med id: " + selectedUID + " borttagen.";
            PopupDelete.IsOpen = false;
            dgContentList.SelectedIndex = -1;
            ShowHide();
            Page_default p = new Page_default();
            FrmContent.Navigate(p);

        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            PopupEditJournal.IsOpen = false;
            PopupJournal.IsOpen = false;
            PopupDetails.IsOpen = false;
            PopupDelete.IsOpen = true;
        }

        private void BtnSaveDetails_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new CVDBContext())
            {
                User_Details ud = db.User_Details.Find(selectedUID);

                ud.User_ID = selectedUID;
                ud.Modified = DateTime.Now;
                ud.Active = (bool)ChkActive.IsChecked;
                ud.Available = (bool)ChkAvailable.IsChecked;
                ud.Available_Date = DPDetails.SelectedDate;
                ud.Salary = GetNumber(TxtDetailsSalary.Text);

                db.Entry(ud).State = EntityState.Modified;

                txtbxtest.Text = ud.ToString();
                db.SaveChanges();
            }
            PopupDetails.IsOpen = false;
        }

        private void BtnDetails_Click(object sender, RoutedEventArgs e)
        {
            PopupEditJournal.IsOpen = false;
            PopupJournal.IsOpen = false;
            PopupDelete.IsOpen = false;
            PopupDetails.IsOpen = PopupDetails.IsOpen == true ? false : true;
        }

        private void ChkAvailable_Checked(object sender, RoutedEventArgs e)
        {
            DPDetails.Visibility = Visibility.Hidden;
        }

        private void ChkAvailable_UnChecked(object sender, RoutedEventArgs e)
        {
            DPDetails.Visibility = Visibility.Visible;
        }

        private int GetNumber(string s)
        {
            string tmp = s.Remove(s.IndexOf(' '));

            int result = 0;
            if (Int32.TryParse(tmp, out result))
                return result;

            return result;
        }

        private void FillDetails(int id)
        {
            using (var db = new CVDBContext())
            {
                var ud = from u_d in db.User_Details
                         where u_d.User_ID == selectedUID
                         select u_d;
                ChkActive.IsChecked = ud.FirstOrDefault().Active;
                ChkAvailable.IsChecked = ud.FirstOrDefault().Available;
                DPDetails.SelectedDate = ud.FirstOrDefault().Available_Date;
                TxtDetailsSalary.Text = AddSEK(ud.FirstOrDefault().Salary.ToString());
            }
        }

        private string AddSEK(string input)
        {
            if (!input.Contains("SEK"))
                input += " SEK";
            return input;
        }

        private void TxtDetailsSalary_LostFocus(object sender, RoutedEventArgs e)
        {
            TxtDetailsSalary.Text = AddSEK(TxtDetailsSalary.Text);
        }

        private void DGJournals_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            using (var db = new CVDBContext())
            {
                var query = from j in db.Journals
                            where j.User_ID == selectedUID
                            select j;
            }
            PopupEditJournal.IsOpen = true;
        }

        private void BtnJournal_Click(object sender, RoutedEventArgs e)
        {
            PopupJournal.IsOpen = PopupJournal.IsOpen == true ? false : true;
            PopupDetails.IsOpen = false;
            PopupDelete.IsOpen = false;
            PopupEditJournal.IsOpen = false;
            //DGJournals.Items.Clear();

            using (var db = new CVDBContext())
            {
                var query = from j in db.Journals
                            where j.User_ID == selectedUID
                            select new { Journal_ID = j.Journal_ID, Created = j.Created, Notes = j.Notes };
                var tmp = query.ToList();
                DGJournals.ItemsSource = tmp;
                //foreach (var v in tmp)
                //{
                //    v.Created = v.Created.Date;
                //    v.Notes = v.Notes.GetFirst25() + "...";
                //}
                //var query2 = from j2 in tmp
                //             select new { Date = j2.Created.ToShortDateString(), Journal = j2.Notes };
                //var results = query2.ToList();
                //DGJournals.ItemsSource = tmp;
            }
        }

        private void BtnNewJournal_Click(object sender, RoutedEventArgs e)
        {
            PopupEditJournal.IsOpen = PopupEditJournal.IsOpen == true ? false : true;
            BtnNewJournal.Content = PopupEditJournal.IsOpen == true ? "Close" : "New Entry";
            LblJournalDate.Content = DateTime.Now;
        }

        private void BtnJournalDelete_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnJournalSave_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new CVDBContext())
            {
                if (DGJournals.SelectedIndex < 0)
                {
                    Journals j = new Journals
                    {
                        Created = DateTime.Now,
                        User_ID = selectedUID,
                        Notes = TxtBxJournalEntry.Text
                    };
                    db.Journals.Add(j);
                    db.Entry(j).State = EntityState.Added;
                }
                else
                {
                    Journals j = new Journals
                    {
                        Notes = TxtBxJournalEntry.Text
                    };
                    db.Entry(j).State = EntityState.Modified;
                }
                db.SaveChanges();
            }
            PopupEditJournal.IsOpen = false;
            PopupJournal.IsOpen = false;
        }


    }
}




