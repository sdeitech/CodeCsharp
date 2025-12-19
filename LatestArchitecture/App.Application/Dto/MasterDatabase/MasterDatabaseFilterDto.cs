using App.Application.Dto.Common;

namespace App.Application.Dto.MasterDatabase
{
    public class MasterDatabaseFilterDto : FilterDto
    {
        public int? ParentOrganizationID { get; set; }
    }
}
