namespace ServiceViewer.Models
{
    public class Service
    {
        public int InstanceId { get; set; }

        public int ServiceId { get; set; }

        public string IpAddress { get; set; } = "";

        public int Port { get; set; }
    }
}