using whris.Application.Dtos;

namespace whris.Application.Mappers
{
    public class MappingProfileForMstShiftCodeDetail
    {
        public AutoMapper.MapperConfiguration config { get; set; }
        public AutoMapper.IMapper mapper { get; set; }

        public MappingProfileForMstShiftCodeDetail()
        {
            config = new AutoMapper.MapperConfiguration(cfg => cfg.AddProfile<MstShiftCodeDetailProfile>());
            mapper = config.CreateMapper();
        }
    }

    public class MstShiftCodeDetailProfile : AutoMapper.Profile
    {
        public MstShiftCodeDetailProfile()
        {
            CreateMap<Data.Models.MstShiftCode, MstShiftCodeDetailDto>()
                .ForMember(dest => dest.MstShiftCodeDays, conf => conf.MapFrom(value => value.MstShiftCodeDays));
            CreateMap<Data.Models.MstShiftCodeDay, MstShiftCodeDayDto>();
        }
    }

    public class MappingProfileForMstShiftCodeDetailReverse
    {
        public AutoMapper.MapperConfiguration config { get; set; }
        public AutoMapper.IMapper mapper { get; set; }

        public MappingProfileForMstShiftCodeDetailReverse()
        {
            config = new AutoMapper.MapperConfiguration(cfg => cfg.AddProfile<MstShiftCodeDetailReverseProfile>());
            mapper = config.CreateMapper();
        }
    }

    public class MstShiftCodeDetailReverseProfile : AutoMapper.Profile
    {
        public MstShiftCodeDetailReverseProfile()
        {
            CreateMap<MstShiftCodeDetailDto, Data.Models.MstShiftCode>()
                .ForMember(dest => dest.MstShiftCodeDays, conf => conf.MapFrom(value => value.MstShiftCodeDays));
            CreateMap<MstShiftCodeDayDto, Data.Models.MstShiftCodeDay>();
        }
    }
}
