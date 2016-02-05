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
    /// Interaction logic for Page3.xaml
    /// </summary>
    public partial class Page3 : Page
    {
        int uid;

        public Page3(int id)
        {
            InitializeComponent();
            uid = id;

            FillGrdEdu();
            BtnDeleteEdu.IsEnabled = false;
            ClearFields();
        }

        private void BtnPage2_Click(object sender, RoutedEventArgs e)
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
                Page2 p2 = new Page2(uid);
                pageFrame.Navigate(p2);
                //pageFrame.Source = new Uri("Page2.xaml", UriKind.Relative);
            }
        }

        private void BtnPage4_Click(object sender, RoutedEventArgs e)
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
                Page4 p4 = new Page4(uid);
                pageFrame.Navigate(p4);
                //pageFrame.Source = new Uri("Page4.xaml", UriKind.Relative);
            }
        }

        public void FillGrdEdu()
        {
            try
            {
                using (var db = new CVDBContext())
                {
                    var query = from rel in db.User_EDU_REL
                                join e in db.Educations on rel.EDU_ID equals e.EDU_ID
                                where rel.User_ID == uid
                                select new { e.EDU_ID, e.School, e.Course, e.Degree, e.Year };
                    GrdEdu.ItemsSource = query.ToList();
                }
                GrdEdu.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ex.InnerException, "fill grid error");
            }
        }

        int eduID;
        Educations eduToAdd;
        private async Task<int> AddEdu()
        {
            try
            {
                int tmp = Convert.ToInt16(TxtbxYear.Text);

                using (var db = new CVDBContext())
                {
                    var query = from edu in db.Educations
                                where edu.EDU_ID == IDtoshow
                                select edu;

                    if (query.Count() < 1)
                    {
                        eduToAdd = new Educations
                        {
                            Course = TxtbxCourse.Text,
                            Degree = TxtbxDegree.Text,
                            Year = Convert.ToInt16(TxtbxYear.Text),
                            Notes = TxtbxNotes.Text,
                            School = TxtbxSchool.Text
                        };
                        db.Educations.Add(eduToAdd);
                        db.Entry(eduToAdd).State = EntityState.Added;
                        await db.SaveChangesAsync();
                        eduID = eduToAdd.GetEduID();
                    }
                    else
                    {
                        eduToAdd = query.FirstOrDefault();
                        eduToAdd.Course = TxtbxCourse.Text;
                        eduToAdd.Degree = TxtbxDegree.Text;
                        eduToAdd.Year = Convert.ToInt16(TxtbxYear.Text);
                        eduToAdd.Notes = TxtbxNotes.Text;
                        eduToAdd.School = TxtbxSchool.Text;
                        eduID = eduToAdd.EDU_ID;
                        db.Entry(eduToAdd).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                return eduID;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ex.InnerException, "add education error");
                return 0;
            }
        }

        private async void BtnAddEdu_Click(object sender, RoutedEventArgs e)
        {
            int tmp = 0;
            try
            {
                tmp = await AddEdu();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ex.InnerException, "save education error");
            }
            //eduID = eduToAdd.EDU_ID;
            //eduID = eduToAdd.GetEduID();
            try
            {
                using (var db = new CVDBContext())
                {
                    var query = from rel in db.User_EDU_REL
                                where rel.User_ID == uid && rel.EDU_ID == tmp
                                select rel.User_EDU_ID;
                    int i = query.FirstOrDefault();
                    User_EDU_REL uer = db.User_EDU_REL.Find(i);
                    if (uer == null)
                    {
                        uer = new User_EDU_REL
                        {
                            User_ID = uid,
                            EDU_ID = tmp
                        };
                        db.User_EDU_REL.Add(uer);
                        db.Entry(uer).State = EntityState.Added;
                    }
                    else
                    {
                        db.Entry(uer).State = EntityState.Modified;
                    }
                    db.SaveChanges();
                    FillGrdEdu();
                    ClearFields();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ex.InnerException, "save user edu relationship error");
            }
        }

        private void TxtbxYear_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TxtbxYear.Text.Length == 4)
            {
                if (TxtbxYear.Text.StringIsInt())
                {
                    if (Convert.ToInt32(TxtbxYear.Text) <= DateTime.Now.Year)
                    {
                        TxtbxYear.BorderBrush = Brushes.Green;
                        BtnAddEdu.IsEnabled = true;
                    }
                }
            }
            else
            {
                TxtbxYear.BorderBrush = Brushes.Red;
                BtnAddEdu.IsEnabled = false;
            }
        }

        int IDtoshow;
        private void GrdEdu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                BtnDeleteEdu.IsEnabled = false;
                if (GrdEdu.SelectedIndex >= 0)
                {
                    var obj = GrdEdu.SelectedItem;
                    System.Type type = obj.GetType();
                    IDtoshow = (int)type.GetProperty("EDU_ID").GetValue(obj, null);
                    using (var db = new CVDBContext())
                    {
                        Educations education = db.Educations.Find(IDtoshow);
                        TxtbxCourse.Text = education.Course;
                        TxtbxDegree.Text = education.Degree;
                        TxtbxNotes.Text = education.Notes;
                        TxtbxSchool.Text = education.School;
                        TxtbxYear.Text = education.Year.ToString();
                    }
                    BtnDeleteEdu.IsEnabled = true;
                    BtnAddEdu.Content = "Edit";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ex.InnerException, "selection changed error");
            }
        }

        private void BtnDeleteEdu_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var db = new CVDBContext())
                {
                    var query = from rel in db.User_EDU_REL
                                where rel.User_ID == uid && rel.EDU_ID == IDtoshow
                                select rel;
                    User_EDU_REL uerToDelete = query.FirstOrDefault();
                    db.User_EDU_REL.Remove(uerToDelete);
                    db.SaveChanges();


                    //Educations eduToDelete = db.Educations.Find(IDtoshow);
                    //db.Educations.Remove(eduToDelete);
                    //db.SaveChanges();
                    FillGrdEdu();
                    ClearFields();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ex.InnerException, "delete user edu relation error");
            }
        }

        private void ClearFields()
        {
            TxtbxCourse.Text = "";
            TxtbxDegree.Text = "";
            TxtbxNotes.Text = "";
            TxtbxSchool.Text = "";
            TxtbxYear.Text = "";
            BtnAddEdu.IsEnabled = false;
            BtnAddEdu.Content = "Add";
        }
    }
}
