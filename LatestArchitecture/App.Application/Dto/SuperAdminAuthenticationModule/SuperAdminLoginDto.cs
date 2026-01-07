using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Dto.SuperAdminAuthenticationModule
{
    namespace App.Application.Dto.AuthenticationModule
    {
        public class SALoginDto
        {
            public string UserName { get; set; }
            public string Password { get; set; }
            public int? OrgnizationId { get; set; }
            public int? ThirdPartyLogin { get; set; }
        }

        public class SAAuthenctication
        {
            public string? GoogleToken { get; set; }
            public string? MicrosoftToken { get; set; }
            public string? FacebookToken { get; set; }

        }

      
        

        public class SAUserResponseModel
        {
            public int UserId { get; set; }
            public string Email { get; set; }
            public string Username { get; set; }
            public int RoleId { get; set; }
            public string RoleName { get; set; }

            public string Password { get; set; }  // hashed password

            public int OffenseLevel { get; set; }      // e.g., severity of login issues
            public int FailedAttempts { get; set; }    // number of failed login attempts

            public DateTime? BlockUntil { get; set; }  // null = not blocked

            public string AccessToken { get; set; }
            public string RefreshToken { get; set; } // null = no expiration

        }





        public class SABruteForceProtectionDto
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


       

        public class SAResetUserPasswordDto
        {
            public string? Email { get; set; }
            public string? ResetPasswordURL { get; set; }
            public string? Password { get; set; }
            public string? Token { get; set; }
            public string? IpAddress { get; set; }
            public string? Source { get; set; }
            public string? SaltName { get; set; }

        }


        public class SAResetPasswordResponseModel
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public string? Email { get; set; }
            public string? UserName { get; set; }
        }


        public class SAForgotPasswordResponseModel
        {
            public int UserId { get; set; }
            public string Email { get; set; }
            public string UserName { get; set; }
            public string ResetToken { get; set; }
            public DateTime ExpiresAt { get; set; }
        }

        public class SAPasswordResetEmailRequest
        {
            public string Email { get; set; }
            public string UserName { get; set; }
            public string OrganizationName { get; set; }
        }
        public class SAVerifyOtpRequest
        {
            public int UserId { get; set; }
            public string Otp { get; set; }
        }

    }
}
