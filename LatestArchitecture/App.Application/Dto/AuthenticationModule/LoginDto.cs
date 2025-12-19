namespace App.Application.Dto.AuthenticationModule
{
    public class LoginDto
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public int? OrgnizationId { get; set; }
        public int? ThirdPartyLogin { get; set; }        
    }

    public class Authenctication
    {
        public string? GoogleToken { get; set; }
        public string? MicrosoftToken { get; set; }
        public string? FacebookToken { get; set; }

    }

    public class OrgSocialSettingsDto
    {
        public bool Google { get; set; }
        public bool Microsoft { get; set; }
        public bool Facebook { get; set; }
    }



    public class AgencySettingDto
    {
        public string OrganizationId { get; set; } = string.Empty;
        public string AgencySettingName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public class FacebookDebugResponse
    {
        public FacebookDebugData? Data { get; set; }
    }

    public class FacebookDebugData
    {
        public bool IsValid { get; set; }
        public string? UserId { get; set; }
        public string? AppId { get; set; }
    }

    public class FacebookUserResponse
    {
        public string? Id { get; set; }
        public string? Email { get; set; }
        public string? First_Name { get; set; }
        public string? Last_Name { get; set; }
    }

    public class UserResponseModel
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public int OrganizationId { get; set; }
        public string Username { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }

        public string Password { get; set; }  // hashed password

        public int OffenseLevel { get; set; }      // e.g., severity of login issues
        public int FailedAttempts { get; set; }    // number of failed login attempts

        public DateTime? BlockUntil { get; set; }  // null = not blocked

        public string AccessToken { get; set; }
        public string RefreshToken { get; set; } // null = no expiration
        public int ThirdPartyLogin { get; set; }
        public DateTime? PasswordModifiedDate { get; set; }

    }





    public class BruteForceProtectionDto
    {

        public int AccountLockOutId { get; set; }

        public string IpAddress { get; set; } = null!;

        public int FailedAttempts { get; set; } = 0;

        public DateTime? BlockUntil { get; set; }

        public DateTime? LastFailedAttempt { get; set; }

        public byte OffenseLevel { get; set; } = 0;

        public string? ClickEvent { get; set; }

        public string? UserName { get; set; }

        public bool IsBlocked { get; set; } = false;

        public bool IsPermanentBlocked { get; set; } = false;

        public DateTime? CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int? UnBlockedBy { get; set; }

        public bool IsSuperAdmin { get; set; } = false;

    }


    public class FailedLoginAttemptResponseModel
    {

        public bool ShouldNotifySuperAdmin { get; set; }
        public int OffenseLevelReturn { get; set; }

    }

    public class ResetUserPasswordDto
    {
        public string? Email { get; set; }
        public string? ResetPasswordURL { get; set; }
        public string? Password { get; set; }
        public string? Token { get; set; }
        public string? IpAddress { get; set; }
        public string? Source { get; set; }
        public string? SaltName { get; set; }

    }


    public class ResetPasswordResponseModel
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string? Email { get; set; }
        public string? UserName { get; set; }
    }


    public class ForgotPasswordResponseModel
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string ResetToken { get; set; }
        public DateTime ExpiresAt { get; set; }
    }

    public class PasswordResetEmailRequest
    {
        public string Email { get; set; }
        public string UserName { get; set; }
        public string OrganizationName { get; set; }
    }
    public class VerifyOtpRequest
    {
        public int UserId { get; set; }
        public string Otp { get; set; }
    }

}
