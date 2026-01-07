namespace App.Application.Dto.Organization
{
    public class OrganizationReponseDto
    {
        public int OrganizationID { get; set; }
        public string OrganizationName { get; set; }
        public int DatabaseID { get; set; }
        public string DatabaseName { get; set; }
        public string Email { get; set; }
        public string ContactPersonFirstName { get; set; }
        public string ContactPersonLastName { get; set; }
        public string ContactPersonPhone { get; set; }
        public int ContactTypeID { get; set; }
        public string ContactTypeName { get; set; }
        public string Timezone { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public int StateID { get; set; }
        public string StateName { get; set; }
        public int CountryID { get; set; }
        public string CountryName { get; set; }
        public string ZipCode { get; set; }
        public string LogoLocalPath { get; set; }
        public string LogoAWSPath { get; set; }
        public string LogoBlobPath { get; set; }
        public string FavIconLocalPath { get; set; }
        public string FavIconAWSPath { get; set; }
        public string FavIconBlobPath { get; set; }
        public string DomainURL { get; set; }
        public string SubDomainName { get; set; }
        public string SUbDomainURL { get; set; }
        public bool IsActive { get; set; }
        public string? PlanName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
