using LearningBlog.Models;
using LearningBlog.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace LearningBlog.Controllers
{
    public class TestController : Controller
    {
        public IActionResult Index()
        {
            string path = "https://localhost:44381/api/user";
            using (var client = new HttpClient())
            {
                Task<HttpResponseMessage> res = client.GetAsync(path);
                if (res.Result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    List<User> all_user = new List<User>();
                    JArray userList = JArray.Parse(res.Result.Content.ReadAsStringAsync().Result);
                    foreach (var i in userList)
                    {
                        all_user.Add(new User()
                        {
                            FullName = i["_source"]["FullName"].ToString(),
                            UserName = i["_source"]["UserName"].ToString(),
                            Password = i["_source"]["Password"].ToString()
                        });
                    }
                }
            }
            return View();
        }
        public IActionResult Home()
        {
            //lay thong tin dang nhap
            var session = HttpContext.Session;          // Lấy ISession
            string key = "infor_access";
            string json = session.GetString(key);
            User loginUser;
            if (json != null)
            {
                loginUser  = JsonSerializer.Deserialize<User>(json);
                ViewBag.User = loginUser.FullName;
            }
            else
            {
                loginUser = null;
                ViewBag.User = "";
            }
            //Lay all post
            List<Post> all_post = new List<Post>();
            string path = "https://localhost:44381/api/post";
            using (var client = new HttpClient())
            {
                Task<HttpResponseMessage> res = client.GetAsync(path);
                if (res.Result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var hhh = res.Result.Content.ReadAsStringAsync().Result.ToString();
                    //JObject postListObject = JObject.Parse(hhh);
                    JArray postListArray = JArray.Parse(hhh);
                    foreach (var i in postListArray)
                    {
                        //Lay username
                        string userName = "", userId = i["_source"]["UserId"].ToString();
                        string get_user_query = @"{
                        ""query"" : {
                          ""match"": {
                               ""_id"" : """ + userId + @"""}
                            }
                            }";
                        using JsonDocument user_doc = JsonDocument.Parse(get_user_query);
                        var resultGetUser = ElasticSearch.getDataAsync(user_doc, "user");
                        JObject userListObj = JObject.Parse(resultGetUser.Result.ToString());
                        JArray userListArr = JArray.Parse(userListObj["hits"]["hits"].ToString());
                        userName = userListArr[0]["_source"]["FullName"].ToString();


                        all_post.Add(new Post()
                        {
                            Id = i["_id"].ToString(),
                            Title = i["_source"]["Title"].ToString(),
                            Content = JsonSerializer.Deserialize<List<PostContent>>(i["_source"]["Content"].ToString()),
                            UserId = userId,
                            UserName = userName
                        }) ;
                    }
                }
            }            
            return View(all_post);
        }
    }
}
