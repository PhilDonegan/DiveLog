using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DiveLog.GUI.ViewComponents
{
	public class TotalDivesComponent : ViewComponent
	{
		private readonly ILogger<TotalDivesComponent> _logger;
		private HttpClient _client;

		public TotalDivesComponent(
			ILogger<TotalDivesComponent> logger,
			IConfiguration configuration)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_ = configuration ?? throw new ArgumentNullException(nameof(logger));

			var divelogAPIUri = configuration.GetSection("DiveLogAPIUri").Value;

			_client = new HttpClient();
			_client.BaseAddress = new Uri(divelogAPIUri);
			_client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
			_client.Timeout = new TimeSpan(0, 5, 0);
		}


		public async Task<string> InvokeAsync()
		{
			try
			{
				return await _client.GetStringAsync("api/Stats");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, ex.Message);
				throw ex;
			}
		}
	}
}
