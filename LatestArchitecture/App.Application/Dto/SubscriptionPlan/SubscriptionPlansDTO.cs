using System;
using System.Collections.Generic;

namespace App.Application.Dto.SubscriptionPlan
{
    public class SubscriptionPlansDTO
    {
        public int PlanId { get; set; }
        public string PlanName { get; set; }
        public int PerClient { get; set; }
        public int ClientCount { get; set; }
        public decimal PlatFormRate { get; set; }
        public bool IsLicensed { get; set; }
        public bool IsMonthly { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; } = false;
        public List<ModulePermissionDTO> SubscriptionPlanModuleList { get; set; }
    }

    public class stripplandetail
    {
        public string ProductId { get; set; }
        public string PriceId { get; set; }
    }

    public class SubscrptionListDTO
    {
        public int PlanId { get; set; }
        public string PlanName { get; set; }
        public int PerClient { get; set; }
        public int PlatFormRate { get; set; }
        public bool IsLicensed { get; set; }
        public bool IsMonthly { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }
        public decimal TotalRecords { get; set; }
        public long? RowNumber { get; set; }
        public double? TotalPages { get; set; }
    }
    public class ModulePermissionDTO
    {
        public int ModuleId { get; set; }
        public List<ScreenPermissionDTO> Screens { get; set; }
    }

    public class ScreenPermissionDTO
    {
        public int ScreenId { get; set; }
        public List<int> Actions { get; set; }
    }

    public class FilterDTO
    {
        public int pageNumber { get; set; } = 1;
        public int pageSize { get; set; } = 10;
        public string sortColumn { get; set; } = string.Empty;
        public string sortOrder { get; set; } = string.Empty;
        public int? id { get; set; }
    }

    public class ListingFiltterDTO : FilterDTO
    {
        public string SearchKey { get; set; } = string.Empty;
        //public string StartWith { get; set; } = string.Empty;
        //public string Tags { get; set; } = string.Empty;
        //public string LocationIDs { get; set; } = string.Empty;
        //public string IsActive { get; set; }
        //public string RoleIds { get; set; } = string.Empty;
    }

    public class SubscriptionPlansListDTO
    {
        public int PlanId { get; set; }
        public string PlanName { get; set; }
        public int PerClient { get; set; }
        public decimal PlatFormRate { get; set; }
        public bool IsLicensed { get; set; }
        public bool IsMonthly { get; set; }
        public string PriceId { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }
        public decimal TotalRecords { get; set; }
        public long? RowNumber { get; set; }
        public int? ClientCount { get; set; }
        public int? SubscribedCount { get; set; }
        public int?  TotalSubscribedCount { get; set; }
        public double? TotalPages { get; set; }
        
        public List<int> SubscriptionPlanModuleList { get; set; }
        public List<int> SubscriptionPlanScreenList { get; set; }
        public List<int> SubscriptionPlanActionList { get; set; }

    }
    public class SubscriptionOrgListDTO
    {
        public int OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public string PlanName { get; set; }
        public int Price { get; set; }
        public string? FinalPrice { get; set; }
        public int PlanId { get; set; }
        public string Status { get; set; }
        public string NextBilling { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }
        public decimal TotalRecords { get; set; }
        public decimal TotalActive { get; set; }
        public decimal TotalCancelled { get; set; }
        public decimal TotalExpired { get; set; }
        public decimal TotalOnHold { get; set; }
        public decimal TotalExpiringInNext2Months { get; set; }
        public long? RowNumber { get; set; }
        public double? TotalPages { get; set; }
        public List<int> SubscriptionPlanModuleList { get; set; }
        public List<int> SubscriptionPlanScreenList { get; set; }
        public List<int> SubscriptionPlanActionList { get; set; }
    }
    public class SubscriptionResult
    {
        public int AgencySubscriptionDetailId { get; set; }
    }
}
