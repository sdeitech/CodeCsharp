using App.Application.Dto;
using App.Common.Models;
using App.Domain.Entities.AgencySubscriptions;

namespace App.Application.Interfaces.Repositories.AgencySubscriptions
{
    public interface IAgencySubscriptionPlanRepository
    {
        Task<CurrentSubscriptionPlanDTO> GetCurrentSubscriptionPlan(int id);
        Task<List<SubscriptionPlanListDTO>> GetAllSubscriptionPlan(int id);
        Task<List<OrganizationSubscriptionPlanDetail>> GetSubscriptionAllPlanList(int id);
        Task<bool> CancelSubscriptionAsync(int id, int? subid);
        Task<JsonModel> BuyPlan(SessionMetadataModel session);

        Task<AgencySubscriptionDetail?> GetSubscriptionPlanById(int planId, int organizationId);
    }
}
