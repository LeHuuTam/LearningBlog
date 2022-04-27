using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearningBlog.Models2
{
    public class Post
    {
        public int Id { get; set; }
        public int User_Id { get; set; }
        public string User_Name { get; set; }
        public string Title { get; set; }
        public List<string> Content { get; set; }
        public List<Comment> Comments { get; set; }
    }
}
