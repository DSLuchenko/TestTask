﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;
using Server.Models.Entities;
using Server.Tools;

namespace Server.Models
{
    public class UsersManager : IUsersManager
    {
        private readonly object locker = new object();

        private readonly IServiceProvider services;
        private readonly DataStorage dataStorage;

        public UsersManager(IServiceProvider services)
        {
            this.services = services;
            this.dataStorage = this.services.GetRequiredService<DataStorage>();
        }

        private MySqlConnection GetConnection()
        {
            Settings settings = services.GetRequiredService<Settings>();

            return new MySqlConnection(settings.MySqlConnectionStrings);
        }

        public List<User> GetAllUsers()
        {
            try
            {
                List<User> users = new List<User>();

                Monitor.Enter(locker);
                using (MySqlConnection connection = GetConnection())
                {
                    connection.Open();

                    using (MySqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = "CALL users_select_all();";

                        MySqlDataReader reader = command.ExecuteReader();

                        while (reader.Read() == true)
                        {
                            users.Add(new User()
                            {
                                Id = reader.GetInt32("id"),
                                Name = reader.GetString("name"),
                                Status = reader.GetString("status")
                            });
                        }
                        reader.Close();
                    }
                    connection.Close();
                    Monitor.Exit(locker);
                }

                return users;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public User SetStatus(int id, string status)
        {
            try
            {
                User updateUser = dataStorage.Users.FirstOrDefault(u => u.Id == id);

                if (updateUser != null && updateUser.Status == status)
                {
                    return updateUser;
                }

                if (updateUser == null)
                {
                    throw new Exception("User not found");
                }

                Monitor.Enter(locker);
                using (MySqlConnection connection = GetConnection())
                {
                    connection.Open();

                    using (MySqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = $@"CALL users_update_user_status({id},'{status}');";

                        if (command.ExecuteNonQuery() != 0)
                        {
                            updateUser = dataStorage.Users.First(u => u.Id == id);
                            updateUser.Status = status;
                        }
                    }
                    connection.Close();
                    Monitor.Exit(locker);
                }

                return updateUser;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void CreateUser(User user)
        {
            try
            {
                if (dataStorage.Users.FirstOrDefault(u => u.Id == user.Id) != null)
                {
                    throw new Exception($@"User with id {user.Id} already exist");
                }

                Monitor.Enter(locker);
                using (MySqlConnection connection = GetConnection())
                {
                    connection.Open();

                    using (MySqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = $@"CALL users_insert_user({user.Id},'{user.Name}','{user.Status}');";

                        if (command.ExecuteNonQuery() != 0)
                        {
                            dataStorage.Users.Add(user);
                        }
                    }
                    connection.Close();
                    Monitor.Exit(locker);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
