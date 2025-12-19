namespace App.Application.Dto.Organization
{
    public class OrganizationDto
    {
        public int OrganizationID { get; set; }
        public string OrganizationName { get; set; }
        public string ContactPersonFirstName { get; set; }
        public string ContactPersonLastName { get; set; }
        public string ContactPersonPhone { get; set; }
        public string ContactPersonEmail { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public int StateID { get; set; }
        public int CountryID { get; set; }
        public string ZipCode { get; set; }
        public string LogoLocalPath { get; set; }
        public string LogoAWSPath { get; set; }
        public string LogoBlobPath { get; set; }
        public string FavIconLocalPath { get; set; }
        public string FavIconAWSPath { get; set; }
        public string FavIconBlobPath { get; set; }
        public bool IsAWSS3Storage { get; set; }
        public bool IsAzureBlobStorage { get; set; }
        public bool IsLocalStorage { get; set; }
        public int DatabaseID { get; set; }
        public int CreatedBy { get; set; }
        public int ContactTypeID { get; set; }
        public string DomainURL { get; set; }
        public string SubDomainName { get; set; }

    }
}
