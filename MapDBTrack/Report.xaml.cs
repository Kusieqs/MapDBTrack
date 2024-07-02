using Newtonsoft.Json;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System.IO;
using System.Numerics;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using OfficeOpenXml;
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
            MessageBox.Show("The file has been saved", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        } // Creating PDF file 
        private void ExcelClick(object sender, RoutedEventArgs e)
        {
            List<Place> places = ListCreate();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(new FileInfo(Path.Combine(desktop,"Report_excel.xlsx"))))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Products");

                worksheet.Cells[1, 1].Value = "Id";
                worksheet.Cells[1, 2].Value = "First name";
                worksheet.Cells[1, 3].Value = "Last name";
                worksheet.Cells[1, 4].Value = "Number";
                worksheet.Cells[1, 5].Value = "Email";
                worksheet.Cells[1, 6].Value = "City";
                worksheet.Cells[1, 7].Value = "Postal code";
                worksheet.Cells[1, 8].Value = "Province";
                worksheet.Cells[1, 9].Value = "Street";

                var range = worksheet.Cells["A1:I1"];
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(OfficeOpenXml.Style.ExcelIndexedColor.Indexed10);
                range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                range.Style.Border.Bottom.Color.SetColor(OfficeOpenXml.Drawing.eThemeSchemeColor.Accent3);

                for (int i = 0; i < places.Count; i++)
                {
                    worksheet.Cells[i + 3, 1].Value = places[i].customer_id;
                    worksheet.Cells[i + 3, 2].Value = places[i].first_name;
                    worksheet.Cells[i + 3, 3].Value = places[i].last_name;
                    worksheet.Cells[i + 3, 4].Value = places[i].contact_number;
                    worksheet.Cells[i + 3, 5].Value = places[i].email;
                    worksheet.Cells[i + 3, 6].Value = places[i].city;
                    worksheet.Cells[i + 3, 7].Value = places[i].postal_code;
                    worksheet.Cells[i + 3, 8].Value = places[i].province;
                    worksheet.Cells[i + 3, 9].Value = places[i].street;
                }
                worksheet.Cells.AutoFitColumns();
                package.Save();

            }
            MessageBox.Show("The file has been saved", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
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
                Place p1 = MainWindow.listOfData.Where(x => x.customer_id == textBlock.Text).FirstOrDefault();
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
