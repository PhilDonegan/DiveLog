using DiveLog.DTO;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace DiveLog.Web.Helpers
{
    public class APIHelper
    {
        private static HttpClient _client = new HttpClient();

        static APIHelper()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile("appsettings.Development.json", true, true)
                .Build();

            var divelogAPIUri = builder.GetSection("DiveLogAPIUri").Value;
            _client.BaseAddress = new Uri(divelogAPIUri);
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
