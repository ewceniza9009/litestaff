using whris.Application.Dtos;

namespace whris.Application.Mappers
{
    public class MappingProfileForTrnPayrollOtherDeductionDetail
    {
        public AutoMapper.MapperConfiguration config { get; set; }
        public AutoMapper.IMapper mapper { get; set; }

        public MappingProfileForTrnPayrollOtherDeductionDetail()
        {
            config = new AutoMapper.MapperConfiguration(cfg => cfg.AddProfile<TrnPayrollOtherDeductionDetailProfile>());
            mapper = config.CreateMapper();
        }
    }

    public class TrnPayrollOtherDeductionDetailProfile : AutoMapper.Profile
    {
        public TrnPayrollOtherDeductionDetailProfile()
        {
            CreateMap<Data.Models.TrnPayrollOtherDeduction, TrnPayrollOtherDeductionDetailDto>()
                .ForMember(dest => dest.TrnPayrollOtherDeductionLines, conf => conf.MapFrom(value => value.TrnPayrollOtherDeductionLines));
            CreateMap<Data.Models.TrnPayrollOtherDeductionLine, TrnPayrollOtherDeductionLineDto>();
        }
    }

    public class MappingProfileForTrnPayrollOtherDeductionDetailReverse
    {
        public AutoMapper.MapperConfiguration config { get; set; }
        public AutoMapper.IMapper mapper { get; set; }

        public MappingProfileForTrnPayrollOtherDeductionDetailReverse()
        {
            config = new AutoMapper.MapperConfiguration(cfg => cfg.AddProfile<TrnPayrollOtherDeductionDetailReverseProfile>());
            mapper = config.CreateMapper();
        }
    }

    public class TrnPayrollOtherDeductionDetailReverseProfile : AutoMapper.Profile
    {
        public TrnPayrollOtherDeductionDetailReverseProfile()
        {
            CreateMap<TrnPayrollOtherDeductionDetailDto, Data.Models.TrnPayrollOtherDeduction>()
                .ForMember(dest => dest.TrnPayrollOtherDeductionLines, conf => conf.MapFrom(value => value.TrnPayrollOtherDeductionLines));
            CreateMap<TrnPayrollOtherDeductionLineDto, Data.Models.TrnPayrollOtherDeductionLine>();
        }
    }
}
