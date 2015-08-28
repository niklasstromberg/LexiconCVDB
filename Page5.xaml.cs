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

namespace XBAPLexiconCVDBInterface
{
    /// <summary>
    /// Interaction logic for Page5.xaml
    /// </summary>
    public partial class Page5 : Page
    {
        public List<string> skillsList = new List<string> { "c#", "d#", "dyslexia" };   // list of the chosen skills for the viewed person
        public string skillString = "";
        
        public Page5()
        {
            InitializeComponent();

            // populate skillsArray from database
            populateLabel();

        }
        private void populateLabel()
        {
            LblSkills.Content = "";
            foreach (string str in skillsList)
            {
                skillString += str + ", ";
            }
            LblSkills.Content = "Skills ("+ skillsList.Count + "): " + skillString;
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
                pageFrame.Source = new Uri("Page4.xaml", UriKind.Relative);
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
            //Button btn = (Button)sender;
            BtnTest.Background = Brushes.LightSeaGreen;
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
