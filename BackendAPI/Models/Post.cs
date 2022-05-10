using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackendAPI.Models
{
    public class PostVM
    {
        public string UserId { get; set; }
        public string Title { get; set; }
        public List<PostContent> Content { get; set; }
    }
}
