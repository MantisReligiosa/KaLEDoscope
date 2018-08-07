namespace Activation
{
    public interface IActivationFile
    {
        bool Exists();
        byte[] Read();
        void Write(byte[] data);
    }
}
