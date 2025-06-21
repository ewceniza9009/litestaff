using whris.Application.Dtos;

namespace whris.Application.Mappers
{
    public class MappingProfileForMstDayTypeDetail
    {
        public AutoMapper.MapperConfiguration config { get; set; }
        public AutoMapper.IMapper mapper { get; set; }

        public MappingProfileForMstDayTypeDetail()
        {
            config = new AutoMapper.MapperConfiguration(cfg => cfg.AddProfile<MstDayTypeDetailProfile>());
            mapper = config.CreateMapper();
        }
    }

    public class MstDayTypeDetailProfile : AutoMapper.Profile
    {
        public MstDayTypeDetailProfile()
        {
            CreateMap<Data.Models.MstDayType, MstDayTypeDetailDto>()
                .ForMember(dest => dest.MstDayTypeDays, conf => conf.MapFrom(value => value.MstDayTypeDays));
            CreateMap<Data.Models.MstDayTypeDay, MstDayTypeDayDto>();
        }
    }

    public class MappingProfileForMstDayTypeDetailReverse
    {
        public AutoMapper.MapperConfiguration config { get; set; }
        public AutoMapper.IMapper mapper { get; set; }

        public MappingProfileForMstDayTypeDetailReverse()
        {
            config = new AutoMapper.MapperConfiguration(cfg => cfg.AddProfile<MstDayTypeDetailReverseProfile>());
            mapper = config.CreateMapper();
        }
    }

    public class MstDayTypeDetailReverseProfile : AutoMapper.Profile
    {
        public MstDayTypeDetailReverseProfile()
        {
            CreateMap<MstDayTypeDetailDto, Data.Models.MstDayType>()
                .ForMember(dest => dest.MstDayTypeDays, conf => conf.MapFrom(value => value.MstDayTypeDays));
            CreateMap<MstDayTypeDayDto, Data.Models.MstDayTypeDay>();
        }
    }
}
