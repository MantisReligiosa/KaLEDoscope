namespace SevenSegmentBoardDevice
{
    public class DisplayType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsFontEnabled { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is DisplayType displayType))
                return false;
            return Id == displayType.Id;
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}
