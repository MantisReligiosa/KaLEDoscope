namespace Extensions
{
    public static class IntegerExtensions
    {
        public static bool Between(this int i, int from, int to)
        {
            return (from <= i) && (i <= to);
        }
    }
}
