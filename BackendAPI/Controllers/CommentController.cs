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
    public class CommentController : ControllerBase
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
                var result = ElasticSearch.getDataAsync(doc, "comment");
                JObject jObject = JObject.Parse(result.Result.ToString());
                return Ok(jObject["hits"]["hits"].ToString());
            }
            catch
            {
                return BadRequest();
            }
        }
        [HttpGet("{postId}")]
        public IActionResult GetById(string postId)
        {
            string query = @"{
                ""query"" : {
                ""match"": {
                    ""postId"" : """ + postId + @"""}
                }
                }";
            try
            {
                using JsonDocument doc = JsonDocument.Parse(query);
                var result = ElasticSearch.getDataAsync(doc, "comment");
                JObject jObject = JObject.Parse(result.Result.ToString());
                return Ok(jObject["hits"]["hits"].ToString());
            }
            catch
            {
                return BadRequest();
            }
        }
        [HttpPost]
        public IActionResult Create(Comment cmt)
        {
            try
            {
                var json = JsonSerializer.Serialize(cmt);
                using JsonDocument doc = JsonDocument.Parse(json);
                var result = ElasticSearch.postDataAsync(doc, "comment");
                return Ok("success");
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
