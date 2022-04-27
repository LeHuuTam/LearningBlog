using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearningBlog.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int User_Id { get; set; }
        public int Post_Id { get; set; }
        public string Content { get; set; }
    }
}
