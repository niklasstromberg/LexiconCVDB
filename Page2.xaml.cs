using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
using System.Configuration;

namespace XBAPLexiconCVDBInterface
{
    /// <summary>
    /// Interaction logic for Page2.xaml
    /// </summary>
    public partial class Page2 : Page
    {
        public Page2()
        {
            InitializeComponent();
            // Här behöver vi hämta informationen som ska in i fälten ifrån databasen, så att vår page i frmContent
            // alltid visar aktuell information
            

            
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
                // This method also needs to save the information in the fields of page2
                // to the database
                pageFrame.Source = new Uri("Page3.xaml", UriKind.Relative);
            }
        }

        private void txtbxPersonalInfo_TextChanged(object sender, TextChangedEventArgs e)
        {
            LblCounter.Content = "0 out of 1024";
            TextBox txtbx = (TextBox)sender;
            //var textBox = sender as TextBox;
            int count = txtbx.Text.Length;
            string content = count + " out of 1024";
            LblCounter.Content = content.ToString();
            
        }
    }
}
