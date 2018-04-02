namespace PixelBoardDevice.Extensions
{
    public static class IntegerExtension
    {
        public static bool Between(this int i, int from, int to)
        {
            return (from <= i) && (i <= to);
        }
    }
}
