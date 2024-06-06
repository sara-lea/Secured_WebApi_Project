using Entities;

namespace Repositories
{
    public interface IUsersRepository
    {
        Task<User> GetById(int id);
        Task<User> Login(User userLogin);
        Task<User> Register(User user);
        Task<User> Update(int id, User userToUpdate);
    }
}