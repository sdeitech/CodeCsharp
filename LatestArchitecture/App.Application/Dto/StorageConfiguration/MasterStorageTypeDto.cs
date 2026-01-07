namespace App.Application.Dto.StorageConfiguration
{
    public class MasterStorageTypeDto
    {
        public int Id { get; set; }
        public string StorageTypeName { get; set; } = string.Empty;
        public string StorageTypeCode { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
