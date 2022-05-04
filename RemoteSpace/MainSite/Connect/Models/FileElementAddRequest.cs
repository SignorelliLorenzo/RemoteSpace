using System.ComponentModel.DataAnnotations;

namespace MainSite.Connect.Models
{
    public class FileElementAddRequest 
    {
        public FileElementSend FileInfo { get; set; }
        public byte[] Content { get; set; }
    }
    public class FileElementSend
    {
        [Required]
        public string Name { get; set; }
        public string Owner { get; set; }
        public string Path { get; set; }
        public string Description { get; set; }
        public bool Shared { get; set; }
        public bool IsDirectory { get; set; }
    }

}
