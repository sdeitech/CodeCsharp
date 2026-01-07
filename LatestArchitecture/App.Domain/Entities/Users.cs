using App.Domain.Entities.Organization;

namespace App.Domain.Entities;

public class Users : BaseEntity
{
    public Users(string email, string firstName, string lastName, string password, int role, int status, int? organizationId = null)
    {
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        Password = password;
        Role = role;
        Status = status;
        OrganizationId = organizationId;
    }
    public Users()
    {
    }
    public string Email { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Password { get; private set; }
    public int Role { get; private set; }
    public int Status { get; private set; }
    public DateTimeOffset? LastLoggedinDate { get; private set; }
    public string FullName => $"{FirstName} {LastName}";

    public int? OrganizationId { get; private set; }

    public void SetId(int id)
    {
        typeof(BaseEntity)
            .GetProperty("Id")?
            .SetValue(this, id);
    }
}