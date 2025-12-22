namespace App.UserManagement.Domain.Entities;

public class Users(string email, string firstName, string lastName, string password, int role, int status) : BaseEntity
{
    public string Email { get; private set; } = email;
    public string FirstName { get; private set; } = firstName;
    public string LastName { get; private set; } = lastName;
    public string Password { get; private set; } = password;
    public int Role { get; private set; } = role;
    public int Status { get; private set; } = status;
    public DateTimeOffset? LastLoggedinDate { get; private set; }
    public string FullName => $"{FirstName} {LastName}";
}