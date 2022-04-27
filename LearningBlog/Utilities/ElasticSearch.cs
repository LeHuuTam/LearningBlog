using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LearningBlog.Utilities
{

    public static class ElasticSearch
    {
        private static HttpClient client;
        private static string IP = "https://34.195.187.172:9200";
        private static string authen = "phunghx:elas@huutam";

        public static string ToJsonString(this JsonDocument jdoc)
        {
            using (var stream = new MemoryStream())
            {
                Utf8JsonWriter writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true });
                jdoc.WriteTo(writer);
                writer.Flush();
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }
        public static HttpClient getClient()
        {
            if (ElasticSearch.client is null)
            {
                var handler = new HttpClientHandler();
                handler.ClientCertificateOptions = ClientCertificateOption.Manual;
                handler.ServerCertificateCustomValidationCallback =
                    (httpRequestMessage, cert, cetChain, policyErrors) =>
                    {
                        return true;
                    };

                ElasticSearch.client = new HttpClient(handler);
                var byteArray = Encoding.ASCII.GetBytes($"{ElasticSearch.authen}");
                ElasticSearch.client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(byteArray));
                ElasticSearch.client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type",
                    "application/json; charset=utf-8");
            }
            return ElasticSearch.client;
        }
        public static async System.Threading.Tasks.Task<string> postDataAsync(JsonDocument doc, string index)
        {
            var content = new StringContent(ElasticSearch.ToJsonString(doc),
                Encoding.ASCII, "application/json");


            var response = await ElasticSearch.getClient().PostAsync("" +
                 $"{ElasticSearch.IP}/{index}/_doc?pretty",
                 content);

            var responseString = await response.Content.ReadAsStringAsync();
            return (responseString);
        }
        public static async System.Threading.Tasks.Task<string> postDataAsyncMulti(string[] docs, string index)
        {
            string postdata = "";
            foreach (var doc in docs)
            {
                using JsonDocument doc2 = JsonDocument.Parse(doc);
                postdata += ElasticSearch.ToJsonString(doc2).Replace("\n", "") + " ";
            }
            Console.WriteLine(postdata);
            var content = new StringContent(postdata,
                Encoding.ASCII, "application/json");


            var response = await ElasticSearch.getClient().PostAsync("" +
                 $"{ElasticSearch.IP}/{index}/_bulk?pretty",
                 content);

            var responseString = await response.Content.ReadAsStringAsync();
            return (responseString);
        }
        public static async System.Threading.Tasks.Task<string> getDataAsync(JsonDocument doc, string index)
        {


            var content = new StringContent(ElasticSearch.ToJsonString(doc),
                Encoding.ASCII, "application/json");


            var response = await ElasticSearch.getClient().PostAsync("" +
                 $"{ElasticSearch.IP}/{index}/_search?pretty",
                 content);

            var responseString = await response.Content.ReadAsStringAsync();
            return (responseString);
        }
    }
}
