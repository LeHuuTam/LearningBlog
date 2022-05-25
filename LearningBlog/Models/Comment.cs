using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearningBlog.Models
{
    public class CommentVM
    {
        public string UserName { get; set; }
        public string Content { get; set; }
    }
    public class Comment
    {
        public string UserId { get; set; }
        public string PostId { get; set; }
        public string Content { get; set; }
    }
}
