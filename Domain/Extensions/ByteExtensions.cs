namespace Extensions
{
    public static class ByteExtensions
    {
        public static bool GetBit(this byte b, byte position)
        {
            if (position > 7)
                throw new System.ArgumentException("Значение должно быть в пределах от 0 до 7",
                    nameof(position));
            return (b & (1 << (7 - position))) != 0;
        }
    }
}
