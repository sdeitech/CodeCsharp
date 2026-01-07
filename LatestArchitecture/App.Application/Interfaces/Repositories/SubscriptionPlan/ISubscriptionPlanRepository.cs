using App.Application.Dto.Common;
using App.Application.Dto.SubscriptionPlan;
using App.Application.Interfaces;
using App.Common.Models;
using App.Domain.Entities.AgencySubscriptions;
using App.Domain.Entities.MasterOrg;
using App.Domain.Entities.SubscriptionPlan;

namespace App.Application.Interfaces.Repositories.SubscriptionPlan
{

    public interface ISubscriptionPlansRepository : IRepository<SubscriptionPlans>
    {
        SubscriptionPlans CreateSubscriptionPlan(SubscriptionPlans subscriptionEntity);

        SubscriptionPlanModules CreateModuleSubscriptionPlan(
            SubscriptionPlanModules subscriptionPlanModule,
            List<ModulePermissionDTO> subscriptionPlanModuleList
        );

        Task<IEnumerable<T>> GetSubscription<T>(ListingFiltterDTO listingFiltterDTO) where T : class, new();
        
        Task<List<SubscriptionPlansListDTO>> GetSubscriptionPlan(
          FilterDto filterDto
        );
      
        Task<List<SubscriptionOrgListDTO>> GetOrgSubscriptionPlans(FilterDto filterDto);
        Task<SubscriptionPlans> GetByIdFromMasterAsync(int id);
        
        Task<List<SubscriptionPlansListDTO>> GetAllSubscriptionPlans();
        Task<List<SubscriptionModuleListDTO>> GetAllModuleList<T>();
        Task<SubscriptionPlans> GetById(int id);
        Task<List<SubscriptionPlanModules>> GetModulesByPlanId(int planId);
        void RemoveModulesByPlanId(int planId);
        Task<List<SubscriptionPlans>> GetAllSubscriptionPlan();
        Task<List<SubscriptionPlanModules>> GetAllSubscriptionPlanModules();
        Task<List<AgencySubscriptionDetail>> GetAllAgencySubscriptionDetails();
        Task<List<MasterOrganization>> GetAllMasterOrganizations();
        Task<JsonModel> SaveOrUpdateSubscriptionPlan(SubscriptionPlansDTO subscriptionDTO);
        Task<stripplandetail> CreateMasterSubscriptionAsync(SubscriptionPlansDTO subscriptionDTO);
        Task<List<SubscriptionPlanModules>> GetSubscriptionPlanModulesByPlanId(int planId);
        Task<List<Modules>> GetAllModules();
    }
    

}
