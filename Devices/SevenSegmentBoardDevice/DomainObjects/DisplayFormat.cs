namespace SevenSegmentBoardDevice
{
    public class DisplayFormat
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Capacity { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is DisplayFormat displayFormat))
                return false;
            return Id == displayFormat.Id;
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}
