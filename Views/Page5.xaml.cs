using System;
using System.Collections;
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

namespace XBAPLexiconCVDBInterface.Views
{
    /// <summary>
    /// Interaction logic for Page5.xaml
    /// </summary>
    public partial class Page5 : Page
    {
        int uid;
        public List<string> skillsList = new List<string>();   // list of the chosen skills for the viewed person
        public List<string> tmpList = new List<string>();
        public string skillString = "";

        public Page5(int id)
        {
            InitializeComponent();
            uid = id;
            FillList();
            PopulateLabel();
            DataContext = skillsList;
            skills.ItemsSource = tmpList;
            TxtbxAddSkill.Visibility = Visibility.Hidden;
            this.Loaded += Page5_Loaded;
        }

        void Page5_Loaded(object sender, RoutedEventArgs e)
        {
            ButtonColours();
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        private void ButtonColours()
        {
            foreach (Button b in FindVisualChildren<Button>(SkillGrid))
            {
                if (skillsList.Contains(b.Content))
                {
                    b.Background = Brushes.LightGreen;
                }
            }
        }

        private void FillList()
        {
            using (var db = new CVDBContext())
            {
                var query = from s in db.Skills
                            orderby s.Skill_Name
                            select s;

                foreach (var s in query)
                {
                    tmpList.Add(s.Skill_Name);
                }
            }
        }

        private void PopulateLabel()
        {
            FillSkillsList();
            LblSkills.Content = "";
            skillString = "";
            skillString = string.Join(",", skillsList);
            LblSkills.Content = "Skills (" + skillsList.Count + "): " + skillString;
        }

        private void FillSkillsList()
        {
            skillsList.Clear();
            using (var db = new CVDBContext())
            {
                var query = from rel in db.User_Skill_REL
                            join skill in db.Skills on rel.Skill_ID equals skill.Skill_ID
                            where rel.User_ID == uid
                            select skill;
                foreach (var v in query)
                {
                    skillsList.Add(v.Skill_Name);
                }
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

        private void BtnPage6_Click(object sender, RoutedEventArgs e)
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
                pageFrame.Source = new Uri("Page6.xaml", UriKind.Relative);
            }
        }

        private void BtnTest_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            btn.Background = btn.Background == Brushes.LightGreen ? (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFDDDDDD")) : Brushes.LightGreen;
            using (var db = new CVDBContext())
            {
                var query = from s in db.Skills
                            where s.Skill_Name == btn.Content.ToString()
                            select s;
                Skills skill = query.FirstOrDefault();
                User_Skill_REL s_rel = new User_Skill_REL
                {
                    User_ID = uid,
                    Skill_ID = skill.Skill_ID
                };
                var q = from rel in db.User_Skill_REL
                        where rel.Skill_ID == s_rel.Skill_ID &&
                              rel.User_ID == uid
                        select rel;
                if (q.Count() != 1)
                {
                    db.User_Skill_REL.Add(s_rel);
                    db.Entry(s_rel).State = EntityState.Added;
                    db.SaveChanges();
                }
                else
                {
                    User_Skill_REL relToDelete = q.FirstOrDefault();
                    db.User_Skill_REL.Remove(relToDelete);
                    db.SaveChanges();
                }
                FillSkillsList();
                PopulateLabel();
            }
        }

        bool isClicked = false;
        private void BtnAddSkill_Click(object sender, RoutedEventArgs e)
        {
            if (isClicked)
            {
                isClicked = false;
                BtnAddSkill.Content = "Add Skill";
                if (TxtbxAddSkill.Text.Length > 0)
                {
                    using (var db = new CVDBContext())
                    {
                        Skills skillToAdd = new Skills
                        {
                            Skill_Name = TxtbxAddSkill.Text
                        };
                        db.Skills.Add(skillToAdd);
                        db.Entry(skillToAdd).State = EntityState.Added;
                        db.SaveChanges();
                    }
                }
                TxtbxAddSkill.Visibility = Visibility.Hidden;
                Reload();
            }
            else
            {
                BtnAddSkill.Content = "Save";
                isClicked = true;
                TxtbxAddSkill.Text = "";
                TxtbxAddSkill.Visibility = Visibility.Visible;
            }
        }

        private void Reload()
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
                Page5 p5 = new Page5(uid);
                pageFrame.Navigate(p5);
                //pageFrame.Source = new Uri("Page4.xaml", UriKind.Relative);
            }
        }
    }
}
