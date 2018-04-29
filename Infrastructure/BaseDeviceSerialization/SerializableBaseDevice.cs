using BaseDevice;

namespace BaseDeviceSerialization
{
    public class SerializableBaseDevice
    {
        public SerializableBaseDevice() { }

        public SerializableBaseDevice(Device device)
            : this()
        {
            AggregationId = device?.AggregationId;
            Brightness = (SerializableBrightness)device.Brightness;
            FolderId = device?.FolderId;
            Id = device.Id;
            Model = device.Model;
            Name = device.Name;
            Network = (SerializableNetwork)device.Network;
            WorkSchedule = (SerializableWorkSchedule)device.WorkSchedule;
        }

        public int? AggregationId { get; set; }
        public SerializableBrightness Brightness { get; set; }
        public int? FolderId { get; set; }
        public int Id { get; set; }
        public string Model { get; set; }
        public string Name { get; set; }
        public SerializableNetwork Network { get; set; }
        public SerializableWorkSchedule WorkSchedule { get; set; }

        public void FillBasicParameters(Device device)
        {
            device.AggregationId = AggregationId;
            device.Brightness = (Brightness)Brightness;
            device.FolderId = FolderId;
            device.Id = Id;
            device.Model = Model;
            device.Name = Name;
            device.Network = (Network)Network;
            device.WorkSchedule = (WorkSchedule)WorkSchedule;
        }
    }
    
}
