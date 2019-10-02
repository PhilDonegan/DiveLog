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
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace DiveLog.GUI.Controllers
{
    public class UploadController : Controller
    {
		private IConfiguration _config;
        private APIHelper _apiHelper;
		private readonly IHubContext<DiveParseProgressHub> _hubContext;
		private readonly ILogger<UploadController> _logger;
		private readonly IFileHelper _fileHelper;
		private readonly IFileUploadManager _fileUploadManager;
		private IParser _parser;

        public UploadController(
            IConfiguration config,
            Func<SupportedParsers, IParser> parserService,
            APIHelper apiHelper,
			IHubContext<DiveParseProgressHub> hubContext,
			ILogger<UploadController> logger,
			IFileHelper fileHelper,
			IFileUploadManager fileUploadManager)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _apiHelper = apiHelper ?? throw new ArgumentNullException(nameof(apiHelper));
			_hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_fileHelper = fileHelper ?? throw new ArgumentNullException(nameof(fileHelper));
			_fileUploadManager = fileUploadManager ?? throw new ArgumentNullException(nameof(fileUploadManager));
			_parser = parserService(SupportedParsers.Shearwater);
			_parser.DiveParsed += DiveParserProgressChanged;
        }

		private async void DiveParserProgressChanged(string Id, ParserProgess parserProgess)
		{
			if (parserProgess != null)
			{
				_logger.LogInformation($"Server: Dive {parserProgess.CurrentDive} processed of {parserProgess.TotalDives}");
				await _hubContext.Clients.Group(Id).SendAsync("progress", parserProgess);
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

			var path = await _fileHelper.AddDataToStorage(data);
			var id = Guid.NewGuid().ToString("N");
			_fileUploadManager.Add(id, path);

			// Return job id to 
			return Json(id);
        }

		[HttpPost]
		public async Task<IActionResult> Process([FromBody] JObject data)
		{
			_ = data ?? throw new ArgumentNullException(nameof(data));

			var id = data["id"].ToString();

			var path = _fileUploadManager.Get(id);
			var dives = await _parser.ProcessDivesAsync(id, path);
			var result = await _apiHelper.UploadDivesToAPI(dives);

			_fileHelper.DeleteUpload(path);
			return RedirectToAction("Index");
		}
	}
}