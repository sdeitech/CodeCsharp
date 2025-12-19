﻿﻿using App.Application.Dto.Common;
using App.Application.Dto.MasterAdmin;
using App.Common.Models;


namespace App.Application.Interfaces.Services
{
    public interface IAdminService
    {
        Task<JsonModel> AddOrUpdateAsync(MasterAdminDto masterAdminDto, string performedBy);
        //Task<JsonModel> GetByIdAsync(int adminId);
        Task<JsonModel> GetAllAdminAsync(FilterDto filter);
        Task<JsonModel> DeleteAsync(int agencyAdminId);
        Task<JsonModel> ToggleActiveStatusAsync(int agencyAdminId, bool isActive);
    }
}
