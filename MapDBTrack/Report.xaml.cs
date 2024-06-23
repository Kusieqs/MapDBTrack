using Newtonsoft.Json;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System.IO;
using System.Numerics;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MapDBTrack
{
    public partial class Report : Window
    {
        string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        public Report()
        {
            InitializeComponent();
        }
        private void PdfClick(object sender, RoutedEventArgs e)
        {
            List<Place> places = ListCreate();

            PdfDocument pdf = new PdfDocument();
            PdfPage page = pdf.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XFont font = new XFont("Calibri", 12, XFontStyle.Regular);
            int y = 30;
            int x = 10;
            int lineHeight = 24;

            foreach (var item in places)
            {
                if(y + lineHeight > page.Height.Point - 25)
                {
                    page = pdf.AddPage();
                    gfx = XGraphics.FromPdfPage(page);
                    y = 30;
                }

                for(int i = 0; i < 9; i++)
                {
                    string content = "";
                    switch (i)
                    {
                        case 0:
                            content = $"Id: {item.customer_id}";
                            break;
                        case 1:
                            content = $"First Name: {item.first_name}";
                            break;
                        case 2:
                            content = $"Last Name: {item.last_name}";
                            break;
                        case 3:
                            content = $"Number: {item.contact_number}";
                            break;
                        case 4:
                            content = $"Email: {item.email}";
                            break;
                        case 5:
                            content = $"City: {item.postal_code}";
                            break;
                        case 6:
                            content = $"Postal Code: {item.postal_code}";
                            break;
                        case 7:
                            content = $"Province: {item.province}";
                            break;
                        case 8:
                            content = $"Street: {item.street}";
                            break;
                    }
                    gfx.DrawString(content, font, XBrushes.Black, x, y);
                    y += lineHeight;
                }
                gfx.DrawString(" ", font, XBrushes.Black, x, y);
                y += lineHeight;
                gfx.DrawString(" ", font, XBrushes.Black, x, y);
                y += lineHeight;
            }
            pdf.Save(Path.Combine(desktop, "Report_PDF.pdf"));
        } // Creating PDF file 
        private void ExcelClick(object sender, RoutedEventArgs e)
        {

        }
        private void JsonClick(object sender, RoutedEventArgs e)
        {
            List<Place> places = ListCreate();
            string json = JsonConvert.SerializeObject(places);
            File.WriteAllText(Path.Combine(desktop,"Report_Json.json"), json);
            MessageBox.Show("The file has been saved", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        } // Creating json file

        private List<Place> ListCreate()
        {
            List<Place> places = new List<Place>();
            for (int i = 0; i < MainWindow.customer.Children.Count; i++)
            {
                Border border = MainWindow.customer.Children[i] as Border;
                StackPanel stackPanel = border.Child as StackPanel;
                TextBlock textBlock = stackPanel.Children[0] as TextBlock;
                Place p1 = MainWindow.places.Where(x => x.customer_id == textBlock.Text).FirstOrDefault();
                places.Add(p1);
            }
            return places;
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
