namespace App.Application.Interfaces.Services.AuthenticationModule
{
    public interface ICurrentUserClaimService
    {
        int? UserId { get; }
        string? UserName { get; }
        string? Role { get; }
        int? OrganizationId { get; }
    }
}
