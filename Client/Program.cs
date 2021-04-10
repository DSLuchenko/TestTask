using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Client.Entities;
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
                        Console.WriteLine(await PostCreateUser(user.Id.ToString(), user.Name, user.Status));
                        break;
                    case 2:
                        id = InputId();
                        Console.Clear();
                        Console.WriteLine("Response:");
                        Console.WriteLine(await PostRemoveUser(id));
                        break;
                    case 3:
                        id = InputId();
                        Console.Clear();
                        Console.WriteLine("Response:");
                        Console.WriteLine(await GetUserInfo(id.ToString()));
                        break;
                    case 4:
                        user = SetStatus();
                        Console.Clear();
                        Console.WriteLine("Response:");
                        Console.WriteLine(await PostSetStatus(user.Id.ToString(), user.Status));
                        break;
                }

                Console.WriteLine("Нажмите любую кнопку чтобы продолжить");
                Console.ReadKey();

            } while (true);
        }

        static async Task<string> PostRemoveUser(int id)
        {
            JObject jObject = new JObject(new JProperty("RemoveUser", new JObject(new JProperty("Id", id))));

            client.DefaultRequestHeaders.Accept.Clear();
            byte[] byteArray = Encoding.ASCII.GetBytes("admin:admin");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            client.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpContent content = new StringContent(JsonConvert.SerializeObject(jObject),
                Encoding.UTF8,
                "application/json");
            HttpResponseMessage response = await client.PostAsync("https://localhost:44301/Auth/RemoveUser", content);

            return await response.Content.ReadAsStringAsync();
        }

        static async Task<string> PostCreateUser(string id, string name, string status)
        {
            XmlDocument doc = new XmlDocument();
            XmlElement element1 = doc.CreateElement(string.Empty, "Request", string.Empty);
            doc.AppendChild(element1);
            XmlElement element2 = doc.CreateElement(string.Empty, "user", string.Empty);
            element1.AppendChild(element2);
            element2.SetAttribute("Id", id);
            element2.SetAttribute("Name", name);
            XmlElement element3 = doc.CreateElement(string.Empty, "Status", string.Empty);
            element3.InnerText = status;
            element2.AppendChild(element3);


            client.DefaultRequestHeaders.Accept.Clear();
            byte[] byteArray = Encoding.ASCII.GetBytes("admin:admin");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            client.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/xml"));

            HttpContent content = new StringContent(doc.InnerXml, Encoding.UTF8,
                "application/xml");

            HttpResponseMessage response = await client.PostAsync("https://localhost:44301/Auth/CreateUser", content);

            return await response.Content.ReadAsStringAsync();
        }

        static async Task<string> PostSetStatus(string id, string newStatus)
        {
            Dictionary<string, string> form = new Dictionary<string, string>();
            form.Add("Id", id);
            form.Add("NewStatus", newStatus);

            client.DefaultRequestHeaders.Accept.Clear();
            byte[] byteArray = Encoding.ASCII.GetBytes("admin:admin");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            client.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

            HttpContent content = new FormUrlEncodedContent(form);
            HttpResponseMessage response = await client.PostAsync("https://localhost:44301/Auth/SetStatus", content);

            return await response.Content.ReadAsStringAsync();
        }

        static async Task<string> GetUserInfo(string id)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("text/html"));

            HttpResponseMessage response = await client.GetAsync($"https://localhost:44301/Public/UserInfo?id={id}");

            return await response.Content.ReadAsStringAsync();
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
        static int InputId()
        {
            string inputId;
            int id;
            do
            {
                Console.Clear();
                Console.WriteLine("RemoveUser");
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
