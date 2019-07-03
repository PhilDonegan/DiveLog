using System.Linq;
using System.Threading.Tasks;
using DiveLog.Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DiveLog.Web.Pages
{
    public class LogViewerModel : PageModel
    {
        public async Task<IActionResult> OnGetDiveLogs()
        {
            var dives = await APIHelper.GetAllDives();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            ViewData["DiveLogs"] = dives.First();
            return Page();
        }
    }
}