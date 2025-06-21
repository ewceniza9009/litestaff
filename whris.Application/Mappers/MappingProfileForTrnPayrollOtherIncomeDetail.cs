using whris.Application.Dtos;

namespace whris.Application.Mappers
{
    public class MappingProfileForTrnPayrollOtherIncomeDetail
    {
        public AutoMapper.MapperConfiguration config { get; set; }
        public AutoMapper.IMapper mapper { get; set; }

        public MappingProfileForTrnPayrollOtherIncomeDetail()
        {
            config = new AutoMapper.MapperConfiguration(cfg => cfg.AddProfile<TrnPayrollOtherIncomeDetailProfile>());
            mapper = config.CreateMapper();
        }
    }

    public class TrnPayrollOtherIncomeDetailProfile : AutoMapper.Profile
    {
        public TrnPayrollOtherIncomeDetailProfile()
        {
            CreateMap<Data.Models.TrnPayrollOtherIncome, TrnPayrollOtherIncomeDetailDto>()
                .ForMember(dest => dest.TrnPayrollOtherIncomeLines, conf => conf.MapFrom(value => value.TrnPayrollOtherIncomeLines));
            CreateMap<Data.Models.TrnPayrollOtherIncomeLine, TrnPayrollOtherIncomeLineDto>();
        }
    }

    public class MappingProfileForTrnPayrollOtherIncomeDetailReverse
    {
        public AutoMapper.MapperConfiguration config { get; set; }
        public AutoMapper.IMapper mapper { get; set; }

        public MappingProfileForTrnPayrollOtherIncomeDetailReverse()
        {
            config = new AutoMapper.MapperConfiguration(cfg => cfg.AddProfile<TrnPayrollOtherIncomeDetailReverseProfile>());
            mapper = config.CreateMapper();
        }
    }

    public class TrnPayrollOtherIncomeDetailReverseProfile : AutoMapper.Profile
    {
        public TrnPayrollOtherIncomeDetailReverseProfile()
        {
            CreateMap<TrnPayrollOtherIncomeDetailDto, Data.Models.TrnPayrollOtherIncome>()
                .ForMember(dest => dest.TrnPayrollOtherIncomeLines, conf => conf.MapFrom(value => value.TrnPayrollOtherIncomeLines));
            CreateMap<TrnPayrollOtherIncomeLineDto, Data.Models.TrnPayrollOtherIncomeLine>();
        }
    }
}
