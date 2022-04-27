using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackendAPI.Models
{
    public class PostVM
    {      
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Title { get; set; }
        public List<PostContent> Contents { get; set; }
    }
    public class Post : PostVM
    {
        public Guid Id { get; set; }
    }
}
