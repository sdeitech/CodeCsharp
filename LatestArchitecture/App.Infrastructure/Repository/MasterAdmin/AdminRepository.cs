using App.Application.Dto.Common;
using App.Application.Dto.MasterAdmin;
using App.Application.Interfaces;
using App.Application.Interfaces.Repositories;
using App.Common.Constant;
using App.Domain.Entities;
using App.SharedConfigs.DBContext;
using Dapper;
using Microsoft.EntityFrameworkCore;
using System.Data;
using static App.Common.Constant.Constants;
using System.Net;
using App.Application.Interfaces.Repositories.AuditLogs;


namespace App.Infrastructure.Repository.MasterAdmin
{
    public class AdminRepository : DbConnectionRepositoryBase, IAdminRepository
    {
        private readonly MasterDbContext _masterDbContext;
        private readonly IAuditLogRepository _auditLogRepository ;

        public AdminRepository(MasterDbContext context, IDbConnectionFactory dbConnectionFactory, IAuditLogRepository auditLogRepository)
        : base(context, dbConnectionFactory)
        {
            _masterDbContext = context;
            _auditLogRepository = auditLogRepository;
        }

        #region Add / Update / Toggle Active Status
        public async Task<AdminResponseDto> AddOrUpdateAdminAsync(MasterAdminDto dto, string performedBy)
        {
           

            var parameters = new DynamicParameters();
            parameters.Add("@AgencyAdminId", dto.AgencyAdminId);
            parameters.Add("@FirstName", dto.FirstName);
            parameters.Add("@LastName", dto.LastName);
            parameters.Add("@RoleId", dto.RoleId);
            parameters.Add("@AgencyId", dto.AgencyId);
            parameters.Add("@Notes", dto.Notes);
            parameters.Add("@IsDelete", dto.IsDelete);
            parameters.Add("@PerformedBy", performedBy);
            parameters.Add("@Email", dto.Email);

            // Query for structured response
            var result = await _dbConnection.QueryFirstOrDefaultAsync<AdminResponseDto>(
                SqlMethod.ADM_Admin_AddUpdate,
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result ?? new AdminResponseDto
            {
                Status = "ERROR",
                Message = "No response from stored procedure",
                AgencyAdminId = null
            };
        }

        public async Task<IEnumerable<MasterAdminDto>> GetAllAdminAsync(FilterDto filter)
        {


            var parameters = new DynamicParameters();
            parameters.Add("@SearchTerm", filter.SearchTerm);
            parameters.Add("@SortColumn", filter.SortColumn ?? "AgencyAdminId");
            parameters.Add("@SortOrder", filter.SortOrder ?? "DESC");
            parameters.Add("@PageNumber", filter.PageNumber <= 0 ? 1 : filter.PageNumber);
            parameters.Add("@PageSize", filter.PageSize <= 0 ? 10 : filter.PageSize);

            return await _dbConnection.QueryAsync<MasterAdminDto>(
                SqlMethod.ADM_Admin_GetAll,
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<bool> DeleteAsync(int agencyAdminId,string ipAddress)
        {

            var admin = await _masterDbContext.AgencyAdmins
                                .FirstOrDefaultAsync(a => a.AgencyAdminId == agencyAdminId);

            if (admin == null)
                return false;

            admin.IsDelete = true;
            admin.DeletedAt = DateTime.UtcNow;
            _masterDbContext.AgencyAdmins.Update(admin);
            _auditLogRepository.SaveChangesWithAuditLogs(AuditLogsScreen.OrganizationAdmin, (int)MasterActions.Update, agencyAdminId, null, ipAddress, (int)MasterPortal.SuperAdminPortal, null, null, null);
            return true;
        }

        public async Task<bool> ToggleActiveStatusAsync(int agencyAdminId, bool isActive, string ipAddress)
        {
            var admin = await _masterDbContext.AgencyAdmins
                                .FirstOrDefaultAsync(a => a.AgencyAdminId == agencyAdminId);

            if (admin == null)
                return false;

            admin.IsActive = isActive;
            _masterDbContext.AgencyAdmins.Update(admin);
            _auditLogRepository.SaveChangesWithAuditLogs(AuditLogsScreen.OrganizationAdmin, (int)MasterActions.Update, agencyAdminId, null, ipAddress, (int)MasterPortal.SuperAdminPortal, null, null, null);


            return true;
        }

        #endregion

    }
}
