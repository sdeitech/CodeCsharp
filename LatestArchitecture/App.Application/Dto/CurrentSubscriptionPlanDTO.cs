using System;

namespace App.Application.Dto
{
    public class CurrentSubscriptionPlanDTO
    {
        public string Status { get; set; }
        public string PlanName { get; set; }
        public int PlanId { get; set; }
        public int OrganizationId { get; set; }
        public string CreatedDate { get; set; }
        public decimal Price { get; set; }
        public string CurrentPeriod { get; set; }
        public string Interval { get; set; }
        public string BillDate { get; set; }
        public string PaymentGateway { get; set; }
        public string OrganizationName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string StateName { get; set; }
        public string CountryName { get; set; }
        public string Zip { get; set; }
        public int ClientCount { get; set; }
        public int ActualCount { get; set; }
        public int AgencySubscriptionDetailID {  get; set; }
        public string CustomerId { get; set; }
        public string SubscriptionID {  get; set; }

        public DateTime? CancelledDate {  get; set; }
        public bool IsActive {  get; set; }

    }
    public class SubscriptionPlanListDTO
    {
        public int PlanId { get; set; }
        public string PlanName { get; set; }
        public int ClientCount { get; set; }

        public decimal PlatFormRate { get; set; }
        public bool IsMonthly { get; set; }
        public bool IsLicensed { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }

    }
    public class OrganizationSubscriptionPlanDetail
    {
        public int PlanId { get; set; }
        public string PlanName { get; set; }
        public int PerClient { get; set; }
        public decimal PlatFormRate { get; set; }
        public bool IsLicensed { get; set; }
        public bool IsMonthly { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime DeletedDate { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int DeletedBy { get; set; }
        public bool IsSelected { get; set; }
        public bool isCreditcard { get; set; }
        public string ModuleName { get; set; }
        public int ClientCount { get; set; }
        public int ActualClient { get; set; }
        public  string PriceId {  get; set; }
        public string ProductId {  get; set; }
        public DateTime? CancelledDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }
        public string BillingCycle { get; set; }
        public int SubscriptionPlanId { get; set; }

    }
}
