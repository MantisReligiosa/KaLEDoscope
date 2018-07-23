namespace BaseDevice
{
    public class Device
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public WorkSchedule WorkSchedule { get; set; } = new WorkSchedule();
        public Brightness Brightness { get; set; } = new Brightness();
        public Network Network { get; set; } = new Network();
        public string Model { get; set; }
        public string Firmware { get; set; }
        public bool IsStandaloneConfiguration { get; set; }
        public int? FolderId { get; set; }
        public int? AggregationId { get; set; }
        public int? AggregationOrder { get; set; }
    }
}
