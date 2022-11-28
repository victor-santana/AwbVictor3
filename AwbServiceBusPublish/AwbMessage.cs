namespace AwbServiceBusPublish
{
    public class AwbMessage
    {
        public int Id { get; set; }
        public string? PackingShippingNumber { get; set; }
        public DateTime CratedDate { get; set; }
        public DateTime PackedDate { get; set; }
        public DateTime ReceivedDate { get; set; }
        public DateTime ShippedDate { get; set; }
        public DateTime Timestamp { get; set; }

    }
}