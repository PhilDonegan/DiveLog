﻿using DiveLog.DTO;
using DiveLog.DTO.Types;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
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
                _logger.LogError(ex, ex.Message);
                throw ex;
            }
        }

		public async Task<List<ComparisonMetricDTO>> GetAvailableComparisons()
		{
			try
			{
				var response = await _client.GetStringAsync("api/Stats/GetAvailableComparisons");
				var result = JsonConvert.DeserializeObject<List<ComparisonMetricDTO>>(response);
				return result;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, ex.Message);
				throw ex;
			}
		}

        public async Task<List<LogEntryDTO>> SearchDives(DiveType? diveType, decimal targetDepth, short targetDepthRange, double TargetDiveLength, double TargetDiveLengthRange)
        {
            var queryParams = new Dictionary<string, string>();

            if (diveType.HasValue)
            {
                queryParams.Add("DiveType", diveType.Value.ToString("D"));
            }

            queryParams.Add("TargetDepth", targetDepth.ToString());
            queryParams.Add("TargetDepthRange", targetDepthRange.ToString());
            queryParams.Add("TargetDiveLength", TargetDiveLength.ToString());
            queryParams.Add("TargetDiveLengthRange", TargetDiveLengthRange.ToString());

            var url = QueryHelpers.AddQueryString("api/LogEntries/SearchDives", queryParams);

            try
            {
                var response = await _client.GetStringAsync(url);
                var dives = JsonConvert.DeserializeObject<List<LogEntryDTO>>(response);
                return dives;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw ex;
            }
        }

		public async Task<List<LogEntryDTO>> CompareDives(int depth, int bottomTime, DiveType diveType)
		{
			var queryParams = new Dictionary<string, string>();

			queryParams.Add("depth", depth.ToString());
			queryParams.Add("bottomTime", bottomTime.ToString());
			queryParams.Add("diveType", diveType.ToString("D"));

			var url = QueryHelpers.AddQueryString("api/LogEntries/CompareDives", queryParams);

			try
			{
				var response = await _client.GetStringAsync(url);
				var dives = JsonConvert.DeserializeObject<List<LogEntryDTO>>(response);
				return dives;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, ex.Message);
				throw ex;
			}			
		}

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
