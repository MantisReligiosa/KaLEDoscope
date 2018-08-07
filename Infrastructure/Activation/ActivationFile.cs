using System.IO;

namespace Activation
{
    public class ActivationFile : IActivationFile
    {
        private readonly string _activationFile = "Activation.dat";

        public bool Exists()
        {
            return File.Exists(_activationFile);
        }

        public byte[] Read()
        {
            return File.ReadAllBytes(_activationFile);
        }

        public void Write(byte[] data)
        {
            File.WriteAllBytes(_activationFile, data);
        }
    }
}
