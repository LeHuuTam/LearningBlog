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
                },
                ""sort"": [{
                    ""@timestamp"": {
                        ""order"": ""desc""
                        }
                    }],   
                ""size"": 100
                }";
            try
            {
                using JsonDocument doc = JsonDocument.Parse(query);
                var result = ElasticSearch.getDataAsync(doc, "post");
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
                var result = ElasticSearch.getDataAsync(doc, "post");
                JObject jObject = JObject.Parse(result.Result.ToString());
                return Ok(jObject["hits"]["hits"].ToString());
            }
            catch
            {
                return BadRequest();
            }
        }
        [HttpGet("user/{userId}")]
        public IActionResult GetByUserId(string userId)
        {
            string query = @"{
                ""query"" : {
                ""match"": {
                    ""UserId"" : """ + userId + @"""}
                }
                }";
            try
            {
                using JsonDocument doc = JsonDocument.Parse(query);
                var result = ElasticSearch.getDataAsync(doc, "post");
                JObject jObject = JObject.Parse(result.Result.ToString());
                return Ok(jObject["hits"]["hits"].ToString());
            }
            catch
            {
                return BadRequest();
            }
        }
        [HttpGet("search")]
        public IActionResult Search(string searchText)
        {
            string query = @"{
                ""query"" : {
                ""multi_match"": {
                    ""query"":"""+ searchText + @""", 
                     ""fields"": [""Title^3"", ""Content.SubTitile^2"", ""Content.SubContent^1""]
                }
                }
                }";
            try
            {
                using JsonDocument doc = JsonDocument.Parse(query);
                var result = ElasticSearch.getDataAsync(doc, "post");
                JObject jObject = JObject.Parse(result.Result.ToString());
                return Ok(jObject["hits"]["hits"].ToString());
            }
            catch
            {
                return BadRequest();
            }
        }
        [HttpPost]
        public IActionResult Create(PostVM postVM)
        {
            try
            {
                var json = JsonSerializer.Serialize(postVM);
                using JsonDocument doc = JsonDocument.Parse(json);
                var result = ElasticSearch.postDataAsync(doc, "post");
                return Ok("success");
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
                    ""_id"" : """ + id + @"""}
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
                return Ok("success");
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
