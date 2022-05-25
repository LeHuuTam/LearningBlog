using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackendAPI.Models
{
    public class Comment
    {
        public string UserId { get; set; }
        public string PostId { get; set; }
        public string Content { get; set; }
    }
}
