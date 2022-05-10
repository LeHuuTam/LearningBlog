using LearningBlog.Models;
using LearningBlog.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearningBlog.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(string userName, string password)
        {
            string path = "https://localhost:44381/api/user";
            object post = GetObject.Get(path);
            List<User> all_user = new List<User>();
            JArray userList = JArray.Parse(post.ToString());
            foreach (var i in userList)
            {
                //them bai post vao list voi userName va list subcontent vua lay duoc o tren
                all_user.Add(new User()
                {
                    Id = i["_id"].ToString(),
                    UserName = i["_source"]["UserName"].ToString(),
                    Password = i["_source"]["Password"].ToString(),
                    FullName = i["_source"]["FullName"].ToString(),
                });
            }
            bool check = false;
            User loginUser = null;
            foreach (User i in all_user)
            {
                if (i.UserName == userName && i.Password == password)
                {
                    check = true;
                    loginUser = new User();
                    loginUser = i;
                    break;
                }
            }
            if (check)
            {
                AddSession(loginUser);
                return RedirectToAction("Home", "Test");
            }
            else
            {
                return View("Login");
            }
        }
        public IActionResult Regrister()
        {
            return View();
        }

        public void AddSession(User user)
        {
            var session = HttpContext.Session;
            string key = "infor_access";
            session.SetString(key, JsonConvert.SerializeObject(user));
        }
    }
}
