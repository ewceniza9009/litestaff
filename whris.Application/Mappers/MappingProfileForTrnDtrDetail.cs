using whris.Application.Dtos;

namespace whris.Application.Mappers
{
    public class MappingProfileForTrnDtrDetail
    {
        public AutoMapper.MapperConfiguration config { get; set; }
        public AutoMapper.IMapper mapper { get; set; }

        public MappingProfileForTrnDtrDetail()
        {
            config = new AutoMapper.MapperConfiguration(cfg => cfg.AddProfile<TrnDtrDetailProfile>());
            mapper = config.CreateMapper();
        }
    }

    public class TrnDtrDetailProfile : AutoMapper.Profile
    {
        public TrnDtrDetailProfile()
        {
            CreateMap<Data.Models.TrnDtr, TrnDtrDetailDto>();
            //    .ForMember(dest => dest.TrnDtrlines, conf => conf.MapFrom(value => value.TrnDtrlines));
            //CreateMap<Data.Models.TrnDtrline, TrnDtrLineDto>();
        }
    }

    public class MappingProfileForTrnDtrDetailReverse
    {
        public AutoMapper.MapperConfiguration config { get; set; }
        public AutoMapper.IMapper mapper { get; set; }

        public MappingProfileForTrnDtrDetailReverse()
        {
            config = new AutoMapper.MapperConfiguration(cfg => cfg.AddProfile<TrnDtrDetailReverseProfile>());
            mapper = config.CreateMapper();
        }
    }

    public class TrnDtrDetailReverseProfile : AutoMapper.Profile
    {
        public TrnDtrDetailReverseProfile()
        {
            CreateMap<TrnDtrDetailDto, Data.Models.TrnDtr>()
                .ForMember(dest => dest.TrnDtrlines, conf => conf.MapFrom(value => value.TrnDtrlines));
            CreateMap<TrnDtrLineDto, Data.Models.TrnDtrline>();
        }
    }

    //Profile for DtrLines

    public class MappingProfileForTrnDtrLine
    {
        public AutoMapper.MapperConfiguration config { get; set; }
        public AutoMapper.IMapper mapper { get; set; }

        public MappingProfileForTrnDtrLine()
        {
            config = new AutoMapper.MapperConfiguration(cfg => cfg.AddProfile<TrnDtrLineProfile>());
            mapper = config.CreateMapper();
        }
    }

    public class TrnDtrLineProfile : AutoMapper.Profile
    {
        public TrnDtrLineProfile()
        {
            CreateMap<Data.Models.TrnDtrline, TrnDtrLineDto>();
            //.ForMember(dest => dest.TrnDtrlines, conf => conf.MapFrom(value => value.TrnDtrlines));
            //CreateMap<Data.Models.TrnDtrline, TrnDtrLineDto>();
        }
    }
}
