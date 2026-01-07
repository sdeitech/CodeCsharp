using App.Application.Dto.AuthenticationModule;

namespace App.Application.Interfaces.Services.AuthenticationModule
{
    public interface IBruteForceProtectionService
    {
        Task<FailedLoginAttemptResponseModel> TrackLoginAttemptAsync(BruteForceProtectionDto bruteForceProtectionDto);
        Task<bool> ResetAttemptsAsync(BruteForceProtectionDto bruteForceProtectionDto);
    }
}
