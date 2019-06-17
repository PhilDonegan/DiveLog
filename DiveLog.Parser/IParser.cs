using DiveLog.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiveLog.Parsers
{
    public interface IParser
    {
        Task<List<LogEntryDTO>> ProcessDivesAsync(object data);
    }
}
