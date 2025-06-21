using whris.Application.Dtos;

namespace whris.Application.Mappers
{
    public class MappingProfileForTrnPayrollDetail
    {
        public AutoMapper.MapperConfiguration config { get; set; }
        public AutoMapper.IMapper mapper { get; set; }

        public MappingProfileForTrnPayrollDetail()
        {
            config = new AutoMapper.MapperConfiguration(cfg => cfg.AddProfile<TrnPayrollDetailProfile>());
            mapper = config.CreateMapper();
        }
    }

    public class TrnPayrollDetailProfile : AutoMapper.Profile
    {
        public TrnPayrollDetailProfile()
        {
            CreateMap<Data.Models.TrnPayroll, TrnPayrollDetailDto>()
                .ForMember(dest => dest.TrnPayrollLines, conf => conf.MapFrom(value => value.TrnPayrollLines));
            CreateMap<Data.Models.TrnPayrollLine, TrnPayrollLineDto>();
        }
    }

    public class MappingProfileForTrnPayrollDetailReverse
    {
        public AutoMapper.MapperConfiguration config { get; set; }
        public AutoMapper.IMapper mapper { get; set; }

        public MappingProfileForTrnPayrollDetailReverse()
        {
            config = new AutoMapper.MapperConfiguration(cfg => cfg.AddProfile<TrnPayrollDetailReverseProfile>());
            mapper = config.CreateMapper();
        }
    }

    public class TrnPayrollDetailReverseProfile : AutoMapper.Profile
    {
        public TrnPayrollDetailReverseProfile()
        {
            CreateMap<TrnPayrollDetailDto, Data.Models.TrnPayroll>()
                .ForMember(dest => dest.TrnPayrollLines, conf => conf.MapFrom(value => value.TrnPayrollLines));
            CreateMap<TrnPayrollLineDto, Data.Models.TrnPayrollLine>();
        }
    }
}
