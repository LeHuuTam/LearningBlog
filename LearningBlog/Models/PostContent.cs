using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LearningBlog.Models
{
    public class PostContent
    {
        public string Type { get; set; } //text or img
        public string SubTitle { get; set; }
        public string SubContent { get; set; } // text or link to img
    }
}
