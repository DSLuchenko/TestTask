using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Server.BasicAuth;
using Server.Models;
using Server.Models.Entities;
using Server.Tools;
using Server.Tools.Communication.Request;
using Server.Tools.Communication.Response;

namespace Server.Controllers
{
    [ApiController]
    public class UsersController : Controller
    {
        private DBManager db;

        public UsersController()
        {
            db = new DBManager();
        }

        [HttpPost("Auth/CreateUser")]
        [BasicAuthorize]
        [Produces("application/xml")]
        public IActionResult CreateUser([FromBody] CreateUserXml xmlData)
        {
            try
            {
                User newUser = xmlData.User;

                if (DataStorage.Users.FirstOrDefault(u => u.Id == newUser.Id) != null)
                {
                    throw new Exception($@"User with id {newUser.Id} already exist");
                }

                User addedUser = db.CreateUser(newUser);

                if (addedUser == null)
                {
                    throw new Exception("User not added, database error!");
                }

                SuccessCreateUserXml successCreateUser = new SuccessCreateUserXml()
                {
                    User = addedUser,
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

        [HttpPost("Auth/RemoveUser")]
        [BasicAuthorize]
        [Produces("application/json")]
        public IActionResult RemoveUser([FromBody] RemoveUserJson jsonData)
        {
            try
            {
                User user = db.SetStatus(jsonData.RemoveUser.Id, "Deleted");

                if (user == null)
                {
                    throw new Exception("User not found");
                }

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


        [HttpPost("Auth/SetStatus")]
        [BasicAuthorize]
        [Produces("application/json")]
        public IActionResult SetStatus([FromForm] IFormCollection formData)
        {
            try
            {
                string id = formData["Id"];
                string status = formData["NewStatus"];

                User user = db.SetStatus(int.Parse(id), status);

                if (user == null)
                {
                    throw new Exception("User not found");
                }

                Dictionary<string, StringValues> formResponse = new Dictionary<string, StringValues>
                {
                    {"Id", user.Id.ToString()},
                    {"Name", user.Name},
                    {"Status", user.Status}
                };

                IFormCollection response = new FormCollection(formResponse);

                return Ok(JsonConvert.SerializeObject(response));
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

        [HttpGet("Public/UserInfo")]
        [Produces("text/html")]
        public IActionResult UserInfo(int id)
        {
            try
            {
                User user = DataStorage.Users.FirstOrDefault(u => u.Id == id);
                if (user==null)
                {
                    throw new Exception("User not found");
                }
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
