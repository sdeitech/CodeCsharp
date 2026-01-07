using App.Application.Dto.Organization;
using AutoMapper;

namespace App.Application.Profiles.Organization
{
    public class OrganizationMappingProfile : Profile
    {
        public OrganizationMappingProfile()
        {
            CreateMap<OrganizationDto, Domain.Entities.Organization.Organization>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.OrganizationID)).ReverseMap();
        }
    }
}
