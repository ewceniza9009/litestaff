using whris.Application.Dtos;

namespace whris.Application.Mappers
{
    public class MappingProfileForTrnLeaveApplicationDetail
    {
        public AutoMapper.MapperConfiguration config { get; set; }
        public AutoMapper.IMapper mapper { get; set; }

        public MappingProfileForTrnLeaveApplicationDetail()
        {
            config = new AutoMapper.MapperConfiguration(cfg => cfg.AddProfile<TrnLeaveApplicationDetailProfile>());
            mapper = config.CreateMapper();
        }
    }

    public class TrnLeaveApplicationDetailProfile : AutoMapper.Profile
    {
        public TrnLeaveApplicationDetailProfile()
        {
            CreateMap<Data.Models.TrnLeaveApplication, TrnLeaveApplicationDetailDto>()
                .ForMember(dest => dest.TrnLeaveApplicationLines, conf => conf.MapFrom(value => value.TrnLeaveApplicationLines));
            CreateMap<Data.Models.TrnLeaveApplicationLine, TrnLeaveApplicationLineDto>();
        }
    }

    public class MappingProfileForTrnLeaveApplicationDetailReverse
    {
        public AutoMapper.MapperConfiguration config { get; set; }
        public AutoMapper.IMapper mapper { get; set; }

        public MappingProfileForTrnLeaveApplicationDetailReverse()
        {
            config = new AutoMapper.MapperConfiguration(cfg => cfg.AddProfile<TrnLeaveApplicationDetailReverseProfile>());
            mapper = config.CreateMapper();
        }
    }

    public class TrnLeaveApplicationDetailReverseProfile : AutoMapper.Profile
    {
        public TrnLeaveApplicationDetailReverseProfile()
        {
            CreateMap<TrnLeaveApplicationDetailDto, Data.Models.TrnLeaveApplication>()
                .ForMember(dest => dest.TrnLeaveApplicationLines, conf => conf.MapFrom(value => value.TrnLeaveApplicationLines));
            CreateMap<TrnLeaveApplicationLineDto, Data.Models.TrnLeaveApplicationLine>();
        }
    }
}
