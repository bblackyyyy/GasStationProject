using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Globalization;
using System.Collections.Generic;
using System.Threading.Tasks;
using Stripe;
using Stripe.Checkout;
using WebApplication2.pdf_gen;
using static WebApplication2.DB_Connection;

namespace WebApplication2.Pages
{
    public class PaymentPageModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public int StationId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PumpId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Fuel { get; set; } = "";

        [BindProperty(SupportsGet = true)]
        public double Litres { get; set; }

        [BindProperty]
        public double Total { get; set; }

        private readonly DB_Connection _db;

        public PaymentPageModel()
        {
            
            StripeConfiguration.ApiKey = Environment.GetEnvironmentVariable("Stripe_secret_key");
           
            _db = new DB_Connection();
        }

        public async Task OnGetAsync()
        {
            
           

            
            if (Request.Cookies.TryGetValue("TotalPrice", out var raw) &&
                double.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed))
            {
                Total = parsed;
                Response.Cookies.Delete("TotalPrice");
                return;
            }

            
            var fuelKey = Fuel.Trim().ToLowerInvariant();
            double price, tax;
            switch (fuelKey)
            {
                case "diesel":
                    price = await _db.GetPrice("diesel");
                    tax   = await _db.GetTax("diesel");
                    break;
                case "e10":
                    price = await _db.GetPrice("e10");
                    tax   = await _db.GetTax("e10");
                    break;
                case "e5":
                    price = await _db.GetPrice("e5");
                    tax   = await _db.GetTax("e5");
                    break;
                case "98":
                    price = await _db.GetPrice("98");
                    tax   = await _db.GetTax("98");
                    break;
                default:
                    price = 0.0;
                    tax   = 0.0;
                    break;
            }

            Total = (price + tax) * Litres;
        }
        
        
        public async Task<IActionResult> OnPostCash()
        {
            if (Total <= 0)
            {
                ModelState.AddModelError(string.Empty, "Total price is missing.");
                return Page();
            }

            if (!Enum.TryParse<FuelType>(Fuel, ignoreCase: true, out var fuelEnum))
            {
                fuelEnum = FuelType.diesel;
            }

            
            PdfGen.Generate(
                stationId:    StationId,
                pumpId:       PumpId,
                fuelType:     fuelEnum,
                price:        _db.GetPrice(Fuel).Result,
                discount:     0f,
                totalLiters:  (float)Litres
            );

            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            string fileName  = $"E:\\WebApplication2\\WebApplication2\\receipt_{StationId}_{PumpId}_{timestamp}.pdf";

            
            if (!System.IO.File.Exists(fileName))
            {
                ModelState.AddModelError(string.Empty, "The receipt file could not be found.");
                return Page();
            }

            byte[] bytes = System.IO.File.ReadAllBytes(fileName);
            System.IO.File.Delete(fileName); 

            
            float remaining = await _db.SetAvailable(StationId, Fuel, (float)Litres);
            long transactionId = await _db.SetTransaction(StationId, Fuel, (float)Litres, (float)Total, DateTime.Now);

            
            return File(bytes, "application/pdf", $"receipt_{StationId}_{PumpId}_{timestamp}.pdf");
        }
        
        
        
    
        
        

    public async Task<IActionResult> OnPostCardAsync()
        {
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                Mode               = "payment",
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency   = "pln",
                            UnitAmount = (long)(Total * 100),
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "Fuel Payment",
                            },
                        },
                        Quantity = 1,
                    },
                },
                SuccessUrl = Url.Page("/Thanks",   null, null, Request.Scheme),
                CancelUrl  = Url.Page("/PaymentPage", null, null, Request.Scheme)
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options);
            float remaining = await _db.SetAvailable(StationId, Fuel, (float)Litres);
            long a = await _db.SetTransaction(StationId, Fuel, (float)Litres, (float)Total, DateTime.Now);
            return new JsonResult(new { sessionId = session.Id });
        }
    }
}
