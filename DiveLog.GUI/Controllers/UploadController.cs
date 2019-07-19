﻿using DiveLog.GUI.Helpers;
using DiveLog.GUI.Models;
using DiveLog.Parser.Types;
using DiveLog.Parsers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
        private IParser _parser;

        public UploadController(
            IConfiguration config,
            Func<SupportedParsers, IParser> parserService,
            APIHelper apiHelper)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _apiHelper = apiHelper ?? throw new ArgumentNullException(nameof(apiHelper));
            _parser = parserService(SupportedParsers.Shearwater);
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

        private async Task<string> AddDataToStorage(IFormFile data)
        {
            var uniqueId = $"{Guid.NewGuid().ToString()}.db";

            if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads")))
            {
                Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads"));
            }

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", uniqueId);
            var storeUpload = bool.TryParse(_config.GetSection("StoreUploads").Value, out _);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await data.CopyToAsync(stream);
                if (storeUpload)
                {
                    var debugUploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "debug");
                    if (!Directory.Exists(debugUploadPath))
                    {
                        Directory.CreateDirectory(debugUploadPath);
                    }

                    System.IO.File.Copy(path, Path.Combine(debugUploadPath, uniqueId));
                }
            }

            return path;
        }
    }
}
