using App.Application.Dto.SubscriptionPlan;
using App.Application.Interfaces.Repositories.AgencySubscriptions;
using App.Application.Interfaces.Services.AgencySubscriptions;
using App.Common.Constant;
using App.Common.Models;
using App.Domain.Entities.AgencySubscriptions;
using App.Domain.Entities.SubscriptionPlan;
using System.Net;

namespace App.Application.Service
{
    public class AgencySubscriptionPlanService : IAgencySubscriptionPlanService
    {
        private JsonModel response = new JsonModel();
        private IAgencySubscriptionPlanRepository _agencySubscriptionPlanRepository;

        public AgencySubscriptionPlanService(IAgencySubscriptionPlanRepository agencySubscriptionPlanRepository)
        {
            _agencySubscriptionPlanRepository= agencySubscriptionPlanRepository;
        }
        public async Task<JsonModel> GetCurrentSubscriptionPlan(int id)
        {
            if (id != 0)
            {
                var currentSubscription = await _agencySubscriptionPlanRepository.GetCurrentSubscriptionPlan(id);
                return new JsonModel(currentSubscription, StatusMessage.FetchMessage, (int)HttpStatusCode.OK);
            }

            return new JsonModel(null, StatusMessage.InvalidData, (int)HttpStatusCode.BadRequest);
        }

        public async Task<JsonModel> GetAllSubscriptionPlan(int id)
        {
            if (id != 0)
            {
                var subscriptionPlans = await _agencySubscriptionPlanRepository.GetAllSubscriptionPlan(id);
                return new JsonModel(subscriptionPlans, StatusMessage.FetchMessage, (int)HttpStatusCode.OK);
            }

            return new JsonModel(null, StatusMessage.InvalidData, (int)HttpStatusCode.BadRequest);
        }

        public async Task<JsonModel> GetSubscriptionAllPlanList(int id)
        {
            var allPlans = await _agencySubscriptionPlanRepository.GetSubscriptionAllPlanList(id);
            return new JsonModel(allPlans, StatusMessage.FetchMessage, (int)HttpStatusCode.OK);
        }
        public async Task<bool> CancelSubscriptionAsync(int id, int? subid)
        {
            bool result = false;

            return result= await _agencySubscriptionPlanRepository.CancelSubscriptionAsync(id, subid);

        }

        public async Task<JsonModel> BuyPlan(SessionMetadataModel session)
        {
            return await _agencySubscriptionPlanRepository.BuyPlan(session);
        }
        public async Task<JsonModel> GetSubscriptionPlanById(int planId, int organizationId)
        {
            // Get subscription plan from repository
            AgencySubscriptionDetail subscriptionPlan = await _agencySubscriptionPlanRepository.GetSubscriptionPlanById(planId, organizationId);

            // Declare response
            JsonModel response;

            if (subscriptionPlan != null)
            {
                response = new JsonModel(subscriptionPlan, StatusMessage.FetchMessage, (int)HttpStatusCode.OK);
            }
            else
            {
                response = new JsonModel(new object(), StatusMessage.NotFound, (int)HttpStatusCode.NotFound);
            }

            return response;
        }




    }
}
