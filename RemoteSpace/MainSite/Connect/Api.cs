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
            
                HttpResponseMessage response = await Client.GetAsync(_Address + "dir/"+path.Replace("\\","_5"));
                response.EnsureSuccessStatusCode();
                var result = JsonConvert.DeserializeObject<ResponseFiles>(await response.Content.ReadAsStringAsync());
                if(!result.Status)
                {
                    throw new Exception(result.Errors[0]);
                }

                return result.Content;
        }
        public static async Task<bool> AddDir(string name,string path, string Owner)
        {
           
            if (!path.StartsWith("\\"))
            {
                path = "\\" + path;
            }
            var requestdir = new FileElementAddRequest();
            requestdir.FileInfo = new FileElementSend() { Name = name, Path = path, Owner = Owner, Description = "", IsDirectory = true, Shared = false };
            requestdir.Content = null;
            List<FileElement> response;
            try
            {
               response = Api.AddFile(requestdir).Result;
            }
            catch (Exception ex)
            {
                return false;
            }
            if (response.Count == 0)
            {
                
            }
            return true;

        }
        public static async Task<bool> CheckPath(string path)
        {
            if (!path.StartsWith("\\"))
            {
                path = "\\" + path;
            }
            HttpResponseMessage response = await Client.GetAsync(_Address + "PathEx/" + path.Replace("\\", "_5"));
            response.EnsureSuccessStatusCode();
            return ("true"==response.Content.ReadAsStringAsync().Result);
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
        public static async Task<bool> Validate(int? id, string owner)
        {
            if(id==null)
            {
                return false;
            }
            var request = await Client.GetAsync(_Address + id);
            request.EnsureSuccessStatusCode();
            var result= JsonConvert.DeserializeObject<ResponseFiles>(await request.Content.ReadAsStringAsync());
            if(result.Content==null || result.Content.First().Owner!=owner)
            {
                return false;
            }
            return true;
        }
        public static async Task<bool> DeleteFile(int id)
        {
            
            var response = await Client.DeleteAsync(_Address + id);

            response.EnsureSuccessStatusCode();
            var result = JsonConvert.DeserializeObject<ResponseModel>(await response.Content.ReadAsStringAsync());
            if (!result.Status)
            {

                return false;
            }

            return true;
        }
        public static async void  Rename (string newname, int? Id)
        {
            
            if (Id == null)
            {
                throw new NullReferenceException("ID");
            }
            
        }
    }
}
