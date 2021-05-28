using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;
using Server.Models.Entities;
using Server.Tools;

namespace Server.Models
{
    public class UsersManager : IUsersManager
    {
        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1);

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
            semaphore.Wait();
            try
            {
                List<User> users = new List<User>();

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
                }
                return users;
            }
            finally
            {
                semaphore.Release();
            }
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            await semaphore.WaitAsync();
            try
            {
                List<User> users = new List<User>();

                using (MySqlConnection connection = GetConnection())
                {
                    connection.Open();

                    using (MySqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = "CALL users_select_all();";

                        MySqlDataReader reader = await command.ExecuteReaderAsync();

                        while (reader.Read() == true)
                        {
                            users.Add(new User()
                            {
                                Id = reader.GetInt32("id"),
                                Name = reader.GetString("name"),
                                Status = reader.GetString("status")
                            });
                        }
                        await reader.CloseAsync();
                    }
                    await connection.CloseAsync();
                }

                return users;
            }
            finally
            {
                semaphore.Release();
            }
        }

        public User SetStatus(int id, string status)
        {
            semaphore.Wait();
            try
            {
                User updateUser = new User();

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

                }
                return updateUser;
            }
            finally
            {
                semaphore.Release();
            }
        }
        public async Task<User> SetStatusAsync(int id, string status)
        {
            await semaphore.WaitAsync();
            try
            {
                User updateUser = new User();

                using (MySqlConnection connection = GetConnection())
                {
                    connection.Open();

                    using (MySqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = $@"CALL users_update_user_status({id},'{status}');";

                        if (await command.ExecuteNonQueryAsync() != 0)
                        {
                            updateUser = dataStorage.Users.First(u => u.Id == id);
                            updateUser.Status = status;
                        }
                    }
                    await connection.CloseAsync();
                }
                return updateUser;
            }
            finally
            {
                semaphore.Release();
            }
        }

        public void CreateUser(User user)
        {
            semaphore.Wait();
            try
            {
                if (dataStorage.Users.FirstOrDefault(u => u.Id == user.Id) != null)
                {
                    throw new Exception($@"User with id {user.Id} already exist");
                }
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
                }
            }
            finally
            {
                semaphore.Release();
            }
        }
        public async Task CreateUserAsync(User user)
        {
            await semaphore.WaitAsync();
            try
            {
                using (MySqlConnection connection = GetConnection())
                {
                    connection.Open();

                    using (MySqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = $@"CALL users_insert_user({user.Id},'{user.Name}','{user.Status}');";

                        if (await command.ExecuteNonQueryAsync() != 0)
                        {
                            dataStorage.Users.Add(user);
                        }
                    }
                    await connection.CloseAsync();
                }
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}
