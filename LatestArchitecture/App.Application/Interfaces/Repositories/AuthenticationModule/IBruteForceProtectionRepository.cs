using App.Application.Dto.AuthenticationModule;

namespace App.Application.Interfaces.Repositories.AuthenticationModule
{
    public interface IBruteForceProtectionRepository
    {
        Task<FailedLoginAttemptResponseModel> LoginAttemptAsync(BruteForceProtectionDto bruteForceProtectionDto);
        Task<bool> ResetAttemptsAsync(BruteForceProtectionDto bruteForceProtectionDto);
    }
}
