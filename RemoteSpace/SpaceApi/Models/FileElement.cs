using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceApi
{
    [Index(new string[] { nameof(Path), /*nameof(FatherDirectoryId),*/ nameof(IsDirectory),nameof(Owner) }, IsUnique =true)]
    public class FileElement
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Weight { get; set; }
        public string Owner { get; set; }
        public DateTime UploadDate { get; set; }
        public string Path { get; set; }
        public string Description { get; set; }
        public bool Shared { get; set; }
        public bool IsDirectory { get; set; }
        //public int? FatherDirectoryId { get; set; }
        //public virtual FileElement FatherDirectory { get; set; }
        //public virtual ICollection<FileElement> ChidFiles { get; set; }
    }
}
