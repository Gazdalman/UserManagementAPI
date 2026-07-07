using UserManagementAPI.Controllers;
using UserManagementAPI.Model;

namespace UserManagementAPI.Services;

public class UserService : IUserService
{
    private readonly List<User> _users = new();

    public IEnumerable<User> GetAll() => _users;

    public User? GetById(int id) => _users.FirstOrDefault(u => u.Id == id);

    public User Create(User user)
    {
        user.Id = _users.Count + 1;
        _users.Add(user);
        return user;
    }

    public User? Update(int id, User updatedUser)
    {
        var existing = _users.FirstOrDefault(u => u.Id == id);
        if (existing is null)
            return null;

        existing.FirstName = updatedUser.FirstName;
        existing.LastName = updatedUser.LastName;
        existing.Email = updatedUser.Email;

        return existing;
    }

    public string? Delete(int id)
    {
        var user = _users.FirstOrDefault(u => u.Id == id);
        if (user is null)
            return null;

      _users.Remove(user);
      return $"User {user.FirstName} {user.LastName} deleted!";
    }
}
