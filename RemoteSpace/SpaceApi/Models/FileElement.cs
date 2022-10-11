using Microsoft.EntityFrameworkCore;
using SpaceApi.Models.Communication.Request;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceApi
{
    [Index(new string[] { nameof(Path), nameof(Name),nameof(User) /*nameof(FatherDirectoryId), nameof(IsDirectory),*/}, IsUnique =true)]
    public class FileElement
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public long Weight { get; set; }
        [Required]
        public string Owner { get; set; }
        [Required]
        public string User { get; set; }
        [Required]
        public DateTime UploadDate { get; set; }
        [Required,RegularExpression("^\\")]
        public string Path { get; set; }
        public string Description { get; set; }
        public bool Shared { get; set; }
        public bool IsDirectory { get; set; }
        //public int? FatherDirectoryId { get; set; }
        //public virtual FileElement FatherDirectory { get; set; }
        //public virtual ICollection<FileElement> ChidFiles { get; set; }
        public FileElement()
        {

        }
        public FileElement(FileElementSend filesent,int weight,string user)
        {
           
            Name=filesent.Name;
            Weight=weight;
            Owner=filesent.Owner;
            UploadDate=DateTime.Now;
            User=user;
            if(filesent.Path== null || !filesent.Path.StartsWith("\\"))
            {
                Path = "\\"+filesent.Path;
            }
            else
            {
                Path = filesent.Path;
            }
           
            Description=filesent.Description;
            Shared=filesent.Shared;
            IsDirectory=filesent.IsDirectory;

        }
    }
}
