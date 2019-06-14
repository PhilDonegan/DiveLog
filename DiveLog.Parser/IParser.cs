using DiveLog.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiveLog.Parsers
{
    public interface IParser
    {
        List<LogEntryDTO> ProcessDives(object data);
    }
}
