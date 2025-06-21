using whris.Application.Dtos;

namespace whris.Application.Mappers
{
    public class MappingProfileForMstCompanyDetail
    {
        public AutoMapper.MapperConfiguration config { get; set; }
        public AutoMapper.IMapper mapper { get; set; }

        public MappingProfileForMstCompanyDetail()
        {
            config = new AutoMapper.MapperConfiguration(cfg => cfg.AddProfile<MstCompanyDetailProfile>());
            mapper = config.CreateMapper();
        }
    }

    public class MstCompanyDetailProfile : AutoMapper.Profile
    {
        public MstCompanyDetailProfile()
        {
            CreateMap<Data.Models.MstCompany, MstCompanyDetailDto>()
                .ForMember(dest => dest.MstBranches, conf => conf.MapFrom(value => value.MstBranches));
            CreateMap<Data.Models.MstBranch, MstBranchDto>();
        }
    }

    public class MappingProfileForMstCompanyDetailReverse
    {
        public AutoMapper.MapperConfiguration config { get; set; }
        public AutoMapper.IMapper mapper { get; set; }

        public MappingProfileForMstCompanyDetailReverse()
        {
            config = new AutoMapper.MapperConfiguration(cfg => cfg.AddProfile<MstCompanyDetailReverseProfile>());
            mapper = config.CreateMapper();
        }
    }

    public class MstCompanyDetailReverseProfile : AutoMapper.Profile
    {
        public MstCompanyDetailReverseProfile()
        {
            CreateMap<MstCompanyDetailDto, Data.Models.MstCompany>()
                .ForMember(dest => dest.MstBranches, conf => conf.MapFrom(value => value.MstBranches));
            CreateMap<MstBranchDto, Data.Models.MstBranch>();
        }
    }
}
