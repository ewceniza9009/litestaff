using whris.Application.Dtos;

namespace whris.Application.Mappers
{
    public class MappingProfileForMstTaxCode
    {
        public AutoMapper.MapperConfiguration config { get; set; }
        public AutoMapper.IMapper mapper { get; set; }

        public MappingProfileForMstTaxCode()
        {
            config = new AutoMapper.MapperConfiguration(cfg => cfg.AddProfile<MstTaxCodeProfile>());
            mapper = config.CreateMapper();
        }
    }

    public class MstTaxCodeProfile : AutoMapper.Profile
    {
        public MstTaxCodeProfile()
        {
            CreateMap<Data.Models.MstTaxCode, MstTaxCodeDto>()
                .IgnoreAllSourcePropertiesWithAnInaccessibleSetter();
        }
    }

    public class MappingProfileForMstTaxCodeReverse
    {
        public AutoMapper.MapperConfiguration config { get; set; }
        public AutoMapper.IMapper mapper { get; set; }

        public MappingProfileForMstTaxCodeReverse()
        {
            config = new AutoMapper.MapperConfiguration(cfg => cfg.AddProfile<MstTaxCodeReverseProfile>());
            mapper = config.CreateMapper();
        }
    }

    public class MstTaxCodeReverseProfile : AutoMapper.Profile
    {
        public MstTaxCodeReverseProfile()
        {
            CreateMap<MstTaxCodeDto, Data.Models.MstTaxCode>()
                .IgnoreAllPropertiesWithAnInaccessibleSetter();
        }
    }
}
