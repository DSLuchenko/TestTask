using System.Collections.Generic;
using System.Threading.Tasks;
using Server.Models.Entities;

namespace Server.Models
{
    public interface IUsersManager
    {
        public List<User> GetAllUsers();
        public Task<List<User>> GetAllUsersAsync();
        public User SetStatus(int id, string status);
        public Task<User> SetStatusAsync(int id, string status);
        public void CreateUser(User user);
        public Task CreateUserAsync(User user);
    }
}
