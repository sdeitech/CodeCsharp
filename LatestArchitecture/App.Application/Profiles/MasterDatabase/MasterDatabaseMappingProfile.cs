using App.Application.Dto.MasterDatabase;
using AutoMapper;

namespace App.Application.Profiles.MasterDatabase
{
    public class MasterDatabaseMappingProfile : Profile
    {
        public MasterDatabaseMappingProfile()
        {
            CreateMap<MasterDatabaseDto, Domain.Entities.MasterDatabase.MasterDatabase>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.DatabaseID)).ReverseMap();

            CreateMap<MasterDatabaseResponseDto, Domain.Entities.MasterDatabase.MasterDatabase>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.DatabaseID)).ReverseMap();
        }
    }
}
