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
using XBAPLexiconCVDBInterface.ViewModels;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Windows.Controls.Primitives;
using System.Collections;
using System.Windows.Navigation;
using System.ComponentModel;
using System.Net;

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
        public int loggedInAs = 1;
        string conn = ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString;

        public Page1()
        {
            InitializeComponent();
            ShowHide();
            dgContentList.UpdateGrid();
            FillTags();
        }

        string currentPage = "";
        public object GetCurrentPage()
        {
            object CurrentPage = FrmContent.Content.GetType();
            currentPage = CurrentPage.ToString();
            return CurrentPage;
        }

        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            ShowHide();
            isCreate = true;
            selectedUID = -1;
            Page2 p2 = new Page2(selectedUID);
            FrmContent.Navigate(p2);
            Btnp2.Visibility = Visibility.Visible;
            Btnp2.Background = System.Windows.Media.Brushes.Green;
            LblPersonalInformation.Visibility = Visibility.Hidden;
            Btnp2.Content = "Save";
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
                try
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
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + ex.InnerException, "uploading or downloading image error");
                }
            }
        }

        private void dgContentList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgContentList.SelectedIndex >= 0)
            {
                var obj = dgContentList.SelectedItem;
                DataGridColumn column = dgContentList.Columns[0];

                if (column.GetCellContent(obj) is TextBlock)
                {
                    TextBlock cellContent = column.GetCellContent(obj) as TextBlock;
                    string content = cellContent.Text.ToLower();
                    selectedUID = Convert.ToInt32(content);
                }
                System.Type type = obj.GetType();
                if (type != null)
                {
                    FillDetails(selectedUID);
                    string first = "";
                    string last = "";
                    DataGridColumn columnF = dgContentList.Columns[1];
                    if (columnF.GetCellContent(obj) is TextBlock)
                    {
                        TextBlock cellContent = columnF.GetCellContent(obj) as TextBlock;
                        first = cellContent.Text;
                    }
                    DataGridColumn columnL = dgContentList.Columns[2];
                    if (columnL.GetCellContent(obj) is TextBlock)
                    {
                        TextBlock cellContent = columnL.GetCellContent(obj) as TextBlock;
                        last = cellContent.Text;
                    }
                    LblPersonalInformation.Content = first.CapitalizeFirst() + " " + last.CapitalizeFirst();
                    txtbxtest.Text = selectedUID.ToString();
                    GetImage();
                    LblTags.Content = FillLabelTags(selectedUID);
                    ShowHide();
                    Btnp2.Background = System.Windows.Media.Brushes.Green;
                    Btnp3.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFDDDDDD"));
                    Btnp4.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFDDDDDD"));
                    Btnp5.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFDDDDDD"));
                    Page2 p2 = new Page2(selectedUID);
                    FrmContent.Navigate(p2);
                }
            }
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
                LblTags.Visibility = Visibility.Hidden;
                TxtbxTagFilter.Visibility = Visibility.Hidden;
                BtnAddTag.Visibility = Visibility.Hidden;
                LstBxTags.Visibility = Visibility.Hidden;
                Btnp2.Visibility = Visibility.Hidden;
                Btnp3.Visibility = Visibility.Hidden;
                Btnp4.Visibility = Visibility.Hidden;
                Btnp5.Visibility = Visibility.Hidden;
                PopupDelete.IsOpen = false;
                PopupDetails.IsOpen = false;
                PopupEditJournal.IsOpen = false;
                PopupJournal.IsOpen = false;
                ImgTags.Visibility = Visibility.Hidden;
                BtndownImage.Visibility = Visibility.Hidden;
                BtnuploadImage.Visibility = Visibility.Hidden;
            }
            else
            {
                BtndownImage.Visibility = Visibility.Visible;
                BtnuploadImage.Visibility = Visibility.Visible;
                LblPersonalInformation.Visibility = Visibility.Visible;
                BtnDetails.Visibility = Visibility.Visible;
                BtnJournal.Visibility = Visibility.Visible;
                BtnLog.Visibility = Visibility.Visible;
                BtnUserImage.Visibility = Visibility.Visible;
                image.Visibility = Visibility.Visible;
                BtnDelete.Visibility = Visibility.Visible;
                LblTags.Visibility = Visibility.Visible;
                TxtbxTagFilter.Visibility = Visibility.Visible;
                BtnAddTag.Visibility = Visibility.Visible;
                LstBxTags.Visibility = Visibility.Visible;
                Btnp2.Visibility = Visibility.Visible;
                Btnp3.Visibility = Visibility.Visible;
                Btnp4.Visibility = Visibility.Visible;
                Btnp5.Visibility = Visibility.Visible;
                PopupDetails.IsOpen = false;
                PopupDelete.IsOpen = false;
                PopupEditJournal.IsOpen = false;
                PopupJournal.IsOpen = false;
                ImgTags.Visibility = Visibility.Visible;
            }
        }

        // Gets the users image from the azure blob storage and shows it in the interface
        // If no image exists in the blob storage for the chosen user, the default image
        // is displayed
        public async void GetImage()
        {
            try
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ex.InnerException, "getting image error");
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

        private async void BtnuploadImage_Click(object sender, RoutedEventArgs e)
        {
            var cofd = new CommonOpenFileDialog();

            if (cofd.ShowDialog() == CommonFileDialogResult.Ok)
            {
                try
                {
                    string fName = Path.GetFileName(cofd.FileName);
                    string name = "User" + selectedUID + "file_" + fName;
                    CloudBlockBlob blockBlob = (App.Current as App).blobcontainer.GetBlockBlobReference(name);
                    using (var fileStream = System.IO.File.OpenRead(cofd.FileName))
                    {
                        await blockBlob.UploadFromStreamAsync(fileStream);
                        {
                            MessageBox.Show(blockBlob.Uri.ToString());
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + ex.InnerException, "uploading file error");
                }
            }
        }

        Dictionary<string, Uri> bloblist = new Dictionary<string, Uri>();
        private void BtndownImage_Click(object sender, RoutedEventArgs e)
        {
            PopupBlobs.IsOpen = PopupBlobs.IsOpen == true ? false : true;
            bloblist.Clear();
            LBBlobList.Items.Clear();
            IEnumerable<IListBlobItem> initialList = (App.Current as App).blobcontainer.ListBlobs(null, true);

            foreach (IListBlobItem item in initialList)
            {
                if (item.GetType() == typeof(CloudBlockBlob))
                {
                    CloudBlockBlob blob = (CloudBlockBlob)item;
                    if (blob.Uri.ToString().Contains("User" + selectedUID))
                    {
                        if (!bloblist.ContainsValue(blob.Uri))
                        {
                            bloblist.Add(blob.Name, blob.Uri);
                        }
                    }
                }
            }

            foreach (var v in bloblist)
            {
                LBBlobList.Items.Add(v.Key);
            }
        }

        private void LBBlobList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Uri uri = bloblist.First(x => x.Key == LBBlobList.SelectedItem.ToString()).Value;
            string file = LBBlobList.SelectedItem.ToString().Substring(LBBlobList.SelectedItem.ToString().IndexOf("_"));
            string name = LblPersonalInformation.Content + file;
            var cofd = new CommonOpenFileDialog();
            cofd.IsFolderPicker = true;
            cofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (cofd.ShowDialog() == CommonFileDialogResult.Ok)
            {
                string folder = cofd.FileName;
                try
                {
                    WebClient webClient = new WebClient();
                    webClient.DownloadFile(uri, folder + "\\" + name);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + ex.InnerException, "Download file error");
                }
            }
            PopupBlobs.IsOpen = false;
        }

        #region PopupDelete
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
            dgContentList.UpdateGrid();
            Page_default p = new Page_default();
            FrmContent.Navigate(p);

        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            PopupEditJournal.IsOpen = false;
            PopupJournal.IsOpen = false;
            PopupDetails.IsOpen = false;
            PopupDelete.IsOpen = true;
            PopupLog.IsOpen = false;
            PopupEditLog.IsOpen = false;
        }
        #endregion

        #region PopupUserDetails
        private void BtnSaveDetails_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new CVDBContext())
            {
                User_Details ud = db.User_Details.Find(selectedUID);

                ud.User_ID = selectedUID;
                ud.Modified = DateTime.Now;
                ud.Active = (bool)ChkActive.IsChecked;
                ud.Available = (bool)ChkAvailable.IsChecked;
                if ((bool)ChkAvailable.IsChecked)
                {
                    DPDetails.SelectedDate = null;
                }
                ud.Available_Date = DPDetails.SelectedDate;
                ud.Salary = GetNumber(TxtDetailsSalary.Text);

                db.Entry(ud).State = EntityState.Modified;

                txtbxtest.Text = ud.ToString();
                db.SaveChanges();
            }
            PopupDetails.IsOpen = false;
            dgContentList.UpdateGrid();
        }

        private void BtnDetails_Click(object sender, RoutedEventArgs e)
        {
            PopupEditJournal.IsOpen = false;
            PopupJournal.IsOpen = false;
            PopupDelete.IsOpen = false;
            PopupEditLog.IsOpen = false;
            PopupLog.IsOpen = false;
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
                if ((bool)ChkAvailable.IsChecked)
                {
                    ChkAvailable.IsChecked = ud.FirstOrDefault().Available;
                    DPDetails.SelectedDate = null;
                }
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
        #endregion

        #region PopupJournals
        int JournalID;
        private void DGJournals_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DGJournals.SelectedIndex >= 0)
            {
                var obj = DGJournals.SelectedItem;
                System.Type type = obj.GetType();
                JournalID = (int)type.GetProperty("Journal_ID").GetValue(obj, null);

                using (var db = new CVDBContext())
                {
                    Journals journal = db.Journals.Find(JournalID);

                    LblJournalDate.Content = journal.Created;
                    LblJournalWho.Content = journal.GetLexiconHandle();
                    TxtBxJournalEntry.Text = journal.Notes;
                }
                BtnNewJournal.Content = "Close";
                PopupEditJournal.IsOpen = true;
            }
        }

        private void BtnJournal_Click(object sender, RoutedEventArgs e)
        {
            BtnNewJournal.Content = "New Entry";
            PopupJournal.IsOpen = PopupJournal.IsOpen == true ? false : true;
            PopupDetails.IsOpen = false;
            PopupDelete.IsOpen = false;
            PopupLog.IsOpen = false;
            PopupEditLog.IsOpen = false;
            PopupEditJournal.IsOpen = false;
            FillJournals();
        }

        private void BtnNewJournal_Click(object sender, RoutedEventArgs e)
        {
            PopupEditJournal.IsOpen = PopupEditJournal.IsOpen == true ? false : true;
            BtnNewJournal.Content = PopupEditJournal.IsOpen == true ? "Close" : "New Entry";
            DGJournals.SelectedIndex = -1;
            DGJournals.SelectedItem = null;
            LblJournalDate.Content = DateTime.Now;
            string lhName = "";
            using (var db = new CVDBContext())
            {
                var query = from lh in db.Lexicon_Handles
                            where lh.Lexicon_Handle_ID == 1
                            select lh.First_Name + " " + lh.Last_Name;
                lhName = query.FirstOrDefault();
            }
            LblJournalWho.Content = lhName;
            TxtBxJournalEntry.Text = "";
            BtnJournalDelete.IsEnabled = false;
        }

        private void BtnJournalDelete_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new CVDBContext())
            {
                Journals JournalToDelete = db.Journals.Find(JournalID);
                db.Journals.Remove(JournalToDelete);
                db.SaveChanges();
            }
            PopupEditJournal.IsOpen = false;
            BtnNewJournal.Content = "New Entry";
            FillJournals();
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
                        Notes = TxtBxJournalEntry.Text,
                        Lexicon_Handle_ID = 1
                    };
                    db.Journals.Add(j);
                    db.Entry(j).State = EntityState.Added;
                }
                else
                {
                    Journals j = db.Journals.Find(JournalID);

                    j.Notes = TxtBxJournalEntry.Text;

                    db.Entry(j).State = EntityState.Modified;
                }
                db.SaveChanges();
            }
            PopupEditJournal.IsOpen = false;
            PopupJournal.IsOpen = true;
            BtnNewJournal.Content = "New Entry";
            FillJournals();
        }

        public void FillJournals()
        {
            using (var db = new CVDBContext())
            {
                var query = from j in db.Journals
                            where j.User_ID == selectedUID
                            select new { Journal_ID = j.Journal_ID, Created = j.Created, Notes = j.Notes };
                var tmp = query.ToList();
                DGJournals.ItemsSource = tmp;
            }
        }
        #endregion

        #region PopupLogs
        public void FillLogs()
        {
            using (var db = new CVDBContext())
            {
                var query = from l in db.Logs
                            join e in db.Log_Events on l.Event_ID equals e.Event_ID
                            where l.User_ID == selectedUID
                            select new { Log_ID = l.Log_ID, Created = l.Created, Event = e.Event_Name };
                var tmp = query.ToList();
                DGLogs.ItemsSource = tmp;
            }
        }

        private void BtnNewLog_Click(object sender, RoutedEventArgs e)
        {
            PopupEditLog.IsOpen = PopupEditLog.IsOpen == true ? false : true;
            BtnNewLog.Content = PopupEditLog.IsOpen == true ? "Close" : "New Log";
            LblLogDate.Content = DateTime.Now;
            string lhName = "";
            using (var db = new CVDBContext())
            {
                var query = from lh in db.Lexicon_Handles
                            where lh.Lexicon_Handle_ID == 1
                            select lh.First_Name + " " + lh.Last_Name;
                lhName = query.FirstOrDefault();
            }
            LblLogWho.Content = lhName;
            TxtBxLogEntry.Text = "";
            BtnLogDelete.IsEnabled = false;
            using (var db = new CVDBContext())
            {
                var query = from le in db.Log_Events
                            select le;
                foreach (Log_Events le in query)
                {
                    if (!CBLogEvents.Items.Contains(le.Event_Name))
                        CBLogEvents.Items.Add(le.Event_Name);
                    if (!eventlist.ContainsKey(le.Event_ID))
                    {
                        eventlist.Add(le.Event_ID, le.Event_Name);
                    }
                }
            }
        }

        int LogID;
        private void DGLogs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DGLogs.SelectedIndex >= 0)
            {
                var obj = DGLogs.SelectedItem;
                System.Type type = obj.GetType();
                LogID = (int)type.GetProperty("Log_ID").GetValue(obj, null);
                PopupEditLog.IsOpen = true;
                BtnNewLog.Content = "Close";
                using (var db = new CVDBContext())
                {
                    Logs log = db.Logs.Find(LogID);
                    var query = from ev in db.Log_Events
                                where ev.Event_ID == log.Event_ID
                                select ev.Event_Name;
                    string tmp = query.FirstOrDefault();
                    LblLogDate.Content = log.Created;
                    LblLogWho.Content = log.GetLexiconHandle();
                    CBLogEvents.SelectedItem = tmp;
                    TxtBxLogEntry.Text = log.Notes;
                }
                BtnLogDelete.IsEnabled = true;
            }
        }

        private void BtnLogDelete_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new CVDBContext())
            {
                Logs LogToDelete = db.Logs.Find(LogID);
                db.Logs.Remove(LogToDelete);
                db.SaveChanges();
            }

            PopupEditLog.IsOpen = false;
            BtnNewLog.Content = "New Log";
            FillLogs();
        }

        private void BtnLogSave_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new CVDBContext())
            {
                if (LogID < 1)
                {
                    Logs newLog = new Logs
                    {
                        Event_ID = getEventID(CBLogEvents.SelectedValue.ToString()),
                        Lexicon_Handle_ID = 1,
                        Created = DateTime.Now,
                        User_ID = selectedUID,
                        Notes = TxtBxLogEntry.Text
                    };
                    db.Logs.Add(newLog);
                    db.Entry(newLog).State = EntityState.Added;
                }
                else
                {
                    Logs logToUpdate = db.Logs.Find(LogID);
                    logToUpdate.Notes = TxtBxLogEntry.Text;
                    db.Entry(logToUpdate).State = EntityState.Modified;
                }
                db.SaveChanges();
            }
            PopupEditLog.IsOpen = false;
            BtnNewLog.Content = "New Log";
            FillLogs();
        }

        private void CBLogEvents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //var obj = DGLogs.SelectedItem;
            //System.Type type = obj.GetType();
            //LogID = (int)type.GetProperty("Log_ID").GetValue(obj, null);
        }

        Dictionary<int, string> eventlist = new Dictionary<int, string>();
        private void BtnLog_Click(object sender, RoutedEventArgs e)
        {
            FillLogs();
            PopupLog.IsOpen = PopupLog.IsOpen == true ? false : true;
            PopupEditLog.IsOpen = false;
            PopupEditJournal.IsOpen = false;
            PopupJournal.IsOpen = false;
            PopupDetails.IsOpen = false;
            PopupDelete.IsOpen = false;
            BtnNewLog.Content = "New Log";
        }

        private int getEventID(string s)
        {
            var v = eventlist.Single(x => x.Value == s);
            return v.Key;
        }
        #endregion

        #region SearchMethods
        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new CVDBContext())
            {
                var query = from user in db.Users
                            join ud in db.User_Details on user.User_ID equals ud.User_ID
                            where user.First_Name.Contains(TxtBoxSearch.Text) ||
                            user.Last_Name.Contains(TxtBoxSearch.Text)
                            orderby user.Last_Name, user.First_Name, ud.Available, ud.Available_Date
                            select new { user.User_ID, user.First_Name, user.Last_Name, ud.Available, ud.Available_Date };
                dgContentList.ItemsSource = query.ToList();
            }
        }


        private void TxtBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            List<string> searchliststrings = new List<string>();
            PopupSearchList.IsOpen = false;
            SearchList.SelectedItem = null;
            SearchList.SelectedIndex = -1;
            List<Extentionmethods.Extentionmethods.person> orglist = (List<Extentionmethods.Extentionmethods.person>)dgContentList.ItemsSource;//new List<Extentionmethods.Extentionmethods.person>();
            searchliststrings.Clear();
            List<Extentionmethods.Extentionmethods.person> newlist = new List<Extentionmethods.Extentionmethods.person>();
            txtbxtest.Text = "";
            if (TxtBoxSearch.Text.Length == 0)
            {
                dgContentList.UpdateGrid();
                orglist = (List<Extentionmethods.Extentionmethods.person>)dgContentList.ItemsSource;//new List<Extentionmethods.Extentionmethods.person>();
                PopupSearchList.IsOpen = false;
                //searchliststrings.Clear();
            }
            else
            {
                foreach (Extentionmethods.Extentionmethods.person p in orglist)
                {
                    if (p.first_name.ToLower().Contains(TxtBoxSearch.Text.ToLower()))
                    {
                        if (!newlist.Contains(p))
                            newlist.Add(p);
                        if (!searchliststrings.Contains(p.first_name))
                            searchliststrings.Add(p.first_name);
                    }
                    if (p.last_name.ToLower().Contains(TxtBoxSearch.Text.ToLower()))
                    {
                        if (!newlist.Contains(p))
                            newlist.Add(p);
                        if (!searchliststrings.Contains(p.last_name))
                            searchliststrings.Add(p.last_name);
                    }
                    foreach (string s in p.skills)
                    {
                        if (s.ToLower().Contains(TxtBoxSearch.Text.ToLower()))
                        {
                            if (!newlist.Contains(p))
                                newlist.Add(p);
                            if (!searchliststrings.Contains(s))
                                searchliststrings.Add(s);
                        }
                    }
                    foreach (string s in p.tags)
                    {
                        if (s.ToLower().Contains(TxtBoxSearch.Text.ToLower()))
                        {
                            if (!newlist.Contains(p))
                                newlist.Add(p);
                            if (!searchliststrings.Contains(s))
                                searchliststrings.Add(s);
                        }
                    }
                }
                dgContentList.ItemsSource = newlist;
                foreach (string s in searchliststrings)
                    txtbxtest.Text += s;
                SearchList.ItemsSource = searchliststrings;
                if (searchliststrings.Count > 0)
                    PopupSearchList.IsOpen = true;
            }
        }

        public IEnumerable<DataGridRow> GetDataGridRows(DataGrid grid)
        {
            var itemsSource = grid.ItemsSource as IEnumerable;
            if (null == itemsSource) yield return null;
            foreach (var item in itemsSource)
            {
                var row = grid.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;
                if (null != row) yield return row;
            }
        }

        private void BtnClearSearch_Click(object sender, RoutedEventArgs e)
        {
            isCreate = false;
            TxtBoxSearch.Text = "";
            dgContentList.SelectedItem = null;
            dgContentList.SelectedIndex = -1;
            selectedUID = -1;
            ShowHide();
            //SearchList.ItemsSource = null;
            Page_default p = new Page_default();
            FrmContent.Navigate(p);
        }

        private void TxtBoxSearch_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //if (TxtBoxSearch.Text.Length > 0)
            //    //SearchList.ItemsSource = searchliststrings;
            //    PopupSearchList.IsOpen = true;
        }

        private void SearchList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SearchList.SelectedItem != null)
            {
                //string s = SearchList.SelectedItem.ToString();
                //s = s.Substring(TxtBoxSearch.Text.Length);
                TxtBoxSearch.Text = SearchList.SelectedItem.ToString();
                PopupSearchList.IsOpen = false;
                //searchliststrings.Clear();
                //SearchList.ItemsSource = null;
            }
        }

        private void SearchList_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //PopupSearchList.IsOpen = false;
        }

        private void TxtBoxSearch_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //if(!TxtBoxSearch.IsMouseOver)
            //PopupSearchList.IsOpen = false;
        }

        #endregion

        #region Tags
        private void FillTags()
        {
            BtnAddTag.IsEnabled = false;
            TxtbxTagFilter.Text = "";
            using (var db = new CVDBContext())
            {
                var query = from tag in db.Tags
                            orderby tag.Tag_Name ascending
                            select tag.Tag_Name;
                LstBxTags.ItemsSource = query.ToList();
            }
        }

        private void TxtbxTagFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            List<string> filteredTags = new List<string>();
            BtnAddTag.IsEnabled = false;
            if (TxtbxTagFilter.Text.Length == 0)
            {
                FillTags();
                BtnAddTag.IsEnabled = false;
            }
            else
            {

                foreach (string s in LstBxTags.ItemsSource)
                {
                    if (s.ToLower().Contains(TxtbxTagFilter.Text.ToLower()))
                    {
                        filteredTags.Add(s);
                    }
                }
                LstBxTags.ItemsSource = filteredTags;
            }
            if (filteredTags.Count == 0)
            {
                BtnAddTag.IsEnabled = true;
            }
        }

        private void BtnAddTag_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new CVDBContext())
            {
                Tags t = new Tags { Tag_Name = TxtbxTagFilter.Text };
                db.Tags.Add(t);
                db.Entry(t).State = EntityState.Added;
                db.SaveChanges();
            }
            FillTags();
        }

        private void LstBxTags_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LstBxTags.SelectedItem != null)
            {
                string tmp = LstBxTags.SelectedItem.ToString();
                if (userTags.Contains(tmp))
                {
                    //delete from database
                    using (var db = new CVDBContext())
                    {
                        var i = from tag in db.Tags
                                where tag.Tag_Name == tmp
                                select tag.Tag_ID;
                        int id = i.FirstOrDefault();
                        var query = from rel in db.User_Tag_REL
                                    where rel.User_ID == selectedUID && rel.Tag_ID == id
                                    select rel;
                        User_Tag_REL toDelete = query.FirstOrDefault();
                        db.User_Tag_REL.Remove(toDelete);
                        db.SaveChanges();
                    }
                    LblTags.Content = FillLabelTags(selectedUID);
                    txtbxtest.Text = "deleted from database: " + tmp;
                }
                else
                {
                    //save to database
                    using (var db = new CVDBContext())
                    {
                        var i = from tag in db.Tags
                                where tag.Tag_Name == tmp
                                select tag.Tag_ID;
                        int id = i.FirstOrDefault();
                        User_Tag_REL toAdd = new User_Tag_REL { User_ID = selectedUID, Tag_ID = id };
                        db.User_Tag_REL.Add(toAdd);
                        db.Entry(toAdd).State = EntityState.Added;
                        db.SaveChanges();
                    }
                    LblTags.Content = FillLabelTags(selectedUID);
                    txtbxtest.Text = "saved to database: " + tmp;
                }
                Unselect();
            }
        }

        private void Unselect()
        {
            LstBxTags.UnselectAll();
        }


        List<string> userTags = new List<string>();
        private string FillLabelTags(int uid)
        {
            LblTags.Content = "";
            using (var db = new CVDBContext())
            {
                var query = from rel in db.User_Tag_REL
                            join tag in db.Tags on rel.Tag_ID equals tag.Tag_ID
                            where rel.User_ID == uid
                            orderby tag.Tag_Name
                            select tag.Tag_Name;
                userTags = query.ToList();
            }
            if (userTags.Count > 0)
            {
                return string.Join(",", userTags).CapitalizeFirst();
            }
            else
            {
                return "";
            }
        }

        private void LstBxTags_LostFocus(object sender, RoutedEventArgs e)
        {
            FillLabelTags(selectedUID);
        }

        private void imgTags_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            PopupTags.IsOpen = true;
        }

        private void ImgTags_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!IsMouseOver)
            {
                PopupTags.IsOpen = true;
            }
            else
            {
                PopupTags.IsOpen = false;
            }
        }

        #endregion

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            bool navigate = true;
            if (FrmContent.Content.GetType() == typeof(Page2))
            {
                Page2 p2 = (Page2)FrmContent.Content;
                if (await p2.Savep2())
                {
                    navigate = true;
                    if (isCreate)
                    {
                        LblPersonalInformation.Content = p2.TxtbxFirstName.Text.CapitalizeFirst() + " " + p2.TxtbxLastName.Text.CapitalizeFirst();
                        LblTags.Content = "";
                        selectedUID = p2.getUID();
                    }
                }
            }
            if (navigate)
            {
                b.Background = System.Windows.Media.Brushes.Green;
                isCreate = false;
                switch (b.Name)
                {
                    case "Btnp2":
                        Page2 p2 = new Page2(selectedUID);
                        FrmContent.Navigate(p2);
                        Btnp3.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFDDDDDD"));
                        Btnp4.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFDDDDDD"));
                        Btnp5.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFDDDDDD"));
                        Btnp2.Content = "Personal";
                        break;
                    case "Btnp3":
                        Page3 p3 = new Page3(selectedUID);
                        FrmContent.Navigate(p3);
                        Btnp2.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFDDDDDD"));
                        Btnp4.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFDDDDDD"));
                        Btnp5.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFDDDDDD"));
                        break;
                    case "Btnp4":
                        Page4 p4 = new Page4(selectedUID);
                        FrmContent.Navigate(p4);
                        Btnp2.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFDDDDDD"));
                        Btnp3.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFDDDDDD"));
                        Btnp5.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFDDDDDD"));
                        break;
                    case "Btnp5":
                        Page5 p5 = new Page5(selectedUID);
                        FrmContent.Navigate(p5);
                        Btnp2.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFDDDDDD"));
                        Btnp3.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFDDDDDD"));
                        Btnp4.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFDDDDDD"));
                        break;
                }
                dgContentList.UpdateGrid();
                ShowHide();
            }
        }
    }
}





