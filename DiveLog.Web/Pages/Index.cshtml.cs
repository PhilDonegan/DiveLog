using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiveLog.Parsers;
using DiveLog.Web.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DiveLog.Web.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public IFormFile DiveProfilesFile { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var data = await FileHelpers.ProcessFormFile(DiveProfilesFile, ModelState);
            var test = new Shearwater();
            test.ProcessDives(data);
            Console.WriteLine(data);
            

            if (!ModelState.IsValid)
            {
                return Page();
            }

            ViewData["DiveProfiles"] = data;
            return Page();
        }
    }
}
