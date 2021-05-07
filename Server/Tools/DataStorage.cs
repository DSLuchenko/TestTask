using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Server.Models;
using Server.Models.Entities;

namespace Server.Tools
{
    public class DataStorage
    {
        private readonly IServiceProvider services;

        public DataStorage(IServiceProvider services)
        {
            Users = new List<User>();

            this.services = services;
        }

        public List<User> Users { get; private set; }

        public void LoadData()
        {
            try
            {
                var usersManager = services.GetRequiredService<IUsersManager>();
                List<User> loadUsers = usersManager.GetAllUsers();
                Users = loadUsers;
                loadUsers = null;
            }
            catch
            {
                Users = new List<User>();
            }
        }
    }
}
