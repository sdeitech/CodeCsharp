using App.Domain.Entities.MasterTimeZone;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace App.Application.Interfaces.Repositories.MasterTimeZone 
{
    public interface IMasterTimeZonesRepository
    {
        Task<List<MasterTimeZones>> GetAllAsync();
        Task<string> GetActiveTimeZoneAsync(CancellationToken ct = default);

    }
}
