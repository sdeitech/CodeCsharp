using App.Application.Dto.AuthenticationModule;
using App.Application.Interfaces.Repositories.AuthenticationModule;
using App.Application.Interfaces.Services.AuthenticationModule;

namespace App.Application.Service.AuthenticationModule
{
    public class BruteForceProtectionService : IBruteForceProtectionService
    {
        public readonly IBruteForceProtectionRepository _bruteForceProtectionReposiotry;
        public BruteForceProtectionService(IBruteForceProtectionRepository bruteForceProtectionReposiotry)
        {
            _bruteForceProtectionReposiotry = bruteForceProtectionReposiotry;

        }

        public async Task<bool> ResetAttemptsAsync(BruteForceProtectionDto bruteForceProtectionDto)
        {
            await _bruteForceProtectionReposiotry.ResetAttemptsAsync(bruteForceProtectionDto);

            return true;
        }


        public async Task<FailedLoginAttemptResponseModel> TrackLoginAttemptAsync(BruteForceProtectionDto bruteForceProtectionDto)
        {
            var isResult = await _bruteForceProtectionReposiotry.LoginAttemptAsync(bruteForceProtectionDto);

            return isResult;
        }

    }
}
