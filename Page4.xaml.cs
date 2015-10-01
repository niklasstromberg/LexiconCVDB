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
    /// Interaction logic for Page4.xaml
    /// </summary>
    public partial class Page4 : Page
    {
        int uid;
        
        public Page4(int id)
        {
            InitializeComponent();
            uid = id;
        }

        private void BtnPage5_Click(object sender, RoutedEventArgs e)
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
                //pageFrame.Source = new Uri("Page5.xaml", UriKind.Relative);
            }
        }

        private void BtnPage3_Click(object sender, RoutedEventArgs e)
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
                Page3 p3 = new Page3(uid);
                pageFrame.Navigate(p3);
                //pageFrame.Source = new Uri("Page3.xaml", UriKind.Relative);
            }
        }

        private void BtnAddReference_Click(object sender, RoutedEventArgs e)
        {
            //if (!PopupAddRef.IsOpen)
            //{
            //    PopupAddRef.IsOpen = true;
            //}
            //else
            //{
            //    PopupAddRef.IsOpen = false;
            //}
            PopupAddRef.IsOpen = PopupAddRef.IsOpen == true ? false : true;
            // bygg en array eller JSON-objekt med ett reference-objekt i.
            // Spara till databasen, behåll REF_ID för referens till
            // när vi sparar employment history-posten
        }

        private void BtnAddRef_Click(object sender, RoutedEventArgs e)
        {
            // Spara reference till databasen och stäng popup
            PopupAddRef.IsOpen = false;
        }



    }
}
