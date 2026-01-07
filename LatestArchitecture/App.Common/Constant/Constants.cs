using Amazon.Auth.AccessControlPolicy;
using Microsoft.Extensions.Configuration;

namespace App.Common.Constant
{
    public static class Constants
    {
        public const string ProviderNameSqlServer = "SqlServer";
        public const string ProviderNamePostgreSQL = "PostgreSQL";

        public static class AuditLogSkipColumns
        {
            public const string CreatedDate = "CreatedDate";
            public const string CreatedBy = "CreatedBy";
            public const string DeletedDate = "DeletedDate";
            public const string DeletedBy = "DeletedBy";
            public const string ModifiedDate = "ModifiedDate";
            public const string ModifiedBy = "ModifiedBy";
            public const string Password = "Password";

            private static readonly HashSet<string> _skipColumns = new HashSet<string>
            {
                CreatedDate,
                CreatedBy,
                DeletedDate,
                DeletedBy,
                ModifiedDate,
                ModifiedBy,
                Password
            };

            public static bool SkipColumns(string columnName)
            {
                return _skipColumns.Contains(columnName);
            }
        }

        public static class AuditLogsScreen
        {
            public const string AddOrganization = "Add Organization";
            public const string ManageOrganization = "Manage Organization";
            public const string UpdateDatabase = "Update Database";
            public const string OrganizationAdmin = "Organization Admin";
            public const string ManageDatabase = "Manage Database";
            public const string SubscriptionPlanManagement = "Subscription Plan Management";
        }
    }

    public static class StatusMessage
    {
        public const string LoginSuccessfully = "Login Successfully.";
        public const string InvalidUserOrPassword = "Invalid username or password.";
        public const string InternalServerError = "Internal Server Error Occured";
        public const string InvalidGoogleToken = "Invalid Google Token";
        public const string InvalidMicrosoftToken = "Invalid Microsoft Token";
        public const string InvalidFacebookToken = "Invalid Facebook Token";
        public const string RecordSavedSuccessfully = "Record has been saved successfully";
        public const string NoDataFound = "No data found.";
        public const string InvalidToken = "Please enter valid token";
        public const string MaxAttemptsReached = "You have reached the maximum number of login attempts. Please try again later.";
        public const string PasswordExpired = "Password expired please click on the Forgot Passowrd and Update your Password";
        public const string AttemptsLeft = "You have {0} attempts. Only {1} left.";

        public const string UserNotFound = "User not found";
        public const string UsernameDoesNotExist = "Username does not exist";
        public const string EmailDoestNotExist = "Email doest not exist";
        public const string RecordFetched = "Record Fetched";
        public const string NoRecordFound = "No Record Found";



        public const string ResetPassword = "Reset password's email sent to user";
        public const string PasswordResetEmailSent = "Password reset email sent.";
        public const string ResetConfirmationEmailSent = "Reset confirmation email sent.";
        public const string OTPSentToEmail = "OTP sent to email.";
        public const string OTPRegeneratedEmail = "OTP regenerated and sent to email.";
        public const string DeleteMessage = "Deleted Successfully";
        public const string FetchMessage = "Fetched Successfully";
        public const string RecordAlreadyExists = "Record already exists";
        public const string SubscriptionPlan = "Subscription Plan";
        public const string APISavedSuccessfully = "[controller] has been saved successfully";
        public const string StatusSuccessfully = "Status has been updated successfully";
        public const string APIUpdatedSuccessfully = "[controller] has been updated successfully";
        public const string ErrorOccured = "Unfortunately, some error was encountered.";
        public const string InvalidData = "Invalid data";
        public const string NotFound = "Not Found";
        public const string PasswordChangedSuccessfully = "Password Changed Successfully";
        public const string DatabaseCreationFailedRollBack = "Failed to create database. Transaction rolled back.";
        public const string ScriptExecutionFailed = "Failed to execute schema script. Transaction rolled back.";
        public const string DatabaseSavedSuccessfully = "Database details saved successfully.";
        public const string DatabaseCreationFailed = "Failed to save database details.";
        public const string CancelSubscription = "Subscription will be cancelled at end of billing period.";
        public const string CancelSubscriptionError = "Cancellation failed or subscription not found.";
        public const string InvalidOTP = "Invalid OTP.";
        public const string ForgotPasswordRequestNotFoundOrExpired = "Forgot password request not found or expired.";
        public const string ResetPasswordEmailSentSuccessfully = "Reset password email sent successfully.";

        //Master Settings Messages
        public const string SaveSuccess = "Master setting saved successfully.";
        public const string UpdateSuccess = "Master setting updated successfully.";
        public const string SaveFailed = "Failed to save master setting.";
        public const string NotFoundMsg = "Master setting not found.";
        public const string InvalidId = "Invalid Id provided.";
        public const string SettingNotFound = "Master setting not found.";
        public const string SettingDeleted = "Master setting deleted successfully.";
        public const string NoSettingsFound = "No settings found.";
        public const string SettingsRetrieved = "Setting retrieved successfully.";
        public const string SettingActivated = "Master setting enabled successfully.";
        public const string SettingDeactivated = "Master setting disabled successfully.";
        public const string SettingExist  = "Setting name already exists. You can only update it.";

        //Master System Setting
        public const string SettingEnabled = "The setting has been enabled successfully.";
        public const string SettingDisabled = "The setting has been disabled successfully.";

        // Master Dashboard Count
        public const string NoContent = "No Content Found.";
        public const string Retrieved = "Retrieved Successfully.";
         

        #region DynamicQuestionnaire Constants
        //Forms Constant Messages
        public const string FormSubmittedSuccessfully = "Form submitted successfully";
        public const string FormCreatedSuccessfully = "Form created successfully";
        public const string FormUpdatedSuccessfully = "Form updated successfully";
        public const string FormRetrievedSuccessfully = "Form retrieved successfully";
        public const string FormRetrievedSuccessfullyUsingStoredProcedure = "Form retrieved successfully using stored procedure";
        public const string FormNotFound = "Form not found";
        public const string FormResponsesRetrievedSuccessfully = "Form responses retrieved successfully";
        public const string ResponseNotFound = "Response not found";
        public const string FormIsAlreadyPublished = "Form is already published";
        public const string FormPublishedSuccessfully = "Form published successfully";
        public const string FormDeletedSuccessfully = "Form deleted successfully";
        public const string QuestionTypesRetrievedSuccessfully = "Question types retrieved successfully";
        public const string FormNotFound_or_NotPublished = "Form not found or not published";
        public const string MultipleSubmissionsAreNotAllowedForThisForm = "Multiple submissions are not allowed for this form";
        public const string ResponseDetailsRetrievedSuccessfully = "Response details retrieved successfully";
        public const string FilterParametersAreRequired = "Filter parameters are required";
        public const string PageNumberAndPageSizeMustBeGreaterThan_0 = "PageNumber and PageSize must be greater than 0";

        //Rules Constant Messages
        public const string A_RuleWithTheSameConditionAlreadyExistsForThisQuestion = "A rule with the same condition already exists for this question";
        public const string RuleCreatedSuccessfully = "Rule created successfully";
        public const string RuleNotFound = "Rule not found";
        public const string NoRulesFoundForEvaluation = "No rules found for evaluation";
        public const string RulesEvaluatedSuccessfully = "Rules evaluated successfully";
        public const string RulesRetrievedSuccessfully = "Rule retrieved successfully";
        public const string RulesDeletedSuccessfully = "Rule deleted successfully";
        public const string RulesUpdatedSuccessfully = "Rule updated successfully";
        
        // Test Mode Constants
        public const string TestModeSubmissionSuccess = "Test mode submission processed successfully";
        public const string TestModeSubmissionError = "An error occurred while processing test mode submission";
        public const string TestModeRuleEvaluationError = "An error occurred while evaluating test mode rules";
        public const string TestModeScoreCalculationSuccess = "Test mode score calculated successfully";
        public const string TestModeScoreCalculationError = "An error occurred while calculating test mode score";
        
        public const string FormTitleCannotBeNullOrWhitespace = "Form title cannot be null, empty, or whitespace.";
        public const string AnErrorOccurredWhileRetrievingQuestionTypes = "An error occurred while retrieving question types.";
        public const string MissingRequiredQuestions = "The following required questions must be answered: {0}";
        public const string QuestionMustHaveAnswer = "Question '{0}' is required and must have an answer";
        public const string QuestionMustHaveValidAnswer = "Question '{0}' is required and must have a valid answer";
        public const string ExportNotFound = "Export not found";
        public const string UnauthorizedAccess = "Unauthorized access to export";

        //Response Messages Constant
        public const string ErrorRetrievingFormResponses = "An error occurred while retrieving form responses";

        //Export Messages Constant
        public const string RequestBodyRequired = "Request body is required";
        public const string ValidFormIdRequired = "Valid Form ID is required";
        public const string ExportFormatRequired = "Export format is required";
        public const string InvalidExportFormat = "Invalid format. Supported formats: CSV, EXCEL, XLSX, PDF";
        public const string ExportIdRequired = "Export ID is required";
        public const string RequestNull = "Export request cannot be null";
        public const string UnsupportedDataType = "Unsupported data type";
        public const string UnsupportedFormat = "Unsupported export format";
        public const string ExportGeneratedSuccessfully = "Export generated successfully";
        public const string ErrorGeneratingExport = "An error occurred while generating export";
        public const string ErrorExportExpired = "Export has expired";
        public const string ExportDownload = "Export ready for download";
        public const string ErrorExportDownload = "An error occurred while downloading export";
        public const string ErrorRetrieveSuccess = "Exports retrieved successfully";
        public const string ErrorRetrieveFailed = "An error occurred while retrieving exports";
        public const string ExportDelete = "Export deleted successfully";
        public const string ErrorExportDelete = "An error occurred while deleting export";
        public const string ExportStatisticsRetrieved = "Export statistics retrieved successfully";
        public const string ExpiredExportsCleanedUp = "Cleaned up {0} expired export files";
        public const string RespondentEmailIsRequired = "Respondent email is required";

        //Scoring Constant Messages
        public const string SubmissionNotFound = "Submission not found";
        public const string ScoreCalculationSuccess = "Score calculated successfully";
        public const string ScoreCalculationError = "An error occurred while calculating score";
        public const string ScoreReCalculationSuccess = "Scores recalculated successfully";
        public const string ScoreReCalculationError = "An error occurred while recalculating scores";
        public const string ScoringUpdatedSuccessfully = "Scoring updated successfully";

        //Catching Exceptions Constants

        public const string AnErrorOccurredWhileCreatingTheForm = "An error occurred while creating the form";
        public const string AnErrorOccurredWhileUpdatingTheForm = "An error occurred while updating the form";
        public const string AnErrorOccurredWhileRetrievingTheForm = "An error occurred while retrieving the form";
        public const string AnErrorOccurredWhileRetrievingForms = "An error occurred while retrieving forms";
        public const string AnErrorOccurredWhilePublishingTheForm = "An error occurred while publishing the form";

        //AnalyticsMessages
        public const string NoSubmissions = "No submissions found for analytics";
        public const string RetrievedSuccessfully = "Analytics retrieved successfully";
        public const string ErrorRetrieving = "An error occurred while getting analytics";
        public const string NoSubmissionsFound = "No submissions found for analytics";
        public const string BreakdownRetrievedSuccessfully = "Score breakdown retrieved successfully";
        public const string ErrorRetrievingBreakdown = "An error occurred while getting score breakdown";
        public const string ErrorUpdating = "An error occurred while updating scoring";
        public const string NoDistributionSubmissionsFound = "No submissions found for distribution";
        public const string ScoreDistributionRetrieved = "Score distribution retrieved successfully";
        public const string ScoreDistributionError = "An error occurred while getting score distribution";

        //Performer
        public const string PerformerRetrievedSuccessfully = "Top performers retrieved successfully";
        public const string PerformerRetrievedError = "An error occurred while getting top performers";

        public const string CreatedSuccessfully = "Record created successfully.";
        public const string UpdatedSuccessfully = "Record updated successfully.";
        public const string OperationFailed = "Operation failed.";

        public const string FetchSuccessfully = "Records fetched successfully.";
        public const string NoRecordsFound = "No records found.";
        public const string DeletedSuccessfully = "Deleted successfully.";

        //Export Submission
        public const string ExportSubmissionError = "No submissions found for export";
        public const string ReportDataExport = "Report data prepared for export";
        public const string ExportReportError = "An error occurred while exporting report";
        public const string UnsupportedExportFormat = "Unsupported export format";
        #endregion

        //Agency Admin
        public const string ActivatedSuccessfully = "Admin activated sucessfully";
        public const string DeactivatedSuccessfully = "Admin deactivated sucessfully";
    }

    public static class Number
    {
        public const int Zero = 0;
        public const int One = 1;
        public const int Two = 2;
        public const int Three = 3;
        public const int Four = 4;
        public const int Five = 5;
        public const int Six = 6;
        public const int Seven = 7;
        public const int Eight = 8;
        public const int Nine = 9;
        public const int Ten = 10;
        public const int Twenty = 20;
        public const int Hundred = 100;
    }


    public static class ImagesPath
    {
        public const string OrganizationImages = "/Images/Organization/"; //its used for both logo and favicon of the organization
        public const string OrganizationImagesLogoS3 = "/Images/Organization/Logo";
        public const string OrganizationImagesFaviconS3 = "/Images/Organization/Favicon";
        public const string OrganizationImagesLogoBlob = "/Images/Organization/Logo";
        public const string OrganizationImagesFaviconBlob = "/Images/Organization/Favicon";
    }

    //These portals are used in audit logs
    public enum MasterPortal
    {
        OrganizationPortal = 1,
        PatientPortal = 2,
        SuperAdminPortal = 3
    }

    //Used for audit logs
    public enum MasterActions
    {
        Add = 1,
        Update = 2,
        Delete = 3
    }

    //Used for audit logs
    public enum Auditlogstatus
    {
        CreatedSuccessfully = 1,
        UpdatedSuccessfully = 2,
        DeletedSuccessfully = 3,
        AlreadyExists = 4,
        SomeErrorOccurred = 5,
        NoDataFound = 6



    }

    public static class EncryptionConfig
    {
        private static IConfiguration _appSettingConfig;

        public static void SetConfiguration(IConfiguration config)
        {
            _appSettingConfig = config;
        }

        public static string Key => _appSettingConfig["EncryptDecryptKey:Secret"];
        public static string IV => _appSettingConfig["EncryptDecryptKey:IV"];
    }



}
