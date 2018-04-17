namespace Extensions
{
    public static class IntegerExtensions
    {
        public static bool Between(this int i, int from, int to)
        {
            return (from < i) && (i < to);
        }

        public static bool Between(this int i, double from, double to)
        {
            return (from < i) && (i < to);
        }
    }
}
