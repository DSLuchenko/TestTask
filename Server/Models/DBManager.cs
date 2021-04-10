using System;
using System.Collections.Generic;
using System.Linq;
using MySqlConnector;
using Server.Models.Entities;
using Server.Tools;

namespace Server.Models
{
    public class DBManager
    {
        private MySqlConnection GetConnection()
        {
            return new MySqlConnection("server=localhost;user=root;password=1234;database=db_test_task");
        }

        public List<User> GetAllUsers()
        {
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

                using (MySqlConnection connection = GetConnection())
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
                            updateUser = null;
                        }
                    }
                    connection.Close();
                }

                return updateUser;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public User CreateUser(User user)
        {
            try
            {
                User createUser = new User();

                using (MySqlConnection connection = GetConnection())
                {
                    connection.Open();

                    using (MySqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = $@"CALL users_insert_user({user.Id},'{user.Name}','{user.Status}');";

                        int affectedRows = command.ExecuteNonQuery();

                        if (affectedRows != 0)
                        {
                            createUser = user;
                            DataStorage.Users.Add(createUser);
                        }
                        else
                        {
                            return null;
                        }
                    }
                    connection.Close();
                }
                return createUser;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
