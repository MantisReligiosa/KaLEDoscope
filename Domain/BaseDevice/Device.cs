namespace BaseDevice
{
    public class Device
    {
        public string Name { get; set; }
        public ushort Id { get; set; }
        public WorkSchedule WorkSchedule { get; set; } = new WorkSchedule();
        public Brightness Brightness { get; set; } = new Brightness();
        public Network Network { get; set; } = new Network
        {
            Port = 3000,
            SubnetMask = 24,
            AlternativeDnsServer = "0.0.0.0"
        };
        public string Model { get; set; }
        public string Firmware { get; set; }
        public bool IsStandaloneConfiguration { get; set; }
        public ushort? FolderId { get; set; }
        public ushort? AggregationId { get; set; }
        public ushort? AggregationOrder { get; set; }
    }
}
