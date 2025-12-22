using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.SuperAdmin.Domain.Entities
{
    public class DropDownDetails(string name, int masterDropDownId, int sortOrder, bool isActive) : BaseEntity
    {
        public string Name { get; private set; } = name;
        public int MasterDropDownId { get; private set; } = masterDropDownId;
        public int SortOrder { get; private set; } = sortOrder;
        public bool IsActive { get; private set; } = isActive;
    }
}
