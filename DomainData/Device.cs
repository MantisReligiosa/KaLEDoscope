namespace DomainData
{
    public class Device
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public WorkSchedule WorkSchedule { get; set; }
        public Brightness Brightness { get; set; }
        public Network Network { get; set; }
        public string Model { get; set; }
    }
}
