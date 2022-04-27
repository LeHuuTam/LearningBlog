﻿using BackendAPI.Models;
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


                List<User> all_user = new List<User>();
                JArray userList = JArray.Parse(jObject["hits"]["hits"].ToString());
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
                return Ok(all_user);
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
                var result = ElasticSearch.getDataAsync(doc, "user");
                JObject jObject = JObject.Parse(result.Result.ToString());
                JArray userList = JArray.Parse(jObject["hits"]["hits"].ToString());
                List<User> all_user = new List<User>();
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
                return Ok(all_user[0]);
            }
            catch
            {
                return BadRequest();
            }
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
