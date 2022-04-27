using LearningBlog.Models;
using LearningBlog.Utilities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace LearningBlog.Controllers
{
    public class TestController : Controller
    {
        public IActionResult Index()
        {
            string path = "https://localhost:44381/api/user";
            object user = Get(path);
            List<User> all_user = new List<User>();
            JArray userList = JArray.Parse(user.ToString());
            foreach (var i in userList)
            {
                all_user.Add(new User()
                {
                    Id = Guid.Parse(i["_source"]["id"].ToString()),
                    FullName = i["_source"]["fullName"].ToString(),
                    UserName = i["_source"]["userName"].ToString(),
                    Password = i["_source"]["password"].ToString()
                });
            }

            
            return View(all_user);
        }
        public IActionResult Home()
        {
            string path = "https://localhost:44381/api/post";
            object post = Get(path);
            List<Post> all_post = new List<Post>();
            JArray postList = JArray.Parse(post.ToString());
            foreach (var i in postList)
            {
                //Lay userName theo userId cua tung bai post
                string user_id = i["_source"]["user_id"].ToString();
                string get_user_query = @"{
                        ""query"" : {
                          ""match"": {
                               ""id"" : """ + user_id + @"""}
                    }
                    }";
                using JsonDocument user_doc = JsonDocument.Parse(get_user_query);
                var result2 = ElasticSearch.getDataAsync(user_doc, "user");
                JObject jObject2 = JObject.Parse(result2.Result.ToString());
                JArray userList = JArray.Parse(jObject2["hits"]["hits"].ToString());
                string user_name = userList[0]["_source"]["fullName"].ToString();
                //tao list subcontent cua tung bai post
                JArray subContentList = JArray.Parse(i["_source"]["content"].ToString());
                List<PostContent> postContents = new List<PostContent>();
                foreach (var j in subContentList)
                {
                    postContents.Add(new PostContent()
                    {
                        SubTitle = j["subtitle"].ToString(),
                        SubContent = j["subcontent"].ToString()
                    });
                }
                //them bai post vao list voi userName va list subcontent vua lay duoc o tren
                all_post.Add(new Post()
                {
                    Id = Guid.Parse(i["_source"]["id"].ToString()),
                    UserId = Guid.Parse(i["_source"]["user_id"].ToString()),
                    UserName = user_name,
                    Title = i["_source"]["title"].ToString(),
                    Contents = postContents
                });
            }


            return View(all_post);
        }
        public object Get(string path)
        {
            using (WebClient webCli = new WebClient())
            {
                return JsonConvert.DeserializeObject<object>(webCli.DownloadString(path));
            }
        }
    }
}
