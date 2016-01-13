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

        int eduID;
        Educations eduToAdd;
        private async Task<int> AddEdu()
        {
            int tmp = Convert.ToInt16(TxtbxYear.Text);
            using (var db = new CVDBContext())
            {
                var query = from edu in db.Educations
                            where edu.Course == TxtbxCourse.Text && edu.Degree == TxtbxDegree.Text && edu.School == TxtbxSchool.Text && edu.Year == tmp && edu.Notes == TxtbxNotes.Text
                            select edu;

                eduToAdd = new Educations
                {
                    Course = TxtbxCourse.Text,
                    Degree = TxtbxDegree.Text,
                    Year = Convert.ToInt16(TxtbxYear.Text),
                    Notes = TxtbxNotes.Text,
                    School = TxtbxSchool.Text
                };
                if (query.Count() < 1)
                {
                    db.Educations.Add(eduToAdd);
                    db.Entry(eduToAdd).State = EntityState.Added;
                    await db.SaveChangesAsync();
                    eduID = eduToAdd.GetEduID();
                }
                else
                {
                    eduID = query.FirstOrDefault().EDU_ID;
                }
            }
            return eduID;
        }

        private async void BtnAddEdu_Click(object sender, RoutedEventArgs e)
        {
            int tmp = await AddEdu();
            //eduID = eduToAdd.EDU_ID;
            //eduID = eduToAdd.GetEduID();
            using (var db = new CVDBContext())
            {
                User_EDU_REL uer = new User_EDU_REL
                {
                    User_ID = uid,
                    EDU_ID = tmp
                };
                db.User_EDU_REL.Add(uer);
                db.Entry(uer).State = EntityState.Added;
                db.SaveChanges();
                FillGrdEdu();
                ClearFields();
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
            }
        }

        private void BtnDeleteEdu_Click(object sender, RoutedEventArgs e)
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

        private void ClearFields()
        {
            TxtbxCourse.Text = "";
            TxtbxDegree.Text = "";
            TxtbxNotes.Text = "";
            TxtbxSchool.Text = "";
            TxtbxYear.Text = "";
        }
    }
}
