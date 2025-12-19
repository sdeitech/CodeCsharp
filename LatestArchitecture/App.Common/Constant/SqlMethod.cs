﻿namespace App.Common.Constant
{
    public class SqlMethod
    {
        public const string Get_SubscriptionPlanList = "Get_SubscriptionPlanList";
        public const string Get_AllSubscriptionPlansList = "Get_AllSubscriptionPlansList";
        public const string ORG_SaveAgencySubscriptionDetails = "ORG_SaveAgencySubscriptionDetails";
        public const string Org_GetSubscriptionPlanList = "Org_GetSubscriptionPlanList";
        public const string GetAllSubscriptionPlanList = "GetAllSubscriptionPlanList";
        public const string Sp_AuthenticateUser = "sp_AuthenticateUser";
        public const string GetModuleNames = "GetModuleNames";
        public const string Get_AgencySubscriptionList = "Get_AgencySubscriptionList";
        public const string ORG_GetSubscriptionPlanById = "ORG_GetSubscriptionPlanById";

        //Superadmin Dashboard
        public const string sp_GetSuperadminDashboardTileCounts = "sp_GetSuperadminDashboardTileCounts";
        public const string MST_GetAuditLogsPaged = "MST_GetAuditLogsPaged"; 


        //Authentication Module 
        public const string AuthenticateUser = "sp_AuthenticateUser";
        public const string AuthenticateUser1 = "sp_AuthenticateUser1";
        public const string ForgotPassword = "sp_ForgotPassword";
        public const string ForgotPassword1 = "sp_ForgotPassword1";
        public const string ResetUserPassword = "sp_ResetUserPassword";
        public const string ResetUserPassword1 = "sp_ResetUserPassword1";
        public const string VerifyUserMfaOtp = "sp_VerifyUserMfaOtp";
        public const string SaveUserOtp = "sp_SaveUserOtp";
        public const string GetUserFromEmail = "sp_GetUserForSSO";
        public const string GetOrgSocialSettings = "sp_GetOrgSocialSettings";

        //Authentication Module 
        public const string AuthenticateSAUser = "sp_AuthenticateSAUser";

        //End 

        public const string ORG_SaveOrUpdateOrganization = "ORG_SaveOrUpdateOrganization";
        public const string ORG_GetAllOrganizations = "ORG_GetAllOrganizations";
        public const string ORG_GetOrganizationByID = "ORG_GetOrganizationByID";
        public const string ORG_GetCardStatistics = "ORG_GetCardStatistics";
        public const string ORG_GetStorageConfiguration = "ORG_GetStorageConfiguration";
        public const string ORG_SaveStorageConfiguration = "ORG_SaveStorageConfiguration";
        public const string ORG_DeleteStorageConfiguration = "ORG_DeleteStorageConfiguration";
        public const string ORG_GetAllStorageConfigurations = "ORG_GetAllStorageConfigurations";
        public const string ADT_GetNewAuditLogs = "ADT_GetNewAuditLogs";
        public const string MST_GetAllMasterDatabase = "MST_GetAllMasterDatabase";
        public const string MST_GetMasterDatabaseById = "MST_GetMasterDatabaseById";
        public const string MDB_GetMasterDatabaseCounts = "MDB_GetMasterDatabaseCounts";

        public const string ADT_GetAuditLogs = "ADT_GetAuditLogs";

        public const string GetCurrentSubscriptionPlan = "GetCurrentSubscriptionPlan";

        //master data
        public const string MST_GetMasterData = "MST_GetMasterData";


        #region Dynamic Questionnaire Stored Procedures
        public const string DQ_GetPagedForms = "DQ_GetPagedForms";
        public const string DQ_GetPagedSubmissions = "DQ_GetPagedSubmissions";
        public const string DQ_RecalculateFormScores = "DQ_RecalculateFormScores";
        #endregion


        //MasterAdmin
        public const string ADM_Admin_AddUpdate = "ADM_Admin_AddUpdate";
        public const string ADM_Admin_GetAll = "ADM_Admin_GetAll";
        public const string ADM_MasterSetting_GetAll = "ADM_MasterSetting_GetAll"; 
        public const string ADM_MasterSystemSetting_GetAll = "ADM_MasterSystemSetting_GetAll";

        
    }
} 
