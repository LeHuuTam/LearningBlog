using LearningBlog.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LearningBlog.Controllers
{
    public class CommentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(string cmtText, string postId)
        {

            var session = HttpContext.Session;          // Lấy ISession
            string key = "infor_access";
            string json = session.GetString(key);
            string userId;
            if (json == null)
            {
                return RedirectToAction("Login", "User");
            }
            else
            {
                User loginUser = JsonConvert.DeserializeObject<User>(json);
                userId = loginUser.Id.ToString();
                //userName = loginUser.FullName.ToString();
            }

            try
            {
                if (!String.IsNullOrEmpty(cmtText))
                {
                    Comment cmt = new Comment()
                    {
                        UserId = userId,
                        PostId = postId,
                        Content = cmtText
                    };
                    string url = "https://localhost:44381/api/Comment";
                    using (var client = new HttpClient())
                    {
                        string CmtJson = System.Text.Json.JsonSerializer.Serialize(cmt);
                        var httpContent = new StringContent(CmtJson, Encoding.UTF8, "application/json");
                        Task<HttpResponseMessage> res = client.PostAsync(url, httpContent);
                        if (res.Result.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            return Redirect("/post/postdetail/" + postId);
                        }
                        else
                        {
                            return BadRequest();
                        }
                    }
                }
                else return BadRequest();
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
