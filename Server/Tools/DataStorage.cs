using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public void ReloadData()
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
                Users = null;
            }
        }
        public async Task ReloadDataAsync()
        {
            try
            {
                await LoadDataAsync();
            }
            catch
            {
                Users = null;
            }
        }
        private async Task LoadDataAsync()
        {
            var usersManager = services.GetRequiredService<IUsersManager>();
            List<User> loadUsers = await usersManager.GetAllUsersAsync();
            Users = loadUsers;
            loadUsers = null;
        }


        public async Task<User> CheckStatusUserAsync(int id, string status)
        {
            try
            {
                User updateUser = await FindUserAsync(id);

                if (updateUser != null && updateUser.Status == status)
                {
                    return updateUser;
                }

                return null;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<bool> CheckCreationUserAsync(int id)
        {
            try
            {
                User user = await CheckUserByIdAsync(id);

                if (user != null)
                {
                    throw new Exception($@"User with id {id} already exist");
                }

                return false;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<User> FindUserAsync(int id)
        {
            try
            {
                User user = await CheckUserByIdAsync(id);

                if (user == null)
                {
                    throw new Exception("User not found");
                }

                return user;

            }
            catch (Exception e)
            {
                throw;
            }
        }

        private async Task<User> CheckUserByIdAsync(int id)
        {
            try
            {
                if (Users == null)
                {
                    await LoadDataAsync();
                }

                User user = await Task.Factory.StartNew(() => Users.FirstOrDefault(u => u.Id == id));

                return user;

            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
