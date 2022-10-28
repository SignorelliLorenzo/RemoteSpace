namespace SpaceApi.Models
{
    public class FreeSpaceResponse: ResponseModel
    {
        public long FreeSpace { get; set; }
        public long? UserSpace { get; set; }
    }
}
