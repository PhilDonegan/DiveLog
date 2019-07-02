using DiveLog.DTO;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
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

        public async static Task<bool> UploadDivesToAPI(List<LogEntryDTO> dives)
        {
            // TODO: Remove this an upload all the dives.
            var test = dives.Take(10).ToList();
            var json = JsonConvert.SerializeObject(test);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            var response = await _client.PostAsync("api/LogEntries", stringContent);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            return false;
        }
    }
}
