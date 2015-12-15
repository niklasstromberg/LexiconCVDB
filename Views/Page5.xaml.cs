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

namespace XBAPLexiconCVDBInterface.Views
{
    /// <summary>
    /// Interaction logic for Page5.xaml
    /// </summary>
    public partial class Page5 : Page
    {
        int uid;
        public List<string> skillsList = new List<string>();   // list of the chosen skills for the viewed person
        public List<string> tmpList = new List<string> { "c#", "d#", "dyslexia" }; 
        public string skillString = "";
        
        public Page5(int id)
        {
            InitializeComponent();
            uid = id;
            // populate skillsList from database
            PopulateLabel();
            DataContext = skillsList;
            skills.ItemsSource = tmpList;

        }
        private void PopulateLabel()
        {
            LblSkills.Content = "";
            skillString = "";
            foreach (string str in skillsList)
            {
                skillString += str + ", ";
            }
            LblSkills.Content = "Skills (" + skillsList.Count + "): " + skillString;
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
            if (skillsList.Contains(btn.Content))
            {
                skillsList.Remove(btn.Content.ToString());
                PopulateLabel();
            }
            else
            {
                skillsList.Add(btn.Content.ToString());
                PopulateLabel();
            }
        }


    }

    // funkar inte, men visar vad jag vill göra
    public partial class skillButton : Button
    {
        private int margin = 7;

        private void skillButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            btn.Background = btn.Background == Brushes.LightGreen ? (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFDDDDDD")) : Brushes.LightGreen;
        }
    }
}
