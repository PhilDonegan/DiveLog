using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using DiveLog.DTO;

namespace DiveLog.Parsers
{
    public class Shearwater : IParser
    {
        public List<LogEntryDTO> ProcessDives(object data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            return null;
        }
    }
}
