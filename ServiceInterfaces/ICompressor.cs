using System.IO;

namespace ServiceInterfaces
{
    public interface ICompressor
    {
        void CopyTo(Stream src, Stream dest);
        string Unzip(byte[] bytes);
        byte[] Zip(string str);
    }
}
