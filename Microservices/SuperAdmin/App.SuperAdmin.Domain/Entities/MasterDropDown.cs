using System.ComponentModel.DataAnnotations;

namespace App.SuperAdmin.Domain.Entities
{
    public class MasterDropDown : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
