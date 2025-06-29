

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplication2.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public int StationId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PumpId { get; set; }

        public void OnGet() { }
    }
}