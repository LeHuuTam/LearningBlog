using Newtonsoft.Json;
using System.Net;


namespace LearningBlog.Utilities
{
    public static class GetObject
    {
        public static object Get(string path)
        {
            using (WebClient webCli = new WebClient())
            {
                return JsonConvert.DeserializeObject<object>(webCli.DownloadString(path));
            }
        }
    }
}
