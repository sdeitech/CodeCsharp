﻿﻿using App.Application.Dto.Common;
using App.Application.Dto.MasterAdmin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Interfaces.Repositories
{
    public interface IAdminRepository
    {
        Task<AdminResponseDto> AddOrUpdateAdminAsync(MasterAdminDto masterAdminDto, string performedBy);
        //Task<MasterAdminDto?> GetByIdAsync(int adminId);
        Task<IEnumerable<MasterAdminDto>> GetAllAdminAsync(FilterDto filter);
        Task<bool> DeleteAsync(int agencyAdminId, string ipAddress);
        Task<bool> ToggleActiveStatusAsync(int agencyAdminId, bool isActive, string ipAddress);
    }
}
