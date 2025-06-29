using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication2;                

public class SecondCustomerPageModel : PageModel
{
    private readonly DB_Connection db;
    

    public SecondCustomerPageModel(DB_Connection db)   
    {
        this.db = db;
    }

    [BindProperty(SupportsGet = true)] public int StationId { get; set; }
    [BindProperty(SupportsGet = true)] public int? PumpId    { get; set; }
    [BindProperty(SupportsGet = true)] public string? Fuel   { get; set; }

    public float   AvailableLitres { get; private set; }
    public float PricePerLitre   { get; private set; }
    public double TaxPerLitre    { get; private set; }

    public async Task<IActionResult> OnGetAsync(int stationId, int pumpId, string fuel)
    {
                                    

        StationId = stationId;
        PumpId    = pumpId;
        Fuel      = fuel;

        if (StationId is { } sid && PumpId is { } pid)
        {
            HttpContext.Session.SetInt32("StationId", sid);
            HttpContext.Session.SetInt32("PumpId",  pid);
        }

        switch (Fuel?.ToLowerInvariant())
        {
            case "diesel":
                PricePerLitre = await db.GetPrice("diesel".Trim());  
                TaxPerLitre   = await db.GetTax("diesel".Trim());
                AvailableLitres = await db.GetAvailable(stationId,"diesel".Trim());
                 
                break;

            case "e10":
                PricePerLitre = await db.GetPrice("e10".Trim());
                TaxPerLitre   = await db.GetTax("e10".Trim());
                AvailableLitres = await db.GetAvailable(stationId,"e10".Trim());
                break;

            case "e5":
                PricePerLitre = await db.GetPrice("e5".Trim());
                TaxPerLitre   = await db.GetTax("e5".Trim());
                AvailableLitres = await db.GetAvailable(stationId,"e5".Trim());;
                break;

            case "98":
                PricePerLitre = await db.GetPrice("98".Trim());
                TaxPerLitre   = await db.GetTax("98".Trim());
                AvailableLitres = await db.GetAvailable(stationId,"98".Trim());;
                break;
        }

        return Page();
    }
}
