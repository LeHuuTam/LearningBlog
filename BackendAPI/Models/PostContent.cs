using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackendAPI.Models
{
    public class PostContent
    {
        public string Type { get; set; } //text or img
        public string SubTitle { get; set; }
        public string SubContent { get; set; } // text or link to img
    }
}
