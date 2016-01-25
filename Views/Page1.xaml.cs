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
            FillGrid();
        }

        private void FillGrid()
        {
            using(var db = new CVDBContext())
            {
                var query = from u in db.Users
                            join ud in db.User_Details on u.User_ID equals ud.User_ID
                            orderby u.Last_Name, u.First_Name, ud.Available, ud.Available_Date
                            select new { u.User_ID, u.Drivers_Licence, u.First_Name, u.Last_Name, ud.Available, ud.Available_Date };
                dgContentList.ItemsSource = query.ToList();
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
            if (dgContentList.SelectedIndex >= 0)
            {
                var obj = dgContentList.SelectedItem;
                System.Type type = obj.GetType();
                selectedUID = (int)type.GetProperty("User_ID").GetValue(obj, null);
                FillDetails(selectedUID);
                string first = (string)type.GetProperty("First_Name").GetValue(obj, null);
                string last = (string)type.GetProperty("Last_Name").GetValue(obj, null);
                LblPersonalInformation.Content = first.CapitalizeFirst() + " " + last.CapitalizeFirst();
                txtbxtest.Text = selectedUID.ToString();
                GetImage();
                ShowHide();
                Page2 p2 = new Page2(selectedUID);
                FrmContent.Navigate(p2);
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
            PopupLog.IsOpen = false;
            PopupEditLog.IsOpen = false;
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
            //DGJournals.Items.Clear();
            FillJournals();

            //using (var db = new CVDBContext())
            //{
            //    var query = from j in db.Journals
            //                where j.User_ID == selectedUID
            //                select new { Journal_ID = j.Journal_ID, Created = j.Created, Notes = j.Notes };
            //    var tmp = query.ToList();
            //    DGJournals.ItemsSource = tmp;
            //    //foreach (var v in tmp)
            //    //{
            //    //    v.Created = v.Created.Date;
            //    //    v.Notes = v.Notes.GetFirst25() + "...";
            //    //}
            //    //var query2 = from j2 in tmp
            //    //             select new { Journal_ID = j2.Journal_ID, Date = j2.Created.Date, Journal = j2.Notes.GetFirst25() };
            //    //var results = query2.ToList();
            //    //DGJournals.ItemsSource = results;
            //}
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
            //DGLogs.SelectedIndex = -1;
            //DGLogs.SelectedItem = null;
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

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            using(var db = new CVDBContext())
            {
                var query = from user in db.Users
                            join ud in db.User_Details on user.User_ID equals ud.User_ID
                            where user.First_Name == TxtBoxSearch.Text ||
                            user.Last_Name == TxtBoxSearch.Text
                            orderby user.Last_Name, user.First_Name, ud.Available, ud.Available_Date
                            select new { user.User_ID, user.First_Name, user.Last_Name, ud.Available, ud.Available_Date };
                dgContentList.ItemsSource = query.ToList();
            }
        }

        private void TxtBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtbxtest.Text = "";
            if (TxtBoxSearch.Text.Length == 0)
            {
                FillGrid();
            }
            else
            {
                var rows = GetDataGridRows(dgContentList);
                List<DataGridRow> filteredList = new List<DataGridRow>();

                foreach (DataGridRow r in rows)
                {
                    //r.Visibility = Visibility.Hidden;
                    foreach (DataGridColumn column in dgContentList.Columns)
                    {
                        if (column.GetCellContent(r) is TextBlock)
                        {
                            TextBlock cellContent = column.GetCellContent(r) as TextBlock;
                            string content = cellContent.Text.ToLower();
                            if (content.Contains(TxtBoxSearch.Text.ToLower()))
                            {
                                filteredList.Add(r);
                                //r.Visibility = Visibility.Visible;
                                txtbxtest.Text += r.Item.ToString();
                            }
                        }
                    }
                }
                dgContentList.ItemsSource = filteredList;
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
            //FillGrid();
            TxtBoxSearch.Text = "";
        }
    }
}




