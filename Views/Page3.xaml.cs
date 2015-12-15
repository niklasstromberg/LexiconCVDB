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
    /// Interaction logic for Page3.xaml
    /// </summary>
    public partial class Page3 : Page
    {
        int uid;

        public Page3(int id)
        {
            InitializeComponent();
            uid = id;
            //List<Education> educations = new List<Education>();
            //GrdEdu.ItemsSource = educations;
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
    }
}
