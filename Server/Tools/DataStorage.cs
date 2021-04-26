using System.Collections.Generic;
using Server.Models;
using Server.Models.Entities;

namespace Server.Tools
{
    public static class DataStorage
    {
        public static List<User> Users = new List<User>();

        public static void LoadData()
        {
            DBManager db = DBManager.GetInstance();
            List<User> loadUsers = db.GetAllUsers();
            Users = loadUsers;
            loadUsers = null;
        }
    }
}
