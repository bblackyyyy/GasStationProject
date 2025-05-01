using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Globalization;
using WebApplication2.pdf_gen;   
using System.IO;

namespace WebApplication2.Pages
{
    public class PaymentPageModel : PageModel
    {
        
        [BindProperty(SupportsGet = true)] public int    StationId { get; set; }
        [BindProperty(SupportsGet = true)] public int    PumpId    { get; set; }
        [BindProperty(SupportsGet = true)] public string Fuel      { get; set; } = "";
        [BindProperty(SupportsGet = true)] public double Litres    { get; set; }

        
        [BindProperty] public double Total { get; set; }

        public void OnGet()
        {
            
            if (Request.Cookies.TryGetValue("TotalPrice", out var raw) &&
                double.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed))
            {
                Total = parsed;
                Response.Cookies.Delete("TotalPrice");
            }
            else
            {
                (double price, double tax) = Fuel.ToLowerInvariant() switch
                {
                    "diesel" => (6.00, 1.20),
                    "e10"    => (5.50, 1.00),
                    "e5"     => (5.80, 1.05),
                    "98"     => (6.50, 1.30),
                    _        => (6.00, 1.20)
                };
                Total = (price + tax) * Litres;
            }
        }

        
        public IActionResult OnPostCash()
        {
            if (Total <= 0)
            {
                ModelState.AddModelError("", "Total price is missing.");
                return Page();
            }

         
            if (!Enum.TryParse<FuelType>(Fuel, ignoreCase: true, out var fuelEnum))
            {
                fuelEnum = FuelType.Diesel; 
            }

           
            PdfGen.Generate(StationId, PumpId, fuelEnum,
                            price: (float)(Total / Litres),   
                            discount: 0f,
                            totalLiters: (float)Litres);

            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            string fileName  = $"receipt_{StationId}_{PumpId}_{timestamp}.pdf";

            byte[] bytes = System.IO.File.ReadAllBytes(fileName);

            
            System.IO.File.Delete(fileName);

            return File(bytes, "application/pdf", fileName);
        }
    }
}
