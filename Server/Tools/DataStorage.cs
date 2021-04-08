using System.Collections.Generic;
using Server.Models;
using Server.Models.Entities;

namespace Server.Tools
{
    public static class DataStorage
    {
        public static List<User> Users { get; set; }

        public static void LoadData()
        {
            Users = null;

            DBManager db = new DBManager();
            Users = db.GetAllUsers();
        }
    }
}
