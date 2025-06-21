using whris.Application.Dtos;

namespace whris.Application.Mappers
{
    public class MappingProfileForMstEmployeeDetail
    {
        public AutoMapper.MapperConfiguration config { get; set; }
        public AutoMapper.IMapper mapper { get; set; }

        public MappingProfileForMstEmployeeDetail()
        {
            config = new AutoMapper.MapperConfiguration(cfg => cfg.AddProfile<MstEmployeeDetailProfile>());
            mapper = config.CreateMapper();
        }
    }

    public class MstEmployeeDetailProfile : AutoMapper.Profile
    {
        public MstEmployeeDetailProfile()
        {
            CreateMap<Data.Models.MstEmployee, MstEmployeeDetailDto>()
                .ForMember(dest => dest.MstEmployeeMemos, conf => conf.MapFrom(value => value.MstEmployeeMemos))
                .ForMember(dest => dest.MstEmployeeShiftCodes, conf => conf.MapFrom(value => value.MstEmployeeShiftCodes))
                .ForMember(dest => dest.MstEmployeeSalaryHistories, conf => conf.MapFrom(value => value.MstEmployeeSalaryHistories));
            CreateMap<Data.Models.MstEmployeeMemo, MstEmployeeMemoDto>();
            CreateMap<Data.Models.MstEmployeeShiftCode, MstEmployeeShiftCodeDto>();
            CreateMap<Data.Models.MstEmployeeSalaryHistory, MstEmployeeSalaryHistoryDto>();
        }
    }

    public class MappingProfileForMstEmployeeDetailReverse
    {
        public AutoMapper.MapperConfiguration config { get; set; }
        public AutoMapper.IMapper mapper { get; set; }

        public MappingProfileForMstEmployeeDetailReverse()
        {
            config = new AutoMapper.MapperConfiguration(cfg => cfg.AddProfile<MstEmployeeDetailReverseProfile>());
            mapper = config.CreateMapper();
        }
    }

    public class MstEmployeeDetailReverseProfile : AutoMapper.Profile
    {
        public MstEmployeeDetailReverseProfile()
        {
            CreateMap<MstEmployeeDetailDto, Data.Models.MstEmployee>()
                .ForMember(dest => dest.MstEmployeeMemos, conf => conf.MapFrom(value => value.MstEmployeeMemos))
                //.ForMember(dest => dest.MstEmployeeShiftCodes, conf => conf.MapFrom(value => value.MstEmployeeShiftCodes))
                .ForMember(dest => dest.MstEmployeeSalaryHistories, conf => conf.MapFrom(value => value.MstEmployeeSalaryHistories));
            CreateMap<MstEmployeeMemoDto, Data.Models.MstEmployeeMemo>();
            //CreateMap<MstEmployeeShiftCodeDto, Data.Models.MstEmployeeShiftCode>();
            CreateMap<MstEmployeeSalaryHistoryDto, Data.Models.MstEmployeeSalaryHistory>();
        }
    }
}
