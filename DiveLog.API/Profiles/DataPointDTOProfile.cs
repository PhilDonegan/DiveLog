using AutoMapper;
using DiveLog.DAL.Models;
using DiveLog.DTO;

namespace DiveLog.API.Profiles
{
    public class DataPointDTOProfile : Profile
    {
        public DataPointDTOProfile()
        {
            CreateMap<DataPoint, DataPointDTO>()
                .ForMember(x => x.LogEntryId, m => m.MapFrom(y => y.LogEntryId))
                .ForMember(x => x.Time, m => m.MapFrom(y => y.Time))
                .ForMember(x => x.Depth, m => m.MapFrom(y => y.Depth))
                .ForMember(x => x.AveragePPO2, m => m.MapFrom(y => y.AveragePPO2))
                .ForMember(x => x.WaterTemp, m => m.MapFrom(y => y.WaterTemp));
        }
    }
}
