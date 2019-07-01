using AutoMapper;
using DiveLog.DAL.Models;
using DiveLog.DTO;

namespace DiveLog.API.Profiles
{
    public class LogEntryProfile : Profile
    {
        public LogEntryProfile()
        {
            CreateMap<LogEntryDTO, LogEntry>();
        }
    }
}
