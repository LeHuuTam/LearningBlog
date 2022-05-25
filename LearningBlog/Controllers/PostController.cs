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
                    //lay useName cua post
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

                    if (userList.Count == 0)
                        userName = "";
                    else
                        userName = userList[0]["_source"]["FullName"].ToString();
                    //Lay comments
                    string get_comment_query = @"{
                            ""query"" : {
                              ""match"": {
                                  ""PostId"" : """ + id + @"""}
                       }
                       }";
                    using JsonDocument cmt_doc = JsonDocument.Parse(get_comment_query);
                    var resultCmt = ElasticSearch.getDataAsync(cmt_doc, "comment");
                    JObject jObjectCmt = JObject.Parse(resultCmt.Result.ToString());
                    JArray CmtList = JArray.Parse(jObjectCmt["hits"]["hits"].ToString());
                    // lay user cua tung cmt
                    List<CommentVM> CmtVM = new List<CommentVM>();
                    foreach (var i in CmtList)
                    {
                        string userNameCmt = "", userIdCmt = i["_source"]["UserId"].ToString();
                        string get_user_query_cmt = @"{
                            ""query"" : {
                              ""match"": {
                                  ""_id"" : """ + userIdCmt + @"""}
                       }
                       }";
                        using JsonDocument user_doc_cmt = JsonDocument.Parse(get_user_query_cmt);
                        var resultUserCmt = ElasticSearch.getDataAsync(user_doc_cmt, "user");
                        JObject jObjectUserCmt = JObject.Parse(resultUserCmt.Result.ToString());
                        JArray userListCmt = JArray.Parse(jObjectUserCmt["hits"]["hits"].ToString());
                        if (userListCmt.Count == 0)
                            userNameCmt = "";
                        else
                            userNameCmt = userListCmt[0]["_source"]["FullName"].ToString();

                        CmtVM.Add(new CommentVM()
                        {
                            UserName = userNameCmt,
                            Content = i["_source"]["Content"].ToString()
                        });
                    }



                    post = new Post()
                    {
                        Id = postArray[0]["_id"].ToString(),
                        UserId = userId,
                        UserName = userName,
                        Title = postArray[0]["_source"]["Title"].ToString(),
                        Content = System.Text.Json.JsonSerializer.Deserialize<List<PostContent>>(postArray[0]["_source"]["Content"].ToString()),
                        Comment = CmtVM
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
        [HttpGet]
        public IActionResult GetByUser()
        {
            //lay thong tin dang nhap
            var session = HttpContext.Session;          // Lấy ISession
            string key = "infor_access";
            string json = session.GetString(key);
            string userId = "";
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
            List<Post> listPost = new List<Post>();
            string path = "https://localhost:44381/api/post/user/" + userId;
            using (var client = new HttpClient())
            {
                Task<HttpResponseMessage> res = client.GetAsync(path);
                if (res.Result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var hhh = res.Result.Content.ReadAsStringAsync().Result.ToString();
                    JArray postListArray = JArray.Parse(hhh);
                    foreach (var i in postListArray)
                    {
                        string userName = "";
                        string userPath = "https://localhost:44381/api/user/postId/" + i["_id"].ToString();
                        using (var client2 = new HttpClient())
                        {
                            Task<HttpResponseMessage> res2 = client2.GetAsync(userPath);
                            userName = res2.Result.Content.ReadAsStringAsync().Result.ToString();
                        }

                        listPost.Add(new Post()
                        {
                            Id = i["_id"].ToString(),
                            Title = i["_source"]["Title"].ToString(),
                            Content = System.Text.Json.JsonSerializer.Deserialize<List<PostContent>>(i["_source"]["Content"].ToString()),
                            UserId = userId,
                            UserName = userName,
                            Date = i["_source"]["Date"].ToString()
                        });
                    }
                }
            }
            return View(listPost);
        }

        [HttpDelete]
        public IActionResult Delete(string id)
        {
            return View();
        }
    }
}
