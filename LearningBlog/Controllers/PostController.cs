using LearningBlog.Models;
using LearningBlog.Utilities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
    public class PostController : Controller
    {
        private static HttpClient client;
        private static string IP = "https://34.195.187.172:9200";
        private static string authen = "phunghx:elas@huutam";
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult PostDetail(int Id)
        {
            string data = @"{
                ""query"" : {
                ""match"": {
                    ""id"":" + Id.ToString() + 
                @"}
                }
                }";


            string userData = @"{
                ""query"" : {
                ""match_all"": {}
                }
                }";


            using JsonDocument doc = JsonDocument.Parse(data);
            using JsonDocument userDoc = JsonDocument.Parse(userData);



            var result = ElasticSearch.getDataAsync(userDoc, "user");
            JObject jObject = JObject.Parse(result.Result.ToString());


            List<User> users = new List<User>();
            JArray userList = JArray.Parse(jObject["hits"]["hits"].ToString());
            foreach (var i in userList)
            {
                users.Add(new User()
                {
                    Id = Int32.Parse(i["_source"]["id"].ToString()),
                    Name = i["_source"]["name"].ToString()
                });
            }



            result = ElasticSearch.getDataAsync(doc, "comment");
            jObject = JObject.Parse(result.Result.ToString());


            List<Comment> comments = new List<Comment>();
            JArray commentList = JArray.Parse(jObject["hits"]["hits"].ToString());
            foreach (var i in commentList)
            {
                comments.Add(new Comment()
                {
                    Id = Int32.Parse(i["_source"]["id"].ToString()),
                    Post_Id = Int32.Parse(i["_source"]["post_id"].ToString()),
                    Content = i["_source"]["content"].ToString()
                });
            }


            result = ElasticSearch.getDataAsync(doc, "post");
            jObject = JObject.Parse(result.Result.ToString());


            List<Post> posts = new List<Post>();
            JArray postList = JArray.Parse(jObject["hits"]["hits"].ToString());
            int uid = 0, poId = 0; string name = "";
            List<Comment> cmts = new List<Comment>();
            List<string> subContents = new List<string>();
            foreach (var i in postList)
            {
                uid = Int32.Parse(i["_source"]["user_id"].ToString());
                foreach (var us in users)
                {
                    if (us.Id == uid)
                    {
                        name = us.Name;
                    }
                }
                poId = Int32.Parse(i["_source"]["id"].ToString());
                foreach (var cmt in comments)
                {
                    if (cmt.Post_Id == poId)
                    {
                        cmts.Add(cmt);
                    }
                }

                JArray subCont = JArray.Parse(i["_source"]["content"].ToString());
                foreach (var j in subCont)
                {
                    subContents.Add(j["subcontent"].ToString());
                }
                posts.Add(new Post()
                {
                    User_Id = Int32.Parse(i["_source"]["user_id"].ToString()),
                    User_Name = name,
                    Title = i["_source"]["title"].ToString(),
                    //Content = i["_source"]["content"].ToString(),
                    Content = subContents,
                    Comments = cmts
                });
            }
            return View(posts[0]);
        }
    }
}
