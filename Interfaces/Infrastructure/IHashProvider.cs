namespace Interfaces.Infrastructure
{
    public interface IHashProvider
    {
        byte[] GetHash(byte[] data);
        ushort HashLenght { get; }
    }
}
