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
            AggregationOrder = device?.AggregationOrder;
            Brightness = (SerializableBrightness)device?.Brightness;
            FolderId = device?.FolderId;
            Id = device?.Id ?? 0;
            Model = device?.Model;
            Name = device?.Name;
            IsStandaloneConfiguration = device?.IsStandaloneConfiguration ?? true;
            Network = (SerializableNetwork)device?.Network;
            WorkSchedule = (SerializableWorkSchedule)device?.WorkSchedule;
        }

        public ushort? AggregationId { get; set; }
        public SerializableBrightness Brightness { get; set; }
        public ushort? FolderId { get; set; }
        public ushort Id { get; set; }
        public string Model { get; set; }
        public string Name { get; set; }
        public SerializableNetwork Network { get; set; }
        public SerializableWorkSchedule WorkSchedule { get; set; }
        public bool IsStandaloneConfiguration { get; set; }
        public ushort? AggregationOrder { get; set; }

        public void FillBasicParameters(Device device)
        {
            device.AggregationId = AggregationId;
            device.AggregationOrder = AggregationOrder;
            device.Brightness = (Brightness)Brightness;
            device.FolderId = FolderId;
            device.Id = Id;
            device.Model = Model;
            device.Name = Name;
            device.Network = (Network)Network;
            device.WorkSchedule = (WorkSchedule)WorkSchedule;
            device.IsStandaloneConfiguration = IsStandaloneConfiguration;
        }
    }
}
