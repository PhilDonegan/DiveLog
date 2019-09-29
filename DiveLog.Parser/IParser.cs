using DiveLog.DTO;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiveLog.Parsers
{
    public interface IParser
    {
		event Shearwater.ParserProgressEventArgs DiveParsed;

		Task<List<LogEntryDTO>> ProcessDivesAsync(IFormFile data);
    }
}
