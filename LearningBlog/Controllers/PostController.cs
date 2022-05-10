using LearningBlog.Models;
using LearningBlog.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LearningBlog.Controllers
{
    public class PostController : Controller
    {
        [HttpGet]
        public IActionResult PostDetail(string id)
        {
            string path = "https://localhost:44381/api/post/" + id;
            Post post = new Post();
            using (var client = new HttpClient())
            {
                Task<HttpResponseMessage> res = client.GetAsync(path);
                if (res.Result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var result = res.Result.Content.ReadAsStringAsync().Result.ToString();
                    JArray postArray = JArray.Parse(result);
                    //lay useName
                    string userName = "", userId = postArray[0]["_source"]["UserId"].ToString();
                    string get_user_query = @"{
                            ""query"" : {
                              ""match"": {
                                  ""_id"" : """ + userId + @"""}
                       }
                       }";
                    using JsonDocument user_doc = JsonDocument.Parse(get_user_query);
                    var resultUser = ElasticSearch.getDataAsync(user_doc, "user");
                    JObject jObjectUser = JObject.Parse(resultUser.Result.ToString());
                    JArray userList = JArray.Parse(jObjectUser["hits"]["hits"].ToString());
                    userName = userList[0]["_source"]["FullName"].ToString();

                    post = new Post()
                    {
                        Id = postArray[0]["_id"].ToString(),
                        UserId = userId,
                        UserName = userName,
                        Title = postArray[0]["_source"]["Title"].ToString(),
                        Content = System.Text.Json.JsonSerializer.Deserialize<List<PostContent>>(postArray[0]["_source"]["Content"].ToString())
                    };
                }
                return View(post);
            }
        }
        [HttpGet]
        public IActionResult Post()
        {
            //lay thong tin dang nhap
            var session = HttpContext.Session;          // Lấy ISession
            string key = "infor_access";
            string json = session.GetString(key);
            if (json == null)
            {
                return RedirectToAction("Login", "User");
            }
            return View();
        }
        [HttpPost]
        public IActionResult Post(string title, string content)
        {

            var session = HttpContext.Session;          // Lấy ISession
            string key = "infor_access";
            string json = session.GetString(key);
            string userId, userName;
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
                Post post = new Post() 
                {
                    UserId = userId,
                    Title = title,
                    Content = new List<PostContent>()
                    {
                        new PostContent()
                        {
                            Type = "text",
                            SubContent = content
                        }
                    }
                };
                string url = "https://localhost:44381/api/Post";
                using (var client = new HttpClient())
                {
                    string postJson = System.Text.Json.JsonSerializer.Serialize(post);
                    var httpContent = new StringContent(postJson, Encoding.UTF8, "application/json");
                    Task<HttpResponseMessage> res = client.PostAsync(url, httpContent);
                    if (res.Result.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        return View("Post");
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
            }
            catch
            {
                return BadRequest();
            }
        }

    }
}
