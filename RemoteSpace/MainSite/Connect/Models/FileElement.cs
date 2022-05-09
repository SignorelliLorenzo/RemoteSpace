using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MainSite.Connect.Models
{
    [Index(new string[] { nameof(Path), nameof(Name), nameof(User) /*nameof(FatherDirectoryId), nameof(IsDirectory),*/}, IsUnique = true)]
    public class FileElement
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public decimal Weight { get; set; }
        [Required]
        public string Owner { get; set; }
        [Required]
        public string User { get; set; }
        [Required]
        public DateTime UploadDate { get; set; }
        [Required, RegularExpression("^\\")]
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
        public FileElement(FileElementSend filesent, int weight, string user)
        {

            Name = filesent.Name;
            Weight = weight;
            Owner = filesent.Owner;
            UploadDate = DateTime.Now;
            User = user;
            if (filesent.Path == null || !filesent.Path.StartsWith("\\"))
            {
                Path = "\\" + filesent.Path;
            }
            else
            {
                Path = filesent.Path;
            }

            Description = filesent.Description;
            Shared = filesent.Shared;
            IsDirectory = filesent.IsDirectory;

        }
    }

    public class FileDisplay
    {
        static readonly string[] SizeSuffixes =
                       { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
        static private string SizeSuffix(Int64 value, int decimalPlaces = 1)
        {
            if (decimalPlaces < 0) { throw new ArgumentOutOfRangeException("decimalPlaces"); }
            if (value < 0) { return "-" + SizeSuffix(-value, decimalPlaces); }
            if (value == 0) { return string.Format("{0:n" + decimalPlaces + "} bytes", 0); }

            // mag is 0 for bytes, 1 for KB, 2, for MB, etc.
            int mag = (int)Math.Log(value, 1024);

            // 1L << (mag * 10) == 2 ^ (10 * mag) 
            // [i.e. the number of bytes in the unit corresponding to mag]
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            // make adjustment when the value is large enough that
            // it would round up to 1000 or more
            if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
            {
                mag += 1;
                adjustedSize /= 1024;
            }

            return string.Format("{0:n" + decimalPlaces + "} {1}",
                adjustedSize,
                SizeSuffixes[mag]);
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Weight { get; set; }
        public DateTime UploadDate { get; set; }
        public string Path { get; set; }
        public string Description { get; set; }
        public bool IsDirectory { get; set; }
        public FileDisplay(FileElement file)
        {
            Id = file.Id;
            Name=file.Name;
            Weight = SizeSuffix((int)file.Weight);
            UploadDate = file.UploadDate;
            Path = file.Path;
            Description = file.Description;
            IsDirectory = file.IsDirectory;


        }
    }
}
