using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Server.BasicAuth;
using Server.Models;
using Server.Models.Entities;
using Server.Tools;
using Server.Tools.DTO.Request;
using Server.Tools.DTO.Response;

namespace Server.Controllers
{
    [ApiController]
    public class UsersController : Controller
    {
        private readonly IServiceProvider services;
        private readonly IUsersManager usersManager;
        private readonly DataStorage dataStorage;
        public UsersController(IServiceProvider services)
        {
            this.services = services;

            usersManager = this.services.GetRequiredService<IUsersManager>();
            dataStorage = this.services.GetRequiredService<DataStorage>();
        }
        /*
        [HttpPost("Auth/CreateUser")]
        [BasicAuthorize]
        [Produces("application/xml")]
        public IActionResult CreateUser()
        {
            try
            {
                XElement xmlNewUser = XDocument.LoadAsync(Request.Body, LoadOptions.None, CancellationToken.None).Result
                    .Element("Request")
                    .Element("user");

                User newUser = new User()
                {
                    Id = int.Parse(xmlNewUser.Attribute("Id").Value),
                    Name = xmlNewUser.Attribute("Name").Value,
                    Status = xmlNewUser.Element("Status").Value
                };

                usersManager.CreateUser(newUser);

                SuccessCreateUserXml successCreateUser = new SuccessCreateUserXml()
                {
                    User = newUser,
                    ErrId = "0",
                    Status = "true"
                };
                return Ok(successCreateUser);
            }
            catch (Exception e)
            {
                NotSuccessCreateUserXml notSuccessCreateUser = new NotSuccessCreateUserXml()
                {
                    ErrId = "1",
                    ErrorMsg = e.Message,
                    Status = "false"
                };
                return BadRequest(notSuccessCreateUser);
            }

        }
        */
        [HttpPost("Auth/CreateUser")]
        [BasicAuthorize]
        [Produces("application/xml")]
        public async Task<IActionResult> CreateUserAsync()
        {
            try
            {
                XElement xmlNewUser = XDocument.LoadAsync(Request.Body, LoadOptions.None, CancellationToken.None).Result
                    .Element("Request")
                    .Element("user");

                User newUser = new User()
                {
                    Id = int.Parse(xmlNewUser.Attribute("Id").Value),
                    Name = xmlNewUser.Attribute("Name").Value,
                    Status = xmlNewUser.Element("Status").Value
                };

                if (await dataStorage.CheckCreationUserAsync(newUser.Id) == false)
                {
                    await usersManager.CreateUserAsync(newUser);
                }

                SuccessCreateUserXml successCreateUser = new SuccessCreateUserXml()
                {
                    User = newUser,
                    ErrId = "0",
                    Status = "true"
                };
                return Ok(successCreateUser);
            }
            catch (Exception e)
            {
                NotSuccessCreateUserXml notSuccessCreateUser = new NotSuccessCreateUserXml()
                {
                    ErrId = "1",
                    ErrorMsg = e.Message,
                    Status = "false"
                };
                return BadRequest(notSuccessCreateUser);
            }

        }

        /*[HttpPost("Auth/RemoveUser")]
        [BasicAuthorize]
        [Produces("application/json")]
        public IActionResult RemoveUser([FromBody] RemoveUserJson jsonData)
        {
            try
            {
                User user = usersManager.SetStatus(jsonData.RemoveUser.Id, "Deleted");

                SuccessRemoveUserJson successRemoveUser = new SuccessRemoveUserJson()
                {
                    Msg = "User was removed",
                    Success = true,
                    User = user
                };
                return Ok(successRemoveUser);
            }
            catch (Exception e)
            {
                NotSuccessRemoveUserJson notSuccessRemoveUser = new NotSuccessRemoveUserJson()
                {
                    ErrorId = 2,
                    Msg = e.Message,
                    Success = false
                };
                return BadRequest(notSuccessRemoveUser);
            }
        }*/
        [HttpPost("Auth/RemoveUser")]
        [BasicAuthorize]
        [Produces("application/json")]
        public async Task<IActionResult> RemoveUserAsync([FromBody] RemoveUserJson jsonData)
        {
            try
            {
                User user = await dataStorage.CheckStatusUserAsync(jsonData.RemoveUser.Id, "Deleted") ??
                            await usersManager.SetStatusAsync(jsonData.RemoveUser.Id, "Deleted");

                SuccessRemoveUserJson successRemoveUser = new SuccessRemoveUserJson()
                {
                    Msg = "User was removed",
                    Success = true,
                    User = user
                };
                return Ok(successRemoveUser);
            }
            catch (Exception e)
            {
                NotSuccessRemoveUserJson notSuccessRemoveUser = new NotSuccessRemoveUserJson()
                {
                    ErrorId = 2,
                    Msg = e.Message,
                    Success = false
                };
                return BadRequest(notSuccessRemoveUser);
            }
        }


        /*[HttpPost("Auth/SetStatus")]
        [BasicAuthorize]
        [Produces("application/json")]
        public IActionResult SetStatus([FromForm] IFormCollection formData)
        {
            try
            {
                string id = formData["Id"];
                string status = formData["NewStatus"];

                User user = usersManager.SetStatus(int.Parse(id), status);

                IFormCollection responseData = new FormCollection(new Dictionary<string, StringValues>()
                {
                    {"Id", user.Id.ToString()},
                    {"Name", user.Name},
                    {"Status", user.Status}
                });
                return Ok(JsonConvert.SerializeObject(responseData));
            }
            catch (Exception e)
            {
                NotSuccessRemoveUserJson notSuccessRemoveUser = new NotSuccessRemoveUserJson()
                {
                    ErrorId = 2,
                    Msg = e.Message,
                    Success = false
                };
                return BadRequest(notSuccessRemoveUser);
            }
        }*/

        [HttpPost("Auth/SetStatus")]
        [BasicAuthorize]
        [Produces("application/json")]
        public async Task<IActionResult> SetStatusAsync([FromForm] IFormCollection formData)
        {
            try
            {
                int id = int.Parse(formData["Id"]);
                string status = formData["NewStatus"];

                User user = await dataStorage.CheckStatusUserAsync(id, status) ??
                            await usersManager.SetStatusAsync(id, status);

                IFormCollection responseData = new FormCollection(new Dictionary<string, StringValues>()
                {
                    {"Id", user.Id.ToString()},
                    {"Name", user.Name},
                    {"Status", user.Status}
                });
                return Ok(JsonConvert.SerializeObject(responseData));
            }
            catch (Exception e)
            {
                NotSuccessRemoveUserJson notSuccessRemoveUser = new NotSuccessRemoveUserJson()
                {
                    ErrorId = 2,
                    Msg = e.Message,
                    Success = false
                };
                return BadRequest(notSuccessRemoveUser);
            }
        }

        /* [HttpGet("Public/UserInfo")]
         [Produces("text/html")]
         public IActionResult UserInfo(int id)
         {
             try
             {
                 User user = dataStorage.Users.First(u => u.Id == id);
                 ViewBag.Title = "UserInfo";
                 return View(user);
             }
             catch (Exception e)
             {
                 ViewBag.Title = "Error";
                 ViewBag.Message = e.Message;

                 return View("Error");
             }
         }*/

        [HttpGet("Public/UserInfo")]
        [Produces("text/html")]
        public async Task<IActionResult> UserInfoAsync(int id)
        {
            try
            {
                User user = await dataStorage.FindUserAsync(id);

                ViewBag.Title = "UserInfo";
                return View(user);
            }
            catch (Exception e)
            {
                ViewBag.Title = "Error";
                ViewBag.Message = e.Message;

                return View("Error");
            }
        }
    }
}
