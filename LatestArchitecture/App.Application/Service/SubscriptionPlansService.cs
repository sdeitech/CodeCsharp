using App.Application.Dto.Common;
using App.Application.Dto.SubscriptionPlan;
using App.Application.Interfaces.Repositories.AuditLogs;
using App.Application.Interfaces.Repositories.SubscriptionPlan;
using App.Application.Interfaces.Services.SubscriptionPlan;
using App.Application.Interfaces.Services.SuperAdmin;
using App.Application.Service.SuperAdmin;
using App.Common.Constant;
using App.Common.Models;
using App.Common.Utility;
using App.Domain.Entities.SubscriptionPlan;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using static App.Common.Constant.Constants;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace App.Application.SubscriptionPlansService
{
    public class SubscriptionPlansService : ISubscriptionPlansService
    {
        private readonly ISubscriptionPlansRepository _subscriptionRepository;
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private JsonModel response = new JsonModel();
        private readonly ISuperAdminTokenService _superAdminTokenService;
        public SubscriptionPlansService(ISubscriptionPlansRepository _subscriptionRepository,
            IAuditLogRepository auditLogRepository,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper,
            ISuperAdminTokenService superAdminTokenService
            )
        {
            this._subscriptionRepository = _subscriptionRepository;
            this._auditLogRepository = auditLogRepository;
            this._httpContextAccessor = httpContextAccessor;
            this._mapper = mapper;
            this._superAdminTokenService = superAdminTokenService;
        }
        
        public async Task<JsonModel> DeleteSubscriptionPlan(int subscriptionid)
        {
            JsonModel result = new JsonModel();
            var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var tokenData = _superAdminTokenService.GetDataFromToken(token);
            int userId = 0;
            if (tokenData is JwtSecurityToken jwtToken)
            {
                var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim != null)
                {
                    int.TryParse(userIdClaim.Value, out userId);
                }
            }
            SubscriptionPlans subscriptionPlans = await _subscriptionRepository.GetByIdFromMasterAsync(subscriptionid);

            if (subscriptionPlans != null && !subscriptionPlans.IsDeleted)
            {
                subscriptionPlans.IsDeleted = true;
                subscriptionPlans.DeletedBy = userId;
                subscriptionPlans.DeletedAt = DateTime.UtcNow;
                _subscriptionRepository.Update(subscriptionPlans);

                // Add audit logging
                string ipAddress = CommonMethods.GetClientIp(_httpContextAccessor.HttpContext);
                _auditLogRepository.SaveChangesWithAuditLogs(AuditLogsScreen.SubscriptionPlanManagement, (int)MasterActions.Delete, userId, null, ipAddress, (int)MasterPortal.SuperAdminPortal, null, null, null);

                result.Message = StatusMessage.DeleteMessage;
                result.StatusCode = (int)HttpStatusCode.OK;
                result.Data = new object();
            }
            else
            {
                result.Message = StatusMessage.NotFound;
                result.StatusCode = (int)HttpStatusCode.NotFound;
            }

            return result;
        }

        public async Task<JsonModel> GetSubscriptionPlanId(int id)
        {
            SubscriptionPlansListDTO subscriptionPlans = new SubscriptionPlansListDTO();
            // Fix: Use the correct method signature for GetByIdAsync and remove the lambda expression
             SubscriptionPlans subscriptionPlan = await _subscriptionRepository.GetByIdFromMasterAsync(id);
            List<SubscriptionPlanModules> subscriptionmapping = await _subscriptionRepository.GetSubscriptionPlanModulesByPlanId(id);
            int[] serviceids = new int[subscriptionmapping.Count];
            int[] screenids = new int[subscriptionmapping.Count];
            int[] Actionids = new int[subscriptionmapping.Count];
            for (int i = 0; i < subscriptionmapping.Count; i++)
            {
                serviceids[i] = subscriptionmapping[i].Moduleid;
                screenids[i] = subscriptionmapping[i].Screenid != null ? subscriptionmapping[i].Screenid.Value : 0;
                Actionids[i] = subscriptionmapping[i].Actionid != null ? subscriptionmapping[i].Actionid.Value : 0;
            }
            subscriptionPlans.SubscriptionPlanModuleList = serviceids.ToList();
            subscriptionPlans.SubscriptionPlanScreenList = screenids.ToList();
            subscriptionPlans.SubscriptionPlanActionList = Actionids.ToList();
            subscriptionPlans.PlanId = subscriptionPlan.Id;
            subscriptionPlans.PlanName =subscriptionPlan.PlanName;
            subscriptionPlans.PlatFormRate = subscriptionPlan.PlatFormRate;

            return new JsonModel()
        {
            Data = subscriptionPlans,
                Message = StatusMessage.FetchMessage,
                StatusCode = (int)HttpStatusCode.OK//Success
            };
            //if (subscriptionPlans != null && !subscriptionPlans.IsDeleted && subscriptionPlans.IsActive)
            //{
            //    SubscriptionPlansDTO superUserDTO = _mapper.Map<SubscriptionPlansDTO>(subscriptionPlans);
            //    response = new JsonModel(superUserDTO, StatusMessage.FetchMessage, (int)HttpStatusCode.OK);
            //}
            //else
            //{
            //    response = new JsonModel(new object(), StatusMessage.NotFound, (int)HttpStatusCode.NotFound);
            //}

            //return response;
        }

    

        public async Task<JsonModel> SaveSubscriptionPlan(SubscriptionPlansDTO subscriptionDTO)
        {
            var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var tokenData = _superAdminTokenService.GetDataFromToken(token);
            int userId = 0;
            if (tokenData is JwtSecurityToken jwtToken)
            {
                var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim != null)
                {
                    int.TryParse(userIdClaim.Value, out userId);
                }
            }

            var result = await _subscriptionRepository.SaveOrUpdateSubscriptionPlan(subscriptionDTO);

            // Add audit logging
            if (result.StatusCode == (int)HttpStatusCode.OK)
            {
                string ipAddress = CommonMethods.GetClientIp(_httpContextAccessor.HttpContext);
                int action = subscriptionDTO.PlanId > 0 ? (int)MasterActions.Update : (int)MasterActions.Add;
                _auditLogRepository.SaveChangesWithAuditLogs(AuditLogsScreen.SubscriptionPlanManagement, action, userId, null, ipAddress, (int)MasterPortal.SuperAdminPortal, null, null, null);
            }

            return result;
        }

        public async Task<stripplandetail> CreateMasterSubscriptionAsync(SubscriptionPlansDTO subscriptionDTO)
        {
            return await _subscriptionRepository.CreateMasterSubscriptionAsync(subscriptionDTO);
        }


        public async Task<JsonModel> GetAllSubscriptionPlan(ListingFiltterDTO listingFiltterDTO)
        {
            List<SubscrptionListDTO> subscriptionModels = (List<SubscrptionListDTO>)await _subscriptionRepository.GetSubscription<SubscrptionListDTO>(listingFiltterDTO);

            response.Data = subscriptionModels;
            response.Message = StatusMessage.FetchMessage;
            response.StatusCode = (int)HttpStatusCode.OK;

            response.Meta = new Meta()
            {
                TotalRecords = (int)(subscriptionModels != null && subscriptionModels.Count > 0 ? subscriptionModels[0].TotalRecords : 0),
                CurrentPage = listingFiltterDTO.pageNumber,
                PageSize = listingFiltterDTO.pageSize,
                DefaultPageSize = listingFiltterDTO.pageSize,
                TotalPages =(int)Math.Ceiling(Convert.ToDecimal((subscriptionModels != null && subscriptionModels.Count > 0 ? subscriptionModels[0].TotalRecords : 0) / listingFiltterDTO.pageSize))
            };

            return response;
        }

        public async Task<JsonModel> GetAllModuleList()
        {
            var moduleList = await _subscriptionRepository.GetAllModuleList<SubscriptionModuleListDTO>();

            response.Data = moduleList;
            response.Message = StatusMessage.FetchMessage;
            response.StatusCode = (int)HttpStatusCode.OK;

            return response;

        }
        public async Task<JsonModel> GetSubscriptionPlans(FilterDto filterDto)
        {
            var subscriptionplan =await _subscriptionRepository.GetSubscriptionPlan(filterDto);

            foreach (var item in subscriptionplan)
            {
                var subscriptionmapping = await _subscriptionRepository.GetSubscriptionPlanModulesByPlanId(item.PlanId);

                int[] serviceids = new int[subscriptionmapping.Count];
                int[] screenids = new int[subscriptionmapping.Count];
                int[] actionids = new int[subscriptionmapping.Count];

                for (int i = 0; i < subscriptionmapping.Count; i++)
                {
                    serviceids[i] = subscriptionmapping[i].Moduleid;
                    screenids[i] = subscriptionmapping[i].Screenid ?? 0;
                    actionids[i] = subscriptionmapping[i].Actionid ?? 0;
                }

                item.SubscriptionPlanModuleList = serviceids.ToList();
                item.SubscriptionPlanScreenList = screenids.ToList();
                item.SubscriptionPlanActionList = actionids.ToList();
            }

            return new JsonModel
            {
                Meta = new Meta
                {
                    TotalRecords = (int)(subscriptionplan?.Count > 0 ? Convert.ToDecimal(subscriptionplan[0].TotalRecords) : 0),
                    CurrentPage = filterDto.PageNumber,
                    PageSize = filterDto.PageSize,
                    DefaultPageSize = filterDto.PageSize,
                    TotalPages = (int)Math.Ceiling(Convert.ToDecimal((subscriptionplan?.Count > 0 ? subscriptionplan[0].TotalRecords : 0) / filterDto.PageSize))
                },
                Data = subscriptionplan,
                Message = StatusMessage.FetchMessage,
                StatusCode = (int)HttpStatusCode.OK
            };
        }


        public async Task<JsonModel> GetAllSubscriptionPlans()
        {
            var subscriptionplan = await _subscriptionRepository.GetAllSubscriptionPlans();
            foreach (var item in subscriptionplan)
            {
                List<SubscriptionPlanModules> subscriptionmapping = await _subscriptionRepository.GetSubscriptionPlanModulesByPlanId(item.PlanId);
                int[] serviceids = new int[subscriptionmapping.Count];
                int[] screenids = new int[subscriptionmapping.Count];
                int[] Actionids = new int[subscriptionmapping.Count];
                for (int i = 0; i < subscriptionmapping.Count; i++)
                {
                    serviceids[i] = subscriptionmapping[i].Moduleid;
                    screenids[i] = subscriptionmapping[i].Screenid != null ? subscriptionmapping[i].Screenid.Value : 0;
                    Actionids[i] = subscriptionmapping[i].Actionid != null ? subscriptionmapping[i].Actionid.Value : 0;
                }
                item.SubscriptionPlanModuleList = serviceids.ToList();
                item.SubscriptionPlanScreenList = screenids.ToList();
                item.SubscriptionPlanActionList = Actionids.ToList();
            }
            return new JsonModel()
            {
                Data = subscriptionplan,
                Message = StatusMessage.FetchMessage,
                StatusCode = (int)HttpStatusCode.OK//Success
            };
        }
        public async Task<JsonModel> SetActiveInActive(int id, bool value)
        {
            var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var tokenData = _superAdminTokenService.GetDataFromToken(token);
            int userId = 0;
            if (tokenData is JwtSecurityToken jwtToken)
            {
                var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim != null)
                {
                    int.TryParse(userIdClaim.Value, out userId);
                }
            }

            // Fix: Use the correct method signature for GetByIdAsync and remove the lambda expression
            var subscriptionPlan = await _subscriptionRepository.GetByIdFromMasterAsync(id);

            if (subscriptionPlan == null)
            {
                return new JsonModel
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = StatusMessage.NotFound,
                    Data = null
                };
            }

            subscriptionPlan.UpdatedBy = userId;
            subscriptionPlan.UpdatedAt = DateTime.UtcNow;

            if (value)
            {
                subscriptionPlan.IsDeleted = false;
                subscriptionPlan.IsActive = true;
            }
            else
            {
                subscriptionPlan.IsActive = false;
            }

            _subscriptionRepository.Update(subscriptionPlan);

            // Add audit logging
            string ipAddress = CommonMethods.GetClientIp(_httpContextAccessor.HttpContext);
            _auditLogRepository.SaveChangesWithAuditLogs(AuditLogsScreen.SubscriptionPlanManagement, (int)MasterActions.Update, userId, null, ipAddress, (int)MasterPortal.SuperAdminPortal, null, null, null);

            return new JsonModel
            {
                StatusCode = StatusCodes.Status200OK,
                Message = StatusMessage.StatusSuccessfully,
                Data = subscriptionPlan
            };
        }
        public async Task<JsonModel> GetOrgSubscriptionPlans(FilterDto filterDto)
        {
            var subscriptionplan = await _subscriptionRepository.GetOrgSubscriptionPlans(filterDto);
            foreach (var item in subscriptionplan)
            {
                List<SubscriptionPlanModules> subscriptionmapping = await _subscriptionRepository.GetSubscriptionPlanModulesByPlanId(item.PlanId);
                int[] serviceids = new int[subscriptionmapping.Count];
                int[] screenids = new int[subscriptionmapping.Count];
                int[] Actionids = new int[subscriptionmapping.Count];
                for (int i = 0; i < subscriptionmapping.Count; i++)
                {
                    serviceids[i] = subscriptionmapping[i].Moduleid;
                    screenids[i] = subscriptionmapping[i].Screenid != null ? subscriptionmapping[i].Screenid.Value : 0;
                    Actionids[i] = subscriptionmapping[i].Actionid != null ? subscriptionmapping[i].Actionid.Value : 0;
                }
                item.SubscriptionPlanModuleList = serviceids.ToList();
                item.SubscriptionPlanScreenList = screenids.ToList();
                item.SubscriptionPlanActionList = Actionids.ToList();
            }
            response.Data = subscriptionplan;
            response.Message = StatusMessage.FetchMessage;
            response.StatusCode = (int)HttpStatusCode.OK;

            return new JsonModel
            {
                Meta = new Meta
                {
                    TotalRecords = (int)(subscriptionplan?.Count > 0 ? Convert.ToDecimal(subscriptionplan[0].TotalRecords) : 0),
                    CurrentPage = filterDto.PageNumber,
                    PageSize = filterDto.PageSize,
                    DefaultPageSize = filterDto.PageSize,
                    TotalPages = (int)Math.Ceiling(Convert.ToDecimal((subscriptionplan?.Count > 0 ? subscriptionplan[0].TotalRecords : 0) / filterDto.PageSize))
                },
                Data = subscriptionplan,
                Message = StatusMessage.FetchMessage,
                StatusCode = (int)HttpStatusCode.OK
            };
        }



    }
}
