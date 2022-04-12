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
    public class ResponseDirContent : ResponseModel
    {
        public List<FileElement> Content { get; set; }
    }
    public class FileResponse : ResponseModel
    {
        public FileElement File { get; set; }
    }
   
}
