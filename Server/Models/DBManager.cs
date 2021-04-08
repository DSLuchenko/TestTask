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
        public List<User> GetAllUsers()
        {
            List<User> users = new List<User>();

            using (MySqlConnection connection = new MySqlConnection("server=localhost;user=root;password=1234;database=db_test_task"))
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

        public User SetStatus(int id, string status)
        {
            using (MySqlConnection connection = new MySqlConnection("server=localhost;user=root;password=1234;database=db_test_task"))
            {
                connection.Open();

                using (MySqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = $@"CALL users_update_user_status({id},'{status}');";

                    int affectedRows = command.ExecuteNonQuery();

                    if (affectedRows != 0)
                    {
                        DataStorage.Users.FirstOrDefault(u => u.Id == id).Status = status;
                    }
                }
                connection.Close();
            }

            return DataStorage.Users.FirstOrDefault(u => u.Id == id);
        }

        public bool CreateUser(User user)
        {
            using (MySqlConnection connection = new MySqlConnection("server=localhost;user=root;password=1234;database=db_test_task"))
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
                        return false;
                    }
                }
                connection.Close();
            }

            return true;
        }
    }
}
