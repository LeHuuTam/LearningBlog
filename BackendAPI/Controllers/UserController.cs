using BackendAPI.Models;
using BackendAPI.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace BackendAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
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
                var result = ElasticSearch.getDataAsync(doc, "user");
                JObject jObject = JObject.Parse(result.Result.ToString());
                return Ok(jObject["hits"]["hits"].ToString());
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
                    ""_id"" : """ + id + @"""}
                }
                }";
            try
            {
                using JsonDocument doc = JsonDocument.Parse(query);
                var result = ElasticSearch.getDataAsync(doc, "user");
                JObject jObject = JObject.Parse(result.Result.ToString());
                return Ok(jObject["hits"]["hits"].ToString());
            }
            catch
            {
                return BadRequest();
            }
        }
        [HttpGet("postid/{postId}")]
        public IActionResult GetUserNameByPostId(string postId)
        {
            string userId;
            //lay post
            string postQuery = @"{
                ""query"" : {
                ""match"": {
                    ""_id"" : """ + postId + @"""}
                }
                }";
            try
            {
                using JsonDocument doc1 = JsonDocument.Parse(postQuery);
                var result1 = ElasticSearch.getDataAsync(doc1, "post");
                JObject jObject1 = JObject.Parse(result1.Result.ToString());
                JArray arr = JArray.Parse(jObject1["hits"]["hits"].ToString());
                userId = arr[0]["_source"]["UserId"].ToString();
            }
            catch
            {
                userId = "";
            }

            //lay user
            string userQuery = @"{
                ""query"" : {
                ""match"": {
                    ""_id"" : """ + userId + @"""}
                }
                }";
            //try
            //{
                using JsonDocument doc = JsonDocument.Parse(userQuery);
                var result = ElasticSearch.getDataAsync(doc, "user");
                JObject jObject = JObject.Parse(result.Result.ToString());
            JArray arr2 = JArray.Parse(jObject["hits"]["hits"].ToString());
            string fn = arr2[0]["_source"]["FullName"].ToString();
            return Ok(fn);
            //}
            //catch
            //{
            //    return BadRequest();
            //}
        }
        [HttpPost]
        public IActionResult Create(User userVM)
        {
            string id = Guid.NewGuid().ToString();
            string query = @"{
                        ""id"":""" + id + @""",
                        ""fullName"" : """ + userVM.FullName + @""",
                        ""userName"" : """ + userVM.UserName + @""",
                        ""password"" : """ + userVM.Password + @"""}";
            try
            {
                using JsonDocument doc = JsonDocument.Parse(query);
                var result = ElasticSearch.postDataAsync(doc, "user");
                return Ok(userVM);
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
                var resultGet = ElasticSearch.getDataAsync(doc, "user");
                JObject jObject = JObject.Parse(resultGet.Result.ToString());
                JArray userList = JArray.Parse(jObject["hits"]["hits"].ToString());
                string _id = userList[0]["_id"].ToString();


                var resultDelete = ElasticSearch.deleteDataAsync(_id, "user");
                return Ok(resultDelete.Result.ToString());
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}

