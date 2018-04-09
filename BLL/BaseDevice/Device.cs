using Aggregations;

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
        public bool IsStandaloneConfiguration { get; set; }
        public Folder Folder { get; set; }
        public Aggregation Aggregation { get; set; }
    }
}
