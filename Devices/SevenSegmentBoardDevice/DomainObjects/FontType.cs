namespace SevenSegmentBoardDevice
{
    public class FontType
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is FontType fontType))
                return false;
            return fontType.Id == Id;
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}
