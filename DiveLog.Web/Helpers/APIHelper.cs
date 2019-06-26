using DiveLog.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DiveLog.Web.Helpers
{
    public class APIHelper
    {
        private static HttpClient _client = new HttpClient();

        static APIHelper()
        {
            _client.BaseAddress = new Uri("https://divelogapi20190611082926.azurewebsites.net");
            _client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async static Task<bool> UploadDivesToAPI(IEnumerable<LogEntryDTO> dives)
        {
            var response = await _client.GetAsync("api/LogEntries");
            if (response.IsSuccessStatusCode)
            {
                var entries = await response.Content.ReadAsStringAsync();
                return true;
            }

            return false;
        }
    }
}
