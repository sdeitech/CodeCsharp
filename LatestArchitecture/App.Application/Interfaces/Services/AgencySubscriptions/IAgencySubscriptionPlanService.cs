using App.Common.Models;

namespace App.Application.Interfaces.Services.AgencySubscriptions
{
    public interface IAgencySubscriptionPlanService
    {
        Task<JsonModel> GetCurrentSubscriptionPlan(int id);
        Task<JsonModel> GetAllSubscriptionPlan(int id);
        Task<JsonModel> GetSubscriptionAllPlanList(int id);
        Task<bool> CancelSubscriptionAsync(int id, int? subid);
        Task<JsonModel> BuyPlan(SessionMetadataModel session);
        Task<JsonModel> GetSubscriptionPlanById(int planId,int organizationId);
        
    }
}
