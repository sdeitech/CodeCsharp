using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.SuperAdmin.Application.Dto
{
    public record MasterDto(
        int Id,
        int MasterDropDownId, 
        string Name,
        bool IsActive
    );
}
