using whris.Application.Dtos;

namespace whris.Application.Mappers
{
    public class MappingProfileForTrnLastWithholdingTaxDetail
    {
        public AutoMapper.MapperConfiguration config { get; set; }
        public AutoMapper.IMapper mapper { get; set; }

        public MappingProfileForTrnLastWithholdingTaxDetail()
        {
            config = new AutoMapper.MapperConfiguration(cfg => cfg.AddProfile<TrnLastWithholdingTaxDetailProfile>());
            mapper = config.CreateMapper();
        }
    }

    public class TrnLastWithholdingTaxDetailProfile : AutoMapper.Profile
    {
        public TrnLastWithholdingTaxDetailProfile()
        {
            CreateMap<Data.Models.TrnLastWithholdingTax, TrnLastWithholdingTaxDetailDto>()
                .ForMember(dest => dest.TrnLastWithholdingTaxLines, conf => conf.MapFrom(value => value.TrnLastWithholdingTaxLines));
            CreateMap<Data.Models.TrnLastWithholdingTaxLine, TrnLastWithholdingTaxLineDto>();
        }
    }

    public class MappingProfileForTrnLastWithholdingTaxDetailReverse
    {
        public AutoMapper.MapperConfiguration config { get; set; }
        public AutoMapper.IMapper mapper { get; set; }

        public MappingProfileForTrnLastWithholdingTaxDetailReverse()
        {
            config = new AutoMapper.MapperConfiguration(cfg => cfg.AddProfile<TrnLastWithholdingTaxDetailReverseProfile>());
            mapper = config.CreateMapper();
        }
    }

    public class TrnLastWithholdingTaxDetailReverseProfile : AutoMapper.Profile
    {
        public TrnLastWithholdingTaxDetailReverseProfile()
        {
            CreateMap<TrnLastWithholdingTaxDetailDto, Data.Models.TrnLastWithholdingTax>()
                .ForMember(dest => dest.TrnLastWithholdingTaxLines, conf => conf.MapFrom(value => value.TrnLastWithholdingTaxLines));
            CreateMap<TrnLastWithholdingTaxLineDto, Data.Models.TrnLastWithholdingTaxLine>();
        }
    }
}
