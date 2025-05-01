// File: Pages/second_customer_page.cshtml.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplication2.Pages
{
    public class SecondCustomerPageModel : PageModel
    {
        /* ───────────── route/query data ───────────── */
        [BindProperty(SupportsGet = true)] public int?    StationId { get; set; }
        [BindProperty(SupportsGet = true)] public int?    PumpId    { get; set; }
        [BindProperty(SupportsGet = true)] public string? Fuel      { get; set; }

        /* ───────────── exposed to Razor ───────────── */
        public int    AvailableLitres { get; private set; }
        public double PricePerLitre   { get; private set; }
        public double TaxPerLitre     { get; private set; }

        public IActionResult OnGet(int? stationId, int? pumpId, string? fuel)
        {
            /* sync bound + route */
            StationId ??= stationId;
            PumpId    ??= pumpId;
            Fuel      ??= fuel;

            /* save IDs for layout */
            if (StationId is { } sid && PumpId is { } pid)
            {
                HttpContext.Session.SetInt32("StationId", sid);
                HttpContext.Session.SetInt32("PumpId",    pid);
            }

            /* ⇢ static price & tax per fuel type */
            switch (Fuel?.ToLowerInvariant())
            {
                case "diesel":
                    PricePerLitre = 6.00;
                    TaxPerLitre   = 1.20;
                    AvailableLitres = 1000;
                    break;

                case "e10":
                    PricePerLitre = 5.50;
                    TaxPerLitre   = 1.00;
                    AvailableLitres = 1;
                    break;

                case "e5":
                    PricePerLitre = 5.80;
                    TaxPerLitre   = 1.05;
                    AvailableLitres = 6;
                    break;

                case "98":
                    PricePerLitre = 6.50;
                    TaxPerLitre   = 1.30;
                    AvailableLitres = 100;
                    break;

                default:
                    PricePerLitre = 6.00;
                    TaxPerLitre   = 1.20;
                    AvailableLitres = 1000;
                    break;
            }

            return Page();
        }
    }
}
