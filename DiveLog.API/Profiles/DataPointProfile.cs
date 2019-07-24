using AutoMapper;
using DiveLog.DAL.Models;
using DiveLog.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiveLog.API.Profiles
{
    public class DataPointProfile : Profile
    {
        public DataPointProfile()
        {
            CreateMap<DataPointDTO, DataPoint>()
                .ForMember(x => x.Id, m => m.Ignore())
                .ForMember(x => x.LogEntryId, m => m.MapFrom(y => y.LogEntryId))
                .ForMember(x => x.Time, m => m.MapFrom(y => y.Time))
                .ForMember(x => x.Depth, m => m.MapFrom(y => y.Depth))
                .ForMember(x => x.AveragePPO2, m => m.MapFrom(y => y.AveragePPO2))
                .ForMember(x => x.WaterTemp, m => m.MapFrom(y => y.WaterTemp))
                .ForMember(x => x.CNS, m => m.MapFrom(y => y.CNS));
        }
    }
}
