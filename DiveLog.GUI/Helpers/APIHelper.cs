using DiveLog.DTO;
using DiveLog.DTO.Types;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WebApiContrib.Content;

namespace DiveLog.GUI.Helpers
{
    public class APIHelper : IDisposable
    {
        private static HttpClient _client;
        private readonly ILogger _logger;

        public APIHelper(
            IConfiguration configuration,
            ILogger<APIHelper> logger)
        {
            _ = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            var divelogAPIUri = configuration.GetSection("DiveLogAPIUri").Value;

            _client = new HttpClient();
            _client.BaseAddress = new Uri(divelogAPIUri);
            _client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            _client.Timeout = new TimeSpan(0, 5, 0);
        }

        public async Task<bool> UploadDivesToAPI(List<LogEntryDTO> dives)
        {
            // TODO: Remove this an upload all the dives.
            //var test = dives.Take(10).ToList();
            var json = JsonConvert.SerializeObject(dives);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
            var compressedContent = new CompressedContent(stringContent, "gzip");
            try
            {
                var response = await _client.PostAsync("api/LogEntries", compressedContent);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
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

        public async Task<List<LogEntryDTO>> SearchDives(DiveType diveType, decimal targetDepth, short targetDepthRange, TimeSpan TargetDiveLength, TimeSpan TargetDiveLengthRange)
        {
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
