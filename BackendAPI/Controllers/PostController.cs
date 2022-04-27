using BackendAPI.Models;
using BackendAPI.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace BackendAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAll()
        {
            string query = @"{
                ""query"" : {
                ""match_all"": {}
                }
                }";
            try
            {
                using JsonDocument doc = JsonDocument.Parse(query);
                var result = ElasticSearch.getDataAsync(doc, "post");
                JObject jObject = JObject.Parse(result.Result.ToString());


                List<Post> all_post = new List<Post>();
                JArray postList = JArray.Parse(jObject["hits"]["hits"].ToString());
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
                return Ok(all_post);
            }
            catch
            {
                return BadRequest();
            }
        }
        [HttpGet("{id}")]
        public IActionResult GetById(string id)
        {
            string query = @"{
                ""query"" : {
                ""match"": {
                    ""id"" : """ + id + @"""}
                }
                }";
            try
            {
                using JsonDocument doc = JsonDocument.Parse(query);
                var result = ElasticSearch.getDataAsync(doc, "post");
                JObject jObject = JObject.Parse(result.Result.ToString());


                List<Post> all_post = new List<Post>();
                JArray postList = JArray.Parse(jObject["hits"]["hits"].ToString());
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
                return Ok(all_post[0]);
            }
            catch
            {
                return BadRequest();
            }
        }
        [HttpPost]
        public IActionResult Create(string userId, Post postVM)
        {
            //Lay ten nguoi dang bai
            string query = @"{
                ""query"" : {
                ""match"": {
                    ""id"" : """ + userId + @"""}
                }
                }";
            try
            {
                using JsonDocument doc_user = JsonDocument.Parse(query);
                var result_user = ElasticSearch.getDataAsync(doc_user, "user");
                JObject jObject = JObject.Parse(result_user.Result.ToString());
                List<User> all_user = new List<User>();
                JArray userList = JArray.Parse(jObject["hits"]["hits"].ToString());
                string userName = userList[0]["_source"]["userName"].ToString();
                string id = Guid.NewGuid().ToString();

                string postQuery = @"{ ""id"" : """ + id + @""",
                                ""user_id"" : """ + userId + @""",
                                ""title"" : """ + postVM.Title + @""",
                                ""content"" : [";

                int j;
                for (j = 0; j < postVM.Contents.Count - 1; j++)
                {
                    postQuery += @"{
                             ""subtitle"" : """ + postVM.Contents[j].SubTitle + @""",
                             ""subcontent"" : """ + postVM.Contents[j].SubContent + @"""
                     },";
                }
                postQuery += @"{
                             ""subtitle"" : """ + postVM.Contents[j].SubTitle + @""",
                             ""subcontent"" : """ + postVM.Contents[j].SubContent + @"""
                     }]}";


                using JsonDocument doc_post = JsonDocument.Parse(postQuery);
                var result_post = ElasticSearch.postDataAsync(doc_post, "post");
                return Ok(new { 
                    UserPost = userName
                });
            }
            catch
            {
                return BadRequest();
            }
        }
        [HttpPut("{id}")]
        public IActionResult Update(string id, User userUpdate)
        {
            string query = @"{
                ""query"" : {
                ""match"": {
                    ""id"" : """ + id + @"""}
                }
                }";
            try
            {
                //Get user can sua
                using JsonDocument docGet = JsonDocument.Parse(query);
                var resultGet = ElasticSearch.getDataAsync(docGet, "user");
                JObject jObject = JObject.Parse(resultGet.Result.ToString());
                JArray userList = JArray.Parse(jObject["hits"]["hits"].ToString());
                if (userList.Count == 0)
                {
                    return NotFound();
                }
                string _id = userList[0]["_id"].ToString(), oldId = userList[0]["_source"]["id"].ToString();
                if (userUpdate.Id.ToString() != oldId)
                {
                    return BadRequest();
                }
                string updateQuery = @"{
                    ""id"":""" + userUpdate.Id.ToString() + @""",
                    ""fullName"" : """ + userUpdate.FullName + @""",
                    ""userName"" : """ + userUpdate.UserName + @""",
                    ""password"" : """ + userUpdate.Password + @"""
                    }";
                //update
                using JsonDocument docUpdate = JsonDocument.Parse(updateQuery);
                var result = ElasticSearch.updateDataAsync(docUpdate, _id, "user");
                return Ok(userList.Count);
            }
            catch
            {
                return BadRequest();
            }
        }
        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            string query = @"{
                ""query"" : {
                ""match"": {
                    ""id"" : """ + id + @"""}
                }
                }";
            try
            {
                using JsonDocument doc = JsonDocument.Parse(query);
                var resultGet = ElasticSearch.getDataAsync(doc, "post");
                JObject jObject = JObject.Parse(resultGet.Result.ToString());
                JArray userList = JArray.Parse(jObject["hits"]["hits"].ToString());
                string _id = userList[0]["_id"].ToString();


                var resultDelete = ElasticSearch.deleteDataAsync(_id, "post");
                return Ok(resultDelete.Result.ToString());
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
