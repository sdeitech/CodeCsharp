using App.Application.Dto.Common;
using App.Application.Dto.SubscriptionPlan;
using App.Common.Models;

namespace App.Application.Interfaces.Services.SubscriptionPlan
{
    public interface ISubscriptionPlansService
    {
        Task<JsonModel> SaveSubscriptionPlan(SubscriptionPlansDTO subscriptionDTO);
        Task<JsonModel> DeleteSubscriptionPlan(int subscriptionid);
        Task<JsonModel> GetSubscriptionPlanId(int id);
        Task<JsonModel> GetAllSubscriptionPlan(ListingFiltterDTO listingFiltterDTO);
        Task<JsonModel> GetOrgSubscriptionPlans(FilterDto filterDTO);
        Task<JsonModel> GetAllModuleList();
        Task<JsonModel> GetSubscriptionPlans(FilterDto filterDto);
        Task<JsonModel> GetAllSubscriptionPlans();
        Task<JsonModel> SetActiveInActive(int id, bool value);
    }
}
