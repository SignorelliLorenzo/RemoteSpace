using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceApi
{
    [Index(nameof(Name), nameof(FatherDirectory), nameof(IsDirectory), IsUnique =true)]
    public class FileElement
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Weight { get; set; }
        public string Onower { get; set; }
        public DateTime UploadDate { get; set; }
        public string Path { get; set; }
        public string Description { get; set; }
        public bool Shared { get; set; }
        public bool IsDirectory { get; set; }
        public FileElement FatherDirectory { get; set; }


    }
}
