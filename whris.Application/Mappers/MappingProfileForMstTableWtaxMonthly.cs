using whris.Application.Dtos;

namespace whris.Application.Mappers
{
    public class MappingProfileForMstTableWtaxMonthly
    {
        public AutoMapper.MapperConfiguration config { get; set; }
        public AutoMapper.IMapper mapper { get; set; }

        public MappingProfileForMstTableWtaxMonthly()
        {
            config = new AutoMapper.MapperConfiguration(cfg => cfg.AddProfile<MstTableWtaxMonthlyProfile>());
            mapper = config.CreateMapper();
        }
    }

    public class MstTableWtaxMonthlyProfile : AutoMapper.Profile
    {
        public MstTableWtaxMonthlyProfile()
        {
            CreateMap<Data.Models.MstTableWtaxMonthly, MstTableWtaxMonthlyDto>()
                .IgnoreAllSourcePropertiesWithAnInaccessibleSetter();
        }
    }

    public class MappingProfileForMstTableWtaxMonthlyReverse
    {
        public AutoMapper.MapperConfiguration config { get; set; }
        public AutoMapper.IMapper mapper { get; set; }

        public MappingProfileForMstTableWtaxMonthlyReverse()
        {
            config = new AutoMapper.MapperConfiguration(cfg => cfg.AddProfile<MstTableWtaxMonthlyReverseProfile>());
            mapper = config.CreateMapper();
        }
    }

    public class MstTableWtaxMonthlyReverseProfile : AutoMapper.Profile
    {
        public MstTableWtaxMonthlyReverseProfile()
        {
            CreateMap<MstTableWtaxMonthlyDto, Data.Models.MstTableWtaxMonthly>()
                .IgnoreAllPropertiesWithAnInaccessibleSetter();
        }
    }
}
