using System.Text;

namespace Extensions
{
    public static class ByteArrayExtensions
    {
        public static string ToStringExtend(this byte[] bytes)
        {
            var sb = new StringBuilder();
            foreach (var b in bytes)
            {
                sb.Append($"[{b:X2}]");
            }
            return sb.ToString();
        }
    }
}
