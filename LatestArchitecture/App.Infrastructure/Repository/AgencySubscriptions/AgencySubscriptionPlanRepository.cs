using App.Application.Dto;
using App.Application.Dto.SubscriptionPlan;
using App.Application.Interfaces;
using App.Application.Interfaces.Repositories.AgencySubscriptions;
using App.Common.Constant;
using App.Common.Models;
using App.Domain.Entities.AgencySubscriptions;
using App.Domain.Entities.MasterAdmin;
using App.Infrastructure.DBContext;
using App.SharedConfigs.DBContext;
using Dapper;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Net;

namespace App.Infrastructure.Repository.AgencySubscriptions
{
    public class AgencySubscriptionPlanRepository : DbConnectionRepositoryBase,IAgencySubscriptionPlanRepository
    {
        private ApplicationDbContext _context;
        protected readonly IDbConnection _masterdbConnection;
        protected readonly IDbConnection _dbConnection;
        protected readonly MasterDbContext _dbContext;
        public AgencySubscriptionPlanRepository(ApplicationDbContext context, IDbConnectionFactory dbConnectionFactory, MasterDbContext masterDbContext)
        : base(context, dbConnectionFactory)
        {
            _context=context;
            var appConnString = context.Database.GetDbConnection().ConnectionString;
            _dbConnection = dbConnectionFactory.CreateConnection(appConnString, context.Database.ProviderName);
            _dbContext = masterDbContext;
            // Master DB connection
            var masterConnString = masterDbContext.Database.GetDbConnection().ConnectionString;
            _masterdbConnection = dbConnectionFactory.CreateConnection(masterConnString, masterDbContext.Database.ProviderName);

        }

        public async Task<CurrentSubscriptionPlanDTO> GetCurrentSubscriptionPlan(int id)
        {
            const string storedProcedure = SqlMethod.GetCurrentSubscriptionPlan;

            var parameters = new DynamicParameters();
            parameters.Add("@organizationid", id);

            var result = await _dbConnection.QueryFirstOrDefaultAsync<CurrentSubscriptionPlanDTO>(
                storedProcedure,
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result;
        }

        public async Task<List<SubscriptionPlanListDTO>> GetAllSubscriptionPlan(int id)
        {
            const string storedProcedure = SqlMethod.GetAllSubscriptionPlanList;
            var parameters = new DynamicParameters();
            parameters.Add("@organizationid", id);

            var result = await _dbConnection.QueryAsync<SubscriptionPlanListDTO>(
                storedProcedure,
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result.ToList();
        }
        public async Task<List<OrganizationSubscriptionPlanDetail>> GetSubscriptionAllPlanList(int id)
        {
            const string storedProcedure = SqlMethod.Org_GetSubscriptionPlanList;
            var parameters = new DynamicParameters();
            parameters.Add("@organizationid", id);

            var result = await _masterdbConnection.QueryAsync<OrganizationSubscriptionPlanDetail>(
                storedProcedure,
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result.ToList();
        }
        public async Task<bool> CancelSubscriptionAsync(int id, int? subid)
        {
            try
            {
                var subscription = await _dbContext.AgencySubscriptionDetail
         .AsNoTracking()
         .FirstOrDefaultAsync(x => x.OrganizationID == id);

                var agencyAdmin = await _dbContext.AgencyAdmins
                    .FirstOrDefaultAsync(x => x.AgencyId == id);

                if (subscription == null)
                    return false;

                try
                {
                    // Simulated subscription cancel flag (replace with actual logic if needed)
                   

                    if (subscription != null)
                    {
                        subscription.Status = "3";
                        subscription.CancelledDate = DateTime.UtcNow;
                        subscription.IsActive = false;
                        _dbContext.AgencySubscriptionDetail.Update(subscription);
                    }

                    if (agencyAdmin != null)
                    {
                        agencyAdmin.IsActive = false;
                        _dbContext.AgencyAdmins.Update(agencyAdmin);
                    }
                    await _dbContext.SaveChangesAsync();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }

            return false;
        }

        public async Task<JsonModel> BuyPlan(SessionMetadataModel session)
        {
            try
            {
                if (session == null)
                {
                    return new JsonModel()
                    {
                        Data = null,
                        Message = StatusMessage.NotFound,
                        StatusCode = (int)HttpStatusCode.NotFound
                    };
                }

                //int totalcount = 0;
                ////var PerClientCost = Convert.ToInt32(session.PerClientCost);
                ////var OrganizationId = Convert.ToInt32(session.OrganizationId);
                ////var ClientCount = Convert.ToInt32(session.ClientCount);
                ////var PlanId = Convert.ToInt32(session.PlanId);
                ////var ExtraClientCount = Convert.ToInt32(session.ExtraClientCount);

                //totalcount = ClientCount + ExtraClientCount;

                // Create new subscription entry
                //var data = await _context.AgencySubscriptionDetail.Where(x => x.Id == session.PlanId && x.OrganizationID==session.OrganizationId).FirstOrDefaultAsync();

                //if (data == null)
                //{
                    int agencySubscriptionDetailId = await SaveAgencySubscriptiondetail(session);

                //}


                return new JsonModel()
                {
                    Data = null,
                    Message = "Success",
                    StatusCode = (int)HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new JsonModel()
                {
                    Data = null,
                    Message = "Fail",
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
            }
        }
        public async Task<int> SaveAgencySubscriptiondetail(SessionMetadataModel sessionMetadata)
        {
            const string storedProcedure = SqlMethod.ORG_SaveAgencySubscriptionDetails;

            var parameters = new DynamicParameters();
            parameters.Add("@OrganizationId", sessionMetadata.OrganizationId);
            parameters.Add("@planid", sessionMetadata.PlanId);
            parameters.Add("@CustomerId", sessionMetadata.CustomerId);
            parameters.Add("@SubscriptionId", sessionMetadata.SubscriptionId);
            parameters.Add("@Price", sessionMetadata.Price);
            parameters.Add("@StartDate", sessionMetadata.StartDate);
            parameters.Add("@EndDate", sessionMetadata.EndDate);
            parameters.Add("@Discount", sessionMetadata.Discount);
            parameters.Add("@BillingCycle", sessionMetadata.BillingCycle);
            parameters.Add("@Note", sessionMetadata.Note);
            parameters.Add("@Status", sessionMetadata.Status);
            parameters.Add("@Frequency", sessionMetadata.Frequency);
            if(sessionMetadata.Status==3 && sessionMetadata.SubscriptionId!=0)
            {
                CancelSubscriptionAsync(sessionMetadata.OrganizationId, sessionMetadata.SubscriptionId);

            }

            var result = await _masterdbConnection.QueryFirstOrDefaultAsync<SubscriptionResult>(
                storedProcedure,
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result?.AgencySubscriptionDetailId ?? -1;
        }

        public async Task<AgencySubscriptionDetail?> GetSubscriptionPlanById(int planId, int organizationId)
        {
            const string storedProcedure = SqlMethod.ORG_GetSubscriptionPlanById; //"ORG_GetSubscriptionPlanById";

            var parameters = new DynamicParameters();
            parameters.Add("@PlanId", planId);
            parameters.Add("@OrganizationId", organizationId);

            var result = await _masterdbConnection.QueryFirstOrDefaultAsync<AgencySubscriptionDetail>(
                storedProcedure,
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result;
        }


    }
}
