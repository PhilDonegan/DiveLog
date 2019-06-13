using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DiveLog.Web.Helpers
{
    public class FileHelpers
    {
        public static async Task<string> ProcessFormFile(IFormFile formFile, ModelStateDictionary modelState)
        {
            if (formFile.Length == 0)
            {
                modelState.AddModelError(formFile.FileName, "The file is empty.");
            }
            else
            {
                try
                {
                    using (var reader = new StreamReader(formFile.OpenReadStream(), new UTF8Encoding(false, true), true))
                    {
                        return await reader.ReadToEndAsync();                        
                    }
                }
                catch (Exception ex)
                {
                    modelState.AddModelError(formFile.FileName, $"The upload of {formFile.FileName} failed with error: {ex.GetBaseException().Message}");
                }
            }

            return string.Empty;
        }
    }
}
