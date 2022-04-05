using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceApi
{
    public class FileElementSample
    {
        
        public string Name { get; }
        public decimal Weight { get; }
        public string Onower { get; }
        public DateTime UploadDate { get; }
        public string path { get; }
        public string Description { get; }
        public bool Shared { get; }
        public bool IsDirectory { get; }
        public FileElementSample(string name, decimal weight, string owner,bool isdirectory=false, string path=null,string description=null, bool shared=false)
        {

        }
        
    }
}
