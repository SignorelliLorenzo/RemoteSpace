using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceApi.Models
{
    public class ResponseModel
    {
        public bool Status { get; set; }
        public List<string> Errors { get; set; }
    }
    public class ResponseFiles : ResponseModel
    {
        public List<FileElement> Content { get; set; }
    }
    public class ResponseFileId : ResponseModel
    {
        public int Id { get; set; }
    }
    public class ResponseFile : ResponseModel
    {
        public byte[] Content { get; set; }
    }
}
