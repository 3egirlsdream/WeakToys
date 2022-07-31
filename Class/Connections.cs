using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace Clipboards.Class
{
    public class Connections
    {
        public static API<T> HttpGet<T>(string url, string contentType = null, Dictionary<string, string> headers = null)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    if (contentType != null)
                        client.DefaultRequestHeaders.Add("ContentType", contentType);
                    if (headers != null)
                    {
                        foreach (var header in headers)
                            client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                    HttpResponseMessage response = client.GetAsync(url).Result;
                    var rst = response.Content.ReadAsStringAsync().Result;
                    return System.Text.Json.JsonSerializer.Deserialize<API<T>>(rst);
                }
                catch(Exception ex)
                {
                    return new API<T>
                    {
                        code = 200,
                        success = false,
                        data = default,
                        message = new APISub { content = ex.Message }
                    };
                }
            }
        }

        public static async Task<API<T>> HttpGetAsync<T>(string url, Encoding encoding = null)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                var data = await httpClient.GetByteArrayAsync(url);
                var ret = encoding.GetString(data);
                var result = System.Text.Json.JsonSerializer.Deserialize<API<T>>(ret);
                return result;
            }
            catch (Exception ex)
            {
                return new API<T>
                {
                    code = 200,
                    success = false,
                    data = default,
                    message = new APISub { content = ex.Message }
                };
            }
        }

        public static API<T> Post<T>(string url, object postData = null)
        {
            try
            {
                HttpClient httpClient = new HttpClient();//http对象
                HttpResponseMessage response = httpClient.PostAsync(url, new JsonContent(postData)).Result;
                string ret = response.Content.ReadAsStringAsync().Result;
                var result = System.Text.Json.JsonSerializer.Deserialize<API<T>>(ret);
                return result;
            }
            catch(Exception ex)
            {
                return new API<T>
                {
                    code = 200,
                    success = false,
                    data = default,
                    message = new APISub { content = ex.Message }
                };
            }
        }

        public static JsonSerializerOptions options = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All, UnicodeRanges.All),
            WriteIndented = true
        };
    }

    public class JsonContent : StringContent
    {
        
        public JsonContent(object obj) :
        base(System.Text.Json.JsonSerializer.Serialize(obj,  Connections.options), Encoding.UTF8, "application/json")
        { }
    }


    public class API<T>
    {
        public int code { get; set; }
        public bool success { get; set; }
        public T data { get; set; }
        public APISub message { get; set; }
    }

    public class APISub
    {
        public string content { get; set; }
    }
}
