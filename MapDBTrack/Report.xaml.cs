using Newtonsoft.Json;
using System.IO;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MapDBTrack
{
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
            List<Place> places = new List<Place>();


            for(int i = 0; i < MainWindow.customer.Children.Count; i++)
            {
                Border border = MainWindow.customer.Children[i] as Border;
                StackPanel stackPanel = border.Child as StackPanel;
                TextBlock textBlock = stackPanel.Children[0] as TextBlock;
                Place p1 = MainWindow.places.Where(x => x.customer_id == textBlock.Text).FirstOrDefault();
                places.Add(p1);
            }


            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string json = JsonConvert.SerializeObject(places);
            File.WriteAllText(Path.Combine(desktop,"Report_Json"), json);
            MessageBox.Show("The file has been saved", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
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
