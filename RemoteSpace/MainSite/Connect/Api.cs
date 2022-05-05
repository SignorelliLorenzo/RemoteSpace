using MainSite.Connect.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace MainSite.Connect
{

    public static class Api
    {
        private static HttpClient Client = new HttpClient();
        private static string _Address;
        public static void Initialize(string token,string Address)
        {
            Client.DefaultRequestHeaders.Authorization= new AuthenticationHeaderValue("Bearer", token);
            _Address = Address;
        }
        public static async Task<byte[]> GetFile(int id)
        {
            

            HttpResponseMessage response = await Client.GetAsync(_Address+ "file/"+id);
            response.EnsureSuccessStatusCode();
            var result = JsonConvert.DeserializeObject<ResponseFile>(await response.Content.ReadAsStringAsync());
            if (!result.Status)
            {
                throw new Exception(result.Errors[0]);
            }

            return result.Content;
        }
        public static async Task< List<FileElement>> GetDirFiles(string path)
        {
            if(!path.StartsWith("\\"))
            {
                path = "\\" + path;
            }
            
                HttpResponseMessage response = await Client.GetAsync(_Address + "dir/"+path);
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
            var response =await Client.PostAsJsonAsync(_Address + "", Request);
            response.EnsureSuccessStatusCode();
            var result = JsonConvert.DeserializeObject<ResponseFiles>(await response.Content.ReadAsStringAsync());
            if (!result.Status)
            {
                throw new Exception(result.Errors[0]);
            }
            return result.Content;
        }
        public static async void DeleteFile(int id)
        {
            var response = await Client.DeleteAsync(_Address + id);

            response.EnsureSuccessStatusCode();
            var result = JsonConvert.DeserializeObject<ResponseModel>(await response.Content.ReadAsStringAsync());
            if (!result.Status)
            {
                throw new Exception(result.Errors[0]);
            }

            return;
        }
    }
}
