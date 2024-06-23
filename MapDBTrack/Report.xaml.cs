using System.Windows;
using System.Windows.Input;

namespace MapDBTrack
{
    /// <summary>
    /// Logika interakcji dla klasy Report.xaml
    /// </summary>
    public partial class Report : Window
    {
        public Report()
        {
            InitializeComponent();
        }
        private void PdfClick(object sender, RoutedEventArgs e)
        {

        }
        private void ExcelClick(object sender, RoutedEventArgs e)
        {

        }
        private void JsonClick(object sender, RoutedEventArgs e)
        {

        }


        private void ExitClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        } // exit method
        private void BorderClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        } // feature to moving window

    }
}
