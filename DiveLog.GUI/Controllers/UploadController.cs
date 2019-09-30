using DiveLog.GUI.Helpers;
using DiveLog.GUI.Hubs;
using DiveLog.GUI.Models;
using DiveLog.Parser.Progress;
using DiveLog.Parser.Types;
using DiveLog.Parsers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace DiveLog.GUI.Controllers
{
    public class UploadController : Controller
    {
		private string _id = Guid.NewGuid().ToString("N");
		private IConfiguration _config;
        private APIHelper _apiHelper;
		private readonly IHubContext<DiveParseProgressHub> _hubContext;
		private readonly ILogger<UploadController> _logger;
		private IParser _parser;

        public UploadController(
            IConfiguration config,
            Func<SupportedParsers, IParser> parserService,
            APIHelper apiHelper,
			IHubContext<DiveParseProgressHub> hubContext,
			ILogger<UploadController> logger)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _apiHelper = apiHelper ?? throw new ArgumentNullException(nameof(apiHelper));
			_hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_parser = parserService(SupportedParsers.Shearwater);
			_parser.DiveParsed += DiveParserProgressChanged;
        }

		private async void DiveParserProgressChanged(ParserProgess parserProgess)
		{
			if (parserProgess != null)
			{
				_logger.LogInformation($"Dive {parserProgess.CurrentDive} processed of {parserProgess.TotalDives}");
				await _hubContext.Clients.Group(_id).SendAsync("progress", parserProgess);
			}
		}

		public IActionResult Index()
        {
            return View("ShearwaterUpload");
        }

        [HttpPost]
        public async Task<IActionResult> Upload([FromForm(Name ="file")] IFormFile data)
        {
            if (!ModelState.IsValid)
            {
                return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }

            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            var dives = await _parser.ProcessDivesAsync(data);
            var result = await _apiHelper.UploadDivesToAPI(dives);

            // Return dives to UI and gather results

            return RedirectToAction("Index");
        }

		//[HttpPost]
		//public async Task<IActionResult> Process(string fileIdentifier)

        
    }
}
