using whris.Application.Dtos;

namespace whris.Application.Mappers
{
    public class MappingProfileForTrnOTApplicationDetail
    {
        public AutoMapper.MapperConfiguration config { get; set; }
        public AutoMapper.IMapper mapper { get; set; }

        public MappingProfileForTrnOTApplicationDetail()
        {
            config = new AutoMapper.MapperConfiguration(cfg => cfg.AddProfile<TrnOTApplicationDetailProfile>());
            mapper = config.CreateMapper();
        }
    }

    public class TrnOTApplicationDetailProfile : AutoMapper.Profile
    {
        public TrnOTApplicationDetailProfile()
        {
            CreateMap<Data.Models.TrnOverTime, TrnOTApplicationDetailDto>()
                .ForMember(dest => dest.TrnOverTimeLines, conf => conf.MapFrom(value => value.TrnOverTimeLines));
            CreateMap<Data.Models.TrnOverTimeLine, TrnOTApplicationLineDto>();
        }
    }

    public class MappingProfileForTrnOTApplicationDetailReverse
    {
        public AutoMapper.MapperConfiguration config { get; set; }
        public AutoMapper.IMapper mapper { get; set; }

        public MappingProfileForTrnOTApplicationDetailReverse()
        {
            config = new AutoMapper.MapperConfiguration(cfg => cfg.AddProfile<TrnOTApplicationDetailReverseProfile>());
            mapper = config.CreateMapper();
        }
    }

    public class TrnOTApplicationDetailReverseProfile : AutoMapper.Profile
    {
        public TrnOTApplicationDetailReverseProfile()
        {
            CreateMap<TrnOTApplicationDetailDto, Data.Models.TrnOverTime>()
                .ForMember(dest => dest.TrnOverTimeLines, conf => conf.MapFrom(value => value.TrnOverTimeLines));
            CreateMap<TrnOTApplicationLineDto, Data.Models.TrnOverTimeLine>();
        }
    }
}
