using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using Server.Models.Entities;
using Server.Tools;

namespace Server.Models
{
    public class DBManager
    {
        private static DBManager instance = null;
        private readonly object dbLock = new object();
        private readonly IConfigurationRoot configuration;

        private DBManager()
        {
            configuration = GetConfiguration();
        }

        public static DBManager GetInstance()
        {
            if (instance == null)
            {
                instance = new DBManager();
            }

            return instance;
        }

        private IConfigurationRoot GetConfiguration()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            return builder.Build();
        }

        private MySqlConnection GetConnetction()
        {
            return new MySqlConnection(configuration.GetSection("ConnectionStrings").GetSection("MySQL").Value);
        }

        public List<User> GetAllUsers()
        {
            try
            {
                List<User> users = new List<User>();

                Monitor.Enter(dbLock);
                using (MySqlConnection connection = GetConnetction())
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
                    Monitor.Exit(dbLock);
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
                User updateUser = new User();

                Monitor.Enter(dbLock);
                using (MySqlConnection connection = GetConnetction())
                {
                    connection.Open();

                    using (MySqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = $@"CALL users_update_user_status({id},'{status}');";

                        int affectedRows = command.ExecuteNonQuery();

                        if (affectedRows != 0)
                        {
                            updateUser = DataStorage.Users.First(u => u.Id == id);
                            updateUser.Status = status;
                        }
                        else
                        {
                            throw new Exception("User not found");
                        }
                    }
                    connection.Close();
                    Monitor.Exit(dbLock);
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
                Monitor.Enter(dbLock);
                using (MySqlConnection connection = GetConnetction())
                {
                    connection.Open();

                    using (MySqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = $@"CALL users_insert_user({user.Id},'{user.Name}','{user.Status}');";

                        int affectedRows = command.ExecuteNonQuery();

                        if (affectedRows != 0)
                        {
                            DataStorage.Users.Add(user);
                        }
                        else
                        {
                            throw new Exception("User not added, database error!");
                        }
                    }
                    connection.Close();
                    Monitor.Exit(dbLock);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
