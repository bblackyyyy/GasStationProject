using System;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace WebApplication2.pdf_gen
{
    

    public static class PdfGen
    {
        public static void Generate(int stationId, int pumpId, FuelType fuelType, float price, float discount, float totalLiters)
        {
            var now = DateTime.Now;
            var timestamp = now.ToString("yyyyMMddHHmmss");
            var fileName = $"receipt_{stationId}_{pumpId}_{timestamp}.pdf";

            
            float subtotal = price * totalLiters;
            float discountAmount = subtotal * discount / 100f;
            float total = subtotal - discountAmount;

            
            string timePart = now.ToString("HHmm");
            string fuelInitial = fuelType.ToString()[0].ToString().ToUpper();
            string operationId = $"{stationId}_{pumpId}_{fuelInitial}_{timePart}";

            
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(40);
                    page.Size(PageSizes.A4);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    
                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Height(25).Background(Colors.Red.Medium);
                    });

                    page.Content().Column(col =>
                    {
                        col.Spacing(10);

                        
                        col.Item().Text("Fuel Receipt").Bold().FontSize(18).AlignCenter();

                        
                        col.Item().Text($"Operation ID: {operationId}").Bold();

                        
                        col.Item().Row(info =>
                        {
                            info.RelativeItem().Column(c =>
                            {
                                c.Item().Text("Station ID:").SemiBold();
                                c.Item().Text(stationId.ToString());
                                c.Item().Text("Pump ID:").SemiBold();
                                c.Item().Text(pumpId.ToString());
                            });

                            info.RelativeItem().Column(c =>
                            {
                                c.Item().Text("Date:").SemiBold();
                                c.Item().Text(now.ToString("yyyy-MM-dd HH:mm"));
                                c.Item().Text("Fuel Type:").SemiBold();
                                c.Item().Text(fuelType.ToString());
                            });
                        });

                        col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                        
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.ConstantColumn(120);
                            });

                            void Row(string label, string value)
                            {
                                table.Cell().Text(label);
                                table.Cell().Text(value).AlignRight();
                            }

                            Row("Unit Price:", $"{price:F2}zł. / L");
                            Row("Total Liters:", $"{totalLiters:F2} L");
                            Row("Tax:", $"{subtotal:F2}zł.");
                            Row("Total:", $"{total:F2}zł.");
                            Row("Payment Method:", "Cash"); 
                        });

                        col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                        
                        col.Item().AlignCenter().Text("Thanks for your purchase!").Italic();
                    });

                    
                    page.Footer().Row(row =>
                    {
                        row.RelativeItem().Height(25).Background(Colors.Red.Medium);
                    });
                });
            });

            document.GeneratePdf(fileName);
            Console.WriteLine($"PDF generated at: {fileName}");
        }
    }
}
