using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Client.DTO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Client
{
    class Program
    {
        private static HttpClient client = new HttpClient();

        static async Task Main(string[] args)
        {
            await Run();
        }

        static async Task Run()
        {
            Console.WriteLine("Нажмите любую кнопку для старта");
            Console.ReadKey();

            do
            {
                Console.Clear();
                int numberMethod = SelectMethod();
                int id;
                User user = new User();

                switch (numberMethod)
                {
                    case 1:
                        user = CreateUser();
                        Console.Clear();
                        Console.WriteLine("Response:");
                        Console.WriteLine(await PostCreateUser(user));
                        break;
                    case 2:
                        id = InputId("RemoveUser");
                        Console.Clear();
                        Console.WriteLine("Response:");
                        Console.WriteLine(await PostRemoveUser(id));
                        break;
                    case 3:
                        id = InputId("UserInfo");
                        Console.Clear();
                        Console.WriteLine("Response:");
                        Console.WriteLine(await GetUserInfo(id.ToString()));
                        break;
                    case 4:
                        user = SetStatus();
                        Console.Clear();
                        Console.WriteLine("Response:");
                        Console.WriteLine(await PostSetStatus(user));
                        break;
                }

                Console.WriteLine("Нажмите любую кнопку чтобы продолжить");
                Console.ReadKey();

            } while (true);
        }

        static void CreateRequestHeaders(string appType)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            byte[] byteArray = Encoding.ASCII.GetBytes("admin:admin");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            client.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue(appType));
        }

        static async Task<string> PostRemoveUser(int id)
        {
            try
            {
                //JObject jObject = new JObject(new JProperty("RemoveUser", new JObject(new JProperty("Id", id))));
                var data = JsonConvert.SerializeObject(new JsonRemoveUserDto() { RemoveUser = new RemoveUser() { Id = id } });

                CreateRequestHeaders("application/json");

                HttpContent content = new StringContent(data,
                    Encoding.UTF8,
                    "application/json");
                HttpResponseMessage response = await client.PostAsync("https://localhost:44301/Auth/RemoveUser", content);

                if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.BadRequest)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    return $@"StatusCode: {response.StatusCode}";
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        static async Task<string> PostCreateUser(User newUser)
        {
            try
            {
                CreateRequestHeaders("application/xml");

                /*using MemoryStream stream = new MemoryStream();
                using XmlWriter xml = XmlWriter.Create(stream);
                xml.WriteStartDocument();
                xml.WriteStartElement("Request");
                xml.WriteStartElement("user");
                xml.WriteAttributeString("Id",newUser.Id.ToString());
                xml.WriteAttributeString("Name",newUser.Name);
                xml.WriteElementString("Status", newUser.Status);
                xml.WriteEndElement();
                xml.Close();

                stream.Position = 0;
                HttpResponseMessage response = await client.PostAsync("https://localhost:44301/Auth/CreateUser",
                    new StreamContent(stream));
                stream.Close();*/

                using MemoryStream stream = new MemoryStream();
                XmlSerializer serializer = new XmlSerializer(typeof(XmlRequestDto));
                serializer.Serialize(XmlWriter.Create(stream), new XmlRequestDto() { User = newUser });
                stream.Position = 0;
                HttpResponseMessage response = await client.PostAsync("https://localhost:44301/Auth/CreateUser", new StreamContent(stream));
                stream.Close();

                if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.BadRequest)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    return $@"StatusCode: {response.StatusCode}";
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        static async Task<string> PostSetStatus(User user)
        {
            try
            {
                Dictionary<string, string> form = new Dictionary<string, string>()
                {
                    {"Id", user.Id.ToString() },
                    {"NewStatus", user.Status }
                };

                CreateRequestHeaders("application/x-www-form-urlencoded");

                HttpContent content = new FormUrlEncodedContent(form);
                HttpResponseMessage response = await client.PostAsync("https://localhost:44301/Auth/SetStatus", content);

                if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.BadRequest)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    return $@"StatusCode: {response.StatusCode}";
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        static async Task<string> GetUserInfo(string id)
        {
            try
            {
                CreateRequestHeaders("text/html");

                HttpResponseMessage response = await client.GetAsync($"https://localhost:44301/Public/UserInfo?id={id}");

                if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.BadRequest)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    return $@"StatusCode: {response.StatusCode}";
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }

        }

        static string SelectStatus()
        {
            string status = string.Empty;
            string numberStatus;
            int num;

            do
            {
                Console.Clear();
                Console.Write("1.New\n2.Active\n3.Blocked\n4.Deleted\nВвыберите номер статуса: ");
                numberStatus = Console.ReadLine();
            }
            while (int.TryParse(numberStatus, out num) == false || num > 4 || num <= 0);

            switch (num)
            {
                case 1:
                    status = "New";
                    break;
                case 2:
                    status = "Active";
                    break;
                case 3:
                    status = "Blocked";
                    break;
                case 4:
                    status = "Deleted";
                    break;
            }

            return status;
        }

        static int SelectMethod()
        {
            string numberMethod;
            int num;
            do
            {
                Console.Clear();
                Console.WriteLine("1.CreateUser\n2.RemoveUser\n3.UserInfo\n4.SetStatus");
                Console.WriteLine("Введите номер метода:");

                numberMethod = Console.ReadLine();

            } while (int.TryParse(numberMethod, out num) == false || num > 4 || num <= 0);

            return num;
        }
        static int InputId(string nameMethod)
        {
            string inputId;
            int id;
            do
            {
                Console.Clear();
                Console.WriteLine(nameMethod);
                Console.Write("Введите id: ");
                inputId = Console.ReadLine();

            } while (int.TryParse(inputId, out id) == false);

            return id;
        }
        static User CreateUser()
        {
            string inputId;
            int id;
            string name;
            string status;

            do
            {
                Console.Clear();
                Console.WriteLine("CreateUser");
                Console.Write("Введите id: ");
                inputId = Console.ReadLine();

            } while (int.TryParse(inputId, out id) == false);

            Console.Write("Введите name: ");
            name = Console.ReadLine();
            status = SelectStatus();

            return new User() { Id = id, Name = name, Status = status };
        }
        static User SetStatus()
        {
            string inputId;
            int id;
            do
            {
                Console.Clear();
                Console.WriteLine("SetStatus");
                Console.Write("Введите id: ");
                inputId = Console.ReadLine();

            } while (int.TryParse(inputId, out id) == false);

            string newStatus = SelectStatus();

            return new User() { Id = id, Status = newStatus };
        }

    }
}
