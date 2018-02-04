namespace DomainEntities
{
    public class Frame
    {
        public byte[] Data { get; set; }
        public byte UnitId { get; set; }
        public byte? CommandId { get; set; }
        public ushort Lenght { get; set; }
        public ushort Index { get; set; }
    }
}