using System.Collections.Generic;
using Server.Models.Entities;

namespace Server.Models
{
    public interface IUsersManager
    {
        public List<User> GetAllUsers();
        public User SetStatus(int id, string status);
        public void CreateUser(User user);
    }
}
