using UserManagementAPI.Model;

namespace UserManagementAPI.Services;
public interface IUserService
{
    IEnumerable<User> GetAll();
    User? GetById(int id);
    User Create(User user);
    User? Update(int id, User updatedUser);
    string Delete(int id);
}
