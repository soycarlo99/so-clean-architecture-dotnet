namespace TaskManagement.Core.Entities;

public class User
{
    public int Id { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string FullName { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;

    private User() { }

    // Public factory method (for users with password - registration)
    public static User Create(string email, string fullName, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));

        if (email.Length > 256)
            throw new ArgumentException("Email cannot exceed 256 characters", nameof(email));

        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Full name cannot be empty", nameof(fullName));

        if (fullName.Length > 100)
            throw new ArgumentException("Full name cannot exceed 100 characters", nameof(fullName));

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash cannot be empty", nameof(passwordHash));

        return new User
        {
            Email = email,
            FullName = fullName,
            PasswordHash = passwordHash,
        };
    }

    public void UpdateEmail(string newEmail)
    {
        if (string.IsNullOrWhiteSpace(newEmail))
            throw new ArgumentException("Email cannot be empty");

        if (newEmail.Length > 256)
            throw new ArgumentException("Email cannot exceed 256 characters");

        Email = newEmail;
    }

    public void UpdateFullName(string newFullName)
    {
        if (string.IsNullOrWhiteSpace(newFullName))
            throw new ArgumentException("Full name cannot be empty");

        if (newFullName.Length > 100)
            throw new ArgumentException("Full name cannot exceed 100 characters");

        FullName = newFullName;
    }
}
