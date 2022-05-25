using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LearningBlog.Models
{
    public class PostVM
    {
        public string Title { get; set; }
        public List<PostContent> Content { get; set; }
        public List<CommentVM> Comment { get; set; }
    }
    public class Post : PostVM
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Date { get; set; }
    }
}
