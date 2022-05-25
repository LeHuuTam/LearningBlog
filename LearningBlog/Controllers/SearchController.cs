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
    public class SearchController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public object Get(string path)
        {
            using (WebClient webCli = new WebClient())
            {
                return JsonConvert.DeserializeObject<object>(webCli.DownloadString(path));
            }
        }
        public IActionResult Search(string searchText)
        {
            string path = "https://localhost:44381/api/post/search?searchText=" + searchText;
            object post = Get(path);
            List<Post> all_post = new List<Post>();
            JArray postList = JArray.Parse(post.ToString());
            foreach (var i in postList)
            {
                //Lay userName theo userId cua tung bai post
                string user_id = i["_source"]["UserId"].ToString();
                string get_user_query = @"{
                        ""query"" : {
                          ""match"": {
                               ""_id"" : """ + user_id + @"""}
                    }
                    }";
                using JsonDocument user_doc = JsonDocument.Parse(get_user_query);
                var result2 = ElasticSearch.getDataAsync(user_doc, "user");
                JObject jObject2 = JObject.Parse(result2.Result.ToString());
                JArray userListArr = JArray.Parse(jObject2["hits"]["hits"].ToString());
                string userName = "";
                if (userListArr.Count == 0)
                    userName = "";
                else
                    userName = userListArr[0]["_source"]["FullName"].ToString();
                //tao list subcontent cua tung bai post
                JArray subContentList = JArray.Parse(i["_source"]["Content"].ToString());
                List<PostContent> postContents = new List<PostContent>();
                foreach (var j in subContentList)
                {
                    postContents.Add(new PostContent()
                    {
                        SubTitle = j["SubTitle"].ToString(),
                        SubContent = j["SubContent"].ToString()
                    });
                }
                //them bai post vao list voi userName va list subcontent vua lay duoc o tren
                all_post.Add(new Post()
                {
                    Id = i["_id"].ToString(),
                    UserId = i["_source"]["UserId"].ToString(),
                    UserName = userName,
                    Title = i["_source"]["Title"].ToString(),
                    Content = System.Text.Json.JsonSerializer.Deserialize<List<PostContent>>(i["_source"]["Content"].ToString())
                });

            }
            return View(all_post);
            
        }
    }
}
