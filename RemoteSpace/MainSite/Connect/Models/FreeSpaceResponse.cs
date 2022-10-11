namespace MainSite.Connect.Models
{
    public class FreeSpaceResponse: ResponseModel
    {
        public long FreeSpace { get; set; }
        public long UserSpace { get; set; }
        public long TotalSpace { get; set; }
    }
}
