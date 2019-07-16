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

namespace DiveLog.GUI.Helpers
{
    public class APIHelper : IDisposable
    {
        private static HttpClient _client = new HttpClient();

        public APIHelper(IConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var divelogAPIUri = configuration.GetSection("DiveLogAPIUri").Value;
            _client.BaseAddress = new Uri(divelogAPIUri);
            _client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<bool> UploadDivesToAPI(List<LogEntryDTO> dives)
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

        public async Task<List<LogEntryDTO>> GetAllDives()
        {
            try
            {
                var repsonse = await _client.GetStringAsync("api/LogEntries");
                var dives = JsonConvert.DeserializeObject<List<LogEntryDTO>>(repsonse);
                return dives;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
