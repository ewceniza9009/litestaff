using whris.Application.Dtos;

namespace whris.Application.Mappers
{
    public class MappingProfileForMstUserDetail
    {
        public AutoMapper.MapperConfiguration config { get; set; }
        public AutoMapper.IMapper mapper { get; set; }

        public MappingProfileForMstUserDetail()
        {
            config = new AutoMapper.MapperConfiguration(cfg => cfg.AddProfile<MstUserDetailProfile>());
            mapper = config.CreateMapper();
        }
    }

    public class MstUserDetailProfile : AutoMapper.Profile
    {
        public MstUserDetailProfile()
        {
            CreateMap<Data.Models.MstUser, MstUserDetailDto>()
                .ForMember(dest => dest.MstUserForms, conf => conf.MapFrom(value => value.MstUserForms));
            CreateMap<Data.Models.MstUserForm, MstUserFormDto>();
        }
    }

    public class MappingProfileForMstUserDetailReverse
    {
        public AutoMapper.MapperConfiguration config { get; set; }
        public AutoMapper.IMapper mapper { get; set; }

        public MappingProfileForMstUserDetailReverse()
        {
            config = new AutoMapper.MapperConfiguration(cfg => cfg.AddProfile<MstUserDetailReverseProfile>());
            mapper = config.CreateMapper();
        }
    }

    public class MstUserDetailReverseProfile : AutoMapper.Profile
    {
        public MstUserDetailReverseProfile()
        {
            CreateMap<MstUserDetailDto, Data.Models.MstUser>()
                .ForMember(dest => dest.MstUserForms, conf => conf.MapFrom(value => value.MstUserForms));
            CreateMap<MstUserFormDto, Data.Models.MstUserForm>();
        }
    }
}
