using BaseDevice;

namespace KaLEDoscope
{
    public class DeviceNode : NodeItem
    {
        public Device Device { get; set; }
        public override string Name { get; set; }
        public bool AllowUpload { get; set; }
        public bool AllowDownload { get; set; }
        public bool AllowSave { get; set; }
        public bool AllowLoad { get; set; }
    }
}
