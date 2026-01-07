using App.Application.Dto.DynamicQuestionnaire;
using App.Application.Dto.SubscriptionPlan;
using App.Domain.Entities.DynamicQuestionnaire;
using App.Domain.Entities.SubscriptionPlan;
using AutoMapper;
namespace App.Application.Profiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {

        CreateMap<SubscriptionPlansDTO, SubscriptionPlans>()
        .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.PlanId))
        .ForMember(dest => dest.PlanName, opt => opt.MapFrom(src => src.PlanName))
        .ForMember(dest => dest.PerClient, opt => opt.MapFrom(src => src.PerClient))
        .ForMember(dest => dest.ClientCount, opt => opt.MapFrom(src => src.ClientCount))
        .ForMember(dest => dest.PlatFormRate, opt => opt.MapFrom(src => src.PlatFormRate))
        .ForMember(dest => dest.IsMonthly, opt => opt.MapFrom(src => src.IsMonthly))
        .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive ?? true));// default to true if null
                                                                                       //.ForAllOtherMembers(opt => opt.Ignore());
        CreateMap<SubscriptionPlans, SubscriptionPlansDTO>().ReverseMap();

    }
}
