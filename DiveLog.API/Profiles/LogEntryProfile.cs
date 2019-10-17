using AutoMapper;
using DiveLog.DAL.Models;
using DiveLog.DTO;

namespace DiveLog.API.Profiles
{
    public class LogEntryProfile : Profile
    {
        public LogEntryProfile()
        {
            CreateMap<LogEntryDTO, LogEntry>()
                .ForMember(x => x.Id, m => m.Ignore())
                .ForMember(x => x.ExternalId, m => m.MapFrom(y => y.ExternalId))
                .ForMember(x => x.DiveDate, m => m.MapFrom(y => y.DiveDate))
                .ForMember(x => x.DiveType, m => m.MapFrom(y => y.DiveType))
                .ForMember(x => x.DataPoints, m => m.MapFrom(y => y.DataPoints))
                .ForMember(x => x.Outcome, m => m.MapFrom(y => y.Outcome))
                .ForMember(x => x.MaxDepth, m => m.MapFrom(y => y.MaxDepth))
                .ForMember(x => x.DiveLength, m => m.MapFrom(y => y.DiveLength))
                .ForMember(x => x.FractionO2, m => m.MapFrom(y => y.FractionO2))
                .ForMember(x => x.FractionHe, m => m.MapFrom(y => y.FractionHe))
				.ForMember(x => x.AverageBottomDepth, m => m.MapFrom(y => y.AverageBottomDepth))
				.ForMember(x => x.BottomTime, m => m.MapFrom(y => y.BottomTime))
                .ForMember(x => x.HashCode, m => m.Ignore());
        }
    }
}
