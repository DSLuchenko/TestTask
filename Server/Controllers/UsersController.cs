using System;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Xml;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Server.Models;
using Server.Models.Entities;
using Server.Tools;

namespace Server.Controllers
{
    [ApiController]
    public class UsersController : ControllerBase
    {
        private DBManager db;

        public UsersController()
        {
            db = new DBManager();
        }

        [HttpPost("Auth/CreateUser")]
        [BasicAuthorize]
        [Produces("application/xml")]
        public ActionResult CreateUser([FromBody] XmlDocument newUser)
        {
            try
            {
                string id = newUser["Request"]["user"].GetAttribute("Id");
                string name = newUser["Request"]["user"].GetAttribute("Name");
                var status = newUser["Request"]["user"]["Status"].InnerText;

                User user = new User()
                {
                    Id = int.Parse(id),
                    Name = name,
                    Status = status
                };

                try
                {
                    if (DataStorage.Users.FirstOrDefault(u => u.Id == user.Id) != null)
                    {
                        throw new Exception();
                    }
                    bool result = db.CreateUser(user);

                    XmlDocument doc = new XmlDocument();
                    XmlElement element1 = doc.CreateElement(string.Empty, "Response", string.Empty);
                    doc.AppendChild(element1);
                    element1.SetAttribute("Success", "true");
                    element1.SetAttribute("ErrorId", "0");
                    XmlElement element2 = doc.CreateElement(string.Empty, "user", string.Empty);
                    element1.AppendChild(element2);
                    element2.SetAttribute("Id", user.Id.ToString());
                    element2.SetAttribute("Name", user.Name);
                    XmlElement element3 = doc.CreateElement(string.Empty, "Status", string.Empty);
                    element3.InnerText = user.Status;
                    element2.AppendChild(element3);

                    return StatusCode(StatusCodes.Status200OK, doc);
                }
                catch
                {
                    XmlDocument doc = new XmlDocument();
                    XmlElement element1 = doc.CreateElement(string.Empty, "Response", string.Empty);
                    doc.AppendChild(element1);
                    element1.SetAttribute("Success", "false");
                    element1.SetAttribute("ErrorId", "1");
                    XmlElement element2 = doc.CreateElement(string.Empty, "ErrorMsg", string.Empty);
                    element1.AppendChild(element2);
                    element2.InnerText = $@"User with id {user.Id} already exist";

                    return StatusCode(StatusCodes.Status500InternalServerError, doc);

                }
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message); ;
            }

        }

        [HttpPost("Auth/RemoveUser")]
        [BasicAuthorize]
        public ActionResult RemoveUser([FromBody] object removeUser)
        {
            string id = JObject.Parse(removeUser.ToString())["RemoveUser"]["Id"].ToString();

            try
            {
                User user = db.SetStatus(int.Parse(id), "Deleted");

                string jsonUser = JsonConvert.SerializeObject(user);
                string response = new JObject(new JProperty("Msg", "User was removed"), new JProperty("Success", true), new JProperty("user", JObject.Parse(jsonUser))).ToString();

                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch
            {
                string response = new JObject(new JProperty("ErrorId", 2), new JProperty("Msg", "User not found"), new JProperty("Success", false)).ToString();
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }


        [HttpPost("Auth/SetStatus")]
        [BasicAuthorize]
        [Produces("application/x-www-form-urlencoded")]
        public void SetStatus()
        {

        }

        [HttpGet("Public/UserInfo/{id}")]
        [BasicAuthorize]
        [Produces("text/html")]
        public void UserInfo(int id)
        {
        }

    }
}
