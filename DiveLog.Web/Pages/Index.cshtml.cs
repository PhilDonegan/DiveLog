using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

            var diveProfiles = await FileHelpers.ProcessFormFile(DiveProfilesFile, ModelState);
            Console.WriteLine(diveProfiles);
            

            if (!ModelState.IsValid)
            {
                return Page();
            }

            ViewData["DiveProfiles"] = diveProfiles;
            return Page();
        }
    }
}
