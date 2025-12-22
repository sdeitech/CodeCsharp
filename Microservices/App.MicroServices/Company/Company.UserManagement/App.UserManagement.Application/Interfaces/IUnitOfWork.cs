namespace App.UserManagement.Application.Interfaces;

public interface IUnitOfWork : IDisposable
{
    public Task<bool> CommitAsync();
}