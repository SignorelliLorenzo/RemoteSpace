using MainSite.Connect.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace MainSite.Connect
{
    public static class Api
    {
        private static HttpClient Client = new HttpClient();
        public static void Initialize(string token)
        {
            Client.DefaultRequestHeaders.Authorization= new AuthenticationHeaderValue("Bearer", token);
        }
        public static async Task< List<FileElement>> GetDirFiles(string path)
        {
            if(!path.StartsWith("\\"))
            {
                path = "\\" + path;
            }
            
                HttpResponseMessage response = await Client.GetAsync("");
                response.EnsureSuccessStatusCode();
                var result = JsonConvert.DeserializeObject<ResponseFiles>(await response.Content.ReadAsStringAsync());
                if(!result.Status)
                {
                    throw new Exception(result.Errors[0]);
                }

                return result.Content;
            
            
            
        }
        public static async Task<List<FileElement>> AddFile(FileElementAddRequest Request)
        {

            HttpRequestMessage request = new HttpRequestMessage();
            request.Method = HttpMethod.Post;
            request.RequestUri = JsonConvert.SerializeObject(Request);
            HttpResponseMessage response = await Client.SendAsync("");
            response.EnsureSuccessStatusCode();
            var result = JsonConvert.DeserializeObject<ResponseFiles>(await response.Content.ReadAsStringAsync());
            if (!result.Status)
            {
                throw new Exception(result.Errors[0]);
            }

            return result.Content;



        }

    }
}
