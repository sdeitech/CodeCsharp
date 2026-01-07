using App.Application.Dto.Common;
using App.Application.Dto.SubscriptionPlan;
using App.Application.Interfaces;
using App.Application.Interfaces.Repositories;
using App.Application.Interfaces.Repositories.SubscriptionPlan;
using App.Common.Constant;
using App.Common.Models;
using App.Domain.Entities.AgencySubscriptions;
using App.Domain.Entities.MasterOrg;
using App.Domain.Entities.SubscriptionPlan;
using App.Infrastructure.DBContext;
using App.SharedConfigs.DBContext;
using AutoMapper;
using Dapper;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Net;

namespace App.Infrastructure.Repository.SubscriptionPlan
{
    public class SubscriptionPlansRepository : BaseRepository<SubscriptionPlans>, ISubscriptionPlansRepository
    {
        private readonly ApplicationDbContext _context;
        protected readonly MasterDbContext _dbContext;
        protected readonly string _connectionString;
        protected readonly IDbConnection _dbConnection;
        protected readonly IDbConnection _masterdbConnection;
        private readonly IMapper _mapper;

        public SubscriptionPlansRepository(IMapper mapper, IDbConnectionFactory dbConnectionFactory, ApplicationDbContext context,MasterDbContext masterDbContext)
            : base(context, dbConnectionFactory) // Pass the required 'dbConnectionFactory' to the base class constructor
        {
            //_context = context;
            //_dbContext = masterDbContext;
            //_connectionString = context.Database.GetDbConnection().ConnectionString;
            //_dbConnection = dbConnectionFactory.CreateConnection(_connectionString, masterDbContext.Database.ProviderName);
            //_mapper = mapper;
            _context = context;
            _dbContext = masterDbContext;
            _mapper = mapper;

            // Application DB connection
            var appConnString = context.Database.GetDbConnection().ConnectionString;
            _dbConnection = dbConnectionFactory.CreateConnection(appConnString, context.Database.ProviderName);

            // Master DB connection
            var masterConnString = masterDbContext.Database.GetDbConnection().ConnectionString;
            _masterdbConnection = dbConnectionFactory.CreateConnection(masterConnString, masterDbContext.Database.ProviderName);

        }

        public SubscriptionPlans CreateSubscriptionPlan(SubscriptionPlans SubscriptionEntity)
        {
            _dbContext.SubscriptionPlans.Add(SubscriptionEntity);
            return SubscriptionEntity;
        }
        public async Task<SubscriptionPlans> GetByIdFromMasterAsync(int id)
        {
            return await _dbContext.SubscriptionPlans
                .FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);
        }


        public SubscriptionPlanModules CreateModuleSubscriptionPlan(SubscriptionPlanModules SubscriptionPlanModule, List<ModulePermissionDTO> SubscriptionPlanModuleList)
        {
            var entriesToAdd = new List<SubscriptionPlanModules>();

            foreach (var module in SubscriptionPlanModuleList)
            {
                if (module.Screens?.Any() == true)
                {
                    foreach (var screen in module.Screens)
                    {
                        if (screen.Actions?.Any() == true)
                        {
                            foreach (var actionId in screen.Actions)
                            {
                                entriesToAdd.Add(CreateSubscriptionPlanModule(
                                    SubscriptionPlanModule.Planid,
                                    module.ModuleId,
                                    screen.ScreenId,
                                    actionId));
                            }
                        }
                        else
                        {
                            entriesToAdd.Add(CreateSubscriptionPlanModule(
                                SubscriptionPlanModule.Planid,
                                module.ModuleId,
                                screen.ScreenId,
                                null));
                        }
                    }
                }
                else
                {
                    entriesToAdd.Add(CreateSubscriptionPlanModule(
                        SubscriptionPlanModule.Planid,
                        module.ModuleId,
                        null,
                        null));
                }
            }

            _dbContext.SubscriptionPlanModules.AddRange(entriesToAdd);
            return SubscriptionPlanModule;
        }
        private SubscriptionPlanModules CreateSubscriptionPlanModule(int planId, int moduleId, int? screenId = null, int? actionId = null)
        {
            return new SubscriptionPlanModules
            {
                Planid = planId,
                Moduleid = moduleId,
                Screenid = screenId,
                Actionid = actionId
            };
        }
        public async Task<IEnumerable<T>> GetSubscription<T>(ListingFiltterDTO listingFiltterDTO) where T : class, new()
        {
            const string storedProcedure = SqlMethod.Get_SubscriptionPlanList;

            var parameters = new DynamicParameters();
            parameters.Add("@SearchKey", listingFiltterDTO.SearchKey);
            parameters.Add("@SortColumn", listingFiltterDTO.sortColumn);
            parameters.Add("@SortOrder", listingFiltterDTO.sortOrder);
            parameters.Add("@PageNumber", listingFiltterDTO.pageNumber);
            parameters.Add("@PageSize", listingFiltterDTO.pageSize);

            return await _masterdbConnection.QueryAsync<T>(
                storedProcedure,
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<List<SubscriptionPlansListDTO>> GetSubscriptionPlan(FilterDto filterDTO)
        {
            const string storedProcedure = SqlMethod.Get_SubscriptionPlanList;

            var parameters = new DynamicParameters();
            parameters.Add("@SearchKey", filterDTO.SearchTerm);
            parameters.Add("@SortColumn", filterDTO.SortColumn);
            parameters.Add("@SortOrder", filterDTO.SortOrder);
            parameters.Add("@PageNumber", filterDTO.PageNumber);
            parameters.Add("@PageSize", filterDTO.PageSize);

            var result = await _masterdbConnection.QueryAsync<SubscriptionPlansListDTO>(
                storedProcedure,
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result.AsList(); 
        }

        public async Task<JsonModel> SaveOrUpdateSubscriptionPlan(SubscriptionPlansDTO subscriptionDTO)
        {
            DateTime currentDate = DateTime.UtcNow;

            try
            {
                var duplicateCheck = await _dbContext.SubscriptionPlans
                    .FirstOrDefaultAsync(l => l.PlanName == subscriptionDTO.PlanName
                                      && l.IsDeleted == false
                                      && (subscriptionDTO.PlanId == 0
                                          ? l.IsActive == true
                                          : l.Id != subscriptionDTO.PlanId));

                if (duplicateCheck != null)
                {
                    return new JsonModel(
                        new object(),
                        StatusMessage.RecordAlreadyExists.Replace("[string]", StatusMessage.SubscriptionPlan),
                        (int)HttpStatusCode.UnprocessableEntity
                    );
                }

                var res = CreateMasterSubscriptionAsync(subscriptionDTO).Result;

                if (subscriptionDTO.PlanId == 0)
                {
                    var subscriptionEntity = _mapper.Map<SubscriptionPlans>(subscriptionDTO);
                    subscriptionEntity.PriceId = res.PriceId;
                    subscriptionEntity.ProductId = res.ProductId;
                    subscriptionEntity.IsActive = true;

                    var created = CreateSubscriptionPlan(subscriptionEntity);

                    if (created != null)
                    {
                        CreateModuleSubscriptionPlan(
                            new SubscriptionPlanModules { Planid = created.Id },
                            subscriptionDTO.SubscriptionPlanModuleList
                        );
                    }

                    return new JsonModel(false, StatusMessage.APISavedSuccessfully.Replace("[controller]", StatusMessage.SubscriptionPlan), (int)HttpStatusCode.OK);
                }
                else
                {
                    var subscriptionEntity = _dbContext.SubscriptionPlans
                        .FirstOrDefault(l => l.Id == subscriptionDTO.PlanId && l.IsDeleted == false);

                    if (subscriptionEntity == null)
                    {
                        return new JsonModel(false, StatusMessage.ErrorOccured, (int)HttpStatusCode.InternalServerError);
                    }

                    subscriptionEntity.UpdatedAt = currentDate;
                    subscriptionEntity.PlanName = subscriptionDTO.PlanName;
                    subscriptionEntity.PlatFormRate = subscriptionDTO.PlatFormRate;
                    subscriptionEntity.IsMonthly = subscriptionDTO.IsMonthly;
                    subscriptionEntity.PerClient = subscriptionDTO.PerClient;
                    subscriptionEntity.PriceId = res.PriceId;
                    subscriptionEntity.ProductId = res.ProductId;
                    subscriptionEntity.IsLicensed = subscriptionDTO.IsLicensed;

                    Update(subscriptionEntity);

                    var existingModules = _dbContext.SubscriptionPlanModules
                        .Where(a => a.Planid == subscriptionEntity.Id)
                        .ToList();

                    _dbContext.SubscriptionPlanModules.RemoveRange(existingModules);

                    CreateModuleSubscriptionPlan(
                        new SubscriptionPlanModules { Planid = subscriptionEntity.Id },
                        subscriptionDTO.SubscriptionPlanModuleList
                    );

                    return new JsonModel(false, StatusMessage.APIUpdatedSuccessfully.Replace("[controller]", StatusMessage.SubscriptionPlan), (int)HttpStatusCode.OK);
                }
            }
            catch
            {
                return new JsonModel(false, StatusMessage.ErrorOccured, (int)HttpStatusCode.InternalServerError);
            }
        }

        public async Task<stripplandetail> CreateMasterSubscriptionAsync(SubscriptionPlansDTO subscriptionDTO)
        {
            var response = new stripplandetail { PriceId = "", ProductId = "" };

            if (subscriptionDTO == null)
                return response;

            var alreadyExists = await _dbContext.SubscriptionPlans
                .AnyAsync(p => p.PlanName.ToUpper() == subscriptionDTO.PlanName.ToUpper());

            var oldRate = await _dbContext.SubscriptionPlans
                .Where(p => p.Id == subscriptionDTO.PlanId)
                .Select(x => x.PlatFormRate)
                .FirstOrDefaultAsync();

            bool shouldCreate = !alreadyExists || subscriptionDTO.PlatFormRate > oldRate;

            if (!shouldCreate)
                return response;

            // Mocked Stripe logic - replace with actual implementation
            response.PriceId = "price_1P1xjK2eZvKYlo2Cq8lYZB5e";
            response.ProductId = "prod_Nk3hJ9oLwZq4Dd";

            return response;
        }

        public async Task<List<SubscriptionPlanModules>> GetSubscriptionPlanModulesByPlanId(int planId)
        {
            return await _dbContext.SubscriptionPlanModules
                .AsNoTracking()
                .Where(a => a.Planid == planId)
                .ToListAsync();
        }

        public async Task<List<Modules>> GetAllModules()
        {
            return await _context.Modules.AsNoTracking().ToListAsync();
        }

        public async Task<List<SubscriptionPlansListDTO>> GetAllSubscriptionPlans()
        {
            return (await _masterdbConnection.QueryAsync<SubscriptionPlansListDTO>(
                SqlMethod.Get_AllSubscriptionPlansList,
                commandType: CommandType.StoredProcedure
            )).AsList();
        }

        public async Task<SubscriptionPlans> GetById(int id)
        {
            return await _context.SubscriptionPlans
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        }

        public async Task<List<SubscriptionPlanModules>> GetModulesByPlanId(int planId)
        {
            return await _context.SubscriptionPlanModules
                .AsNoTracking()
                .Where(x => x.Planid == planId)
                .ToListAsync();
        }

        public void RemoveModulesByPlanId(int planId)
        {
            var modules = _context.SubscriptionPlanModules
                .Where(x => x.Planid == planId)
                .ToList();

            if (modules.Count > 0)
            {
                _context.SubscriptionPlanModules.RemoveRange(modules);
            }
        }

        public async Task<List<SubscriptionPlans>> GetAllSubscriptionPlan()
        {
            return await _context.SubscriptionPlans.AsNoTracking().ToListAsync();
        }

        public async Task<List<SubscriptionPlanModules>> GetAllSubscriptionPlanModules()
        {
            return await _context.SubscriptionPlanModules.AsNoTracking().ToListAsync();
        }

        public async Task<List<AgencySubscriptionDetail>> GetAllAgencySubscriptionDetails()
        {
            return await _context.AgencySubscriptionDetail.AsNoTracking().ToListAsync();
        }

        public async Task<List<MasterOrganization>> GetAllMasterOrganizations()
        {
            return await _context.MasterOrganization.AsNoTracking().ToListAsync();
        }

        //public async Task<List<SubscriptionModuleListDTO>> GetAllModuleList<T>()
        //{
        //    return (await _dbConnection.QueryAsync<SubscriptionModuleListDTO>(
        //        SqlMethod.GetModuleNames,
        //        new { OrganizationId = 0 },
        //        commandType: CommandType.StoredProcedure
        //    )).AsList();


        //}
        public async Task<List<SubscriptionModuleListDTO>> GetAllModuleList<T>()
        {
            using (var multi = await _dbConnection.QueryMultipleAsync(
                SqlMethod.GetModuleNames,
                new { OrganizationId = 0 },
                commandType: CommandType.StoredProcedure))
            {
                // Read result sets
                var modulePermissions = (await multi.ReadAsync<ModulePermissionsSuperAdmin>()).ToList();
                var screenPermissions = (await multi.ReadAsync<ScreenPermissionsSuperAdmin>()).ToList();
                var actionPermissions = (await multi.ReadAsync<ActionPermissonsSuperAdmin>()).ToList();

                // Build lookups for fast grouping
                var screensLookup = screenPermissions.ToLookup(s => s.ModuleId);
                var actionsLookup = actionPermissions.ToLookup(a => a.ModuleId);

                // Build final module list
                var modules = modulePermissions
                    .GroupBy(m => m.ModuleId)
                    .Select(g => new SubscriptionModuleListDTO
                    {
                        ModuleId = g.Key,
                        ModuleName = g.First().ModuleName,
                        DisplayOrder = g.First().DisplayOrder,
                        ModulePermissions = g.ToList(),
                        ScreenPermissions = screensLookup[g.Key].ToList(),
                        ActionPermissions = actionsLookup[g.Key].ToList()
                    })
                    .OrderBy(m => m.DisplayOrder)
                    .ToList();

                return modules;
            }
        }


        //public async Task<List<SubscriptionOrgListDTO>> GetOrgSubscriptionPlans(FilterDto filterDTO)
        //{
        //    const string storedProcedure = SqlMethod.Get_AgencySubscriptionList;

        //    var parameters = new DynamicParameters();
        //    parameters.Add("@SearchKey", filterDTO.SearchTerm);
        //    parameters.Add("@SortColumn", filterDTO.SortColumn);
        //    parameters.Add("@SortOrder", filterDTO.SortOrder);
        //    parameters.Add("@PageNumber", filterDTO.PageNumber);
        //    parameters.Add("@PageSize", filterDTO.PageSize);

        //    var result = await _dbConnection.QueryAsync<SubscriptionOrgListDTO>(
        //        storedProcedure,
        //        parameters,
        //        commandType: CommandType.StoredProcedure
        //    );

        //    return result.AsList();
        //}

        public async Task<List<Application.Dto.SubscriptionPlan.SubscriptionOrgListDTO>> GetOrgSubscriptionPlans(FilterDto filterDTO)
        {
            const string storedProcedure = SqlMethod.Get_AgencySubscriptionList;

            var parameters = new DynamicParameters();
            parameters.Add("@SearchKey", filterDTO.SearchTerm);
            parameters.Add("@SortColumn", filterDTO.SortColumn);
            parameters.Add("@SortOrder", filterDTO.SortOrder);
            parameters.Add("@PageNumber", filterDTO.PageNumber);
            parameters.Add("@PageSize", filterDTO.PageSize);

            var result = await _masterdbConnection.QueryAsync<Application.Dto.SubscriptionPlan.SubscriptionOrgListDTO>(
                storedProcedure,
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result.AsList();
        }

       
    }
}