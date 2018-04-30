namespace KaLEDoscope.Serialization
{
    public class SerializableContainer
    {
        public ContentType ContentType { get; set; }
        public object Content { get; set; }
    }

    public enum ContentType
    {
        Device = 0,
        Folder,
        Aggregator
    }
}
