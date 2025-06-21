using whris.Application.Dtos;

namespace whris.Application.Mappers
{
    public class MappingProfileForTrnChangeShiftCodeDetail
    {
        public AutoMapper.MapperConfiguration config { get; set; }
        public AutoMapper.IMapper mapper { get; set; }

        public MappingProfileForTrnChangeShiftCodeDetail()
        {
            config = new AutoMapper.MapperConfiguration(cfg => cfg.AddProfile<TrnChangeShiftDetailProfile>());
            mapper = config.CreateMapper();
        }
    }

    public class TrnChangeShiftDetailProfile : AutoMapper.Profile
    {
        public TrnChangeShiftDetailProfile()
        {
            CreateMap<Data.Models.TrnChangeShift, TrnChangeShiftCodeDetailDto>()
                .ForMember(dest => dest.TrnChangeShiftLines, conf => conf.MapFrom(value => value.TrnChangeShiftLines));
            CreateMap<Data.Models.TrnChangeShiftLine, TrnChangeShiftCodeLineDto>();
        }
    }

    public class MappingProfileForTrnChangeShiftCodeDetailReverse
    {
        public AutoMapper.MapperConfiguration config { get; set; }
        public AutoMapper.IMapper mapper { get; set; }

        public MappingProfileForTrnChangeShiftCodeDetailReverse()
        {
            config = new AutoMapper.MapperConfiguration(cfg => cfg.AddProfile<TrnChangeShiftDetailReverseProfile>());
            mapper = config.CreateMapper();
        }
    }

    public class TrnChangeShiftDetailReverseProfile : AutoMapper.Profile
    {
        public TrnChangeShiftDetailReverseProfile()
        {
            CreateMap<TrnChangeShiftCodeDetailDto, Data.Models.TrnChangeShift>()
                .ForMember(dest => dest.TrnChangeShiftLines, conf => conf.MapFrom(value => value.TrnChangeShiftLines));
            CreateMap<TrnChangeShiftCodeLineDto, Data.Models.TrnChangeShiftLine>();
        }
    }
}
