using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackendAPI.Models
{
    public class PostContent
    {
        public Guid Id { get; set; }
        public string SubTitle { get; set; }
        public string SubContent { get; set; }
    }
}
