using whris.Application.Dtos;

namespace whris.Application.Mappers
{
    public class MappingProfileForMstUserForms
    {
        public AutoMapper.MapperConfiguration config { get; set; }
        public AutoMapper.IMapper mapper { get; set; }

        public MappingProfileForMstUserForms()
        {
            config = new AutoMapper.MapperConfiguration(cfg => cfg.AddProfile<MstUserFormsProfile>());
            mapper = config.CreateMapper();
        }
    }

    public class MstUserFormsProfile : AutoMapper.Profile
    {
        public MstUserFormsProfile()
        {
            CreateMap<Data.Models.MstUserForm, MstUserFormDto>();
        }
    }

    public class MappingProfileForMstUserFormsReverse
    {
        public AutoMapper.MapperConfiguration config { get; set; }
        public AutoMapper.IMapper mapper { get; set; }

        public MappingProfileForMstUserFormsReverse()
        {
            config = new AutoMapper.MapperConfiguration(cfg => cfg.AddProfile<MstUserFormsReverseProfile>());
            mapper = config.CreateMapper();
        }
    }

    public class MstUserFormsReverseProfile : AutoMapper.Profile
    {
        public MstUserFormsReverseProfile()
        {
            CreateMap<MstUserFormDto, Data.Models.MstUserForm>();
        }
    }
}
