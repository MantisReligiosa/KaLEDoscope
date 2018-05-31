using System.Text;

namespace ServiceInterfaces
{
    public abstract class Responce<T>
        where T : class, new()
    {
        protected byte[] _bytes;
        public Resultativity Resultativity { get; private set; }

        public void SetByteSequence(byte[] recievedBytes)
        {
            _bytes = recievedBytes;
            Resultativity = GetResultativity();
        }

        private Resultativity GetResultativity()
        {
            if (_bytes.Length < 5)
                return Resultativity.IncorrectRequestLength;
            if (_bytes[2] == 0xf0)
                return Resultativity.Accepted;
            if (_bytes[2] == 0xf1)
                return Resultativity.Busy;
            if (_bytes[2] == 0xf2)
                return Resultativity.IncorrectRequestData;
            if (_bytes[2] == 0xf2)
                return Resultativity.IncorrectRequestObject;
            return Resultativity.DataRequest;
        }

        public abstract T Cast();

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var b in _bytes)
            {
                sb.Append($"[{b:X2}]");
            }
            return sb.ToString();
        }
    }

    public enum Resultativity
    {
        IncorrectRequestLength = -3,
        IncorrectRequestData = -2,
        IncorrectRequestObject = -1,
        Busy = 0,
        DataRequest = 1,
        Accepted = 2
    }
}
