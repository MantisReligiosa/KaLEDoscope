using CommandProcessing.Exceptions;
using ServiceInterfaces;
using SmartTechnologiesM.Base.Extensions;

namespace CommandProcessing
{
    public abstract class Responce<T> : IResponce<T>
        where T : class
    {
        protected byte[] _bytes;
        public Resultativity Resultativity { get; private set; }

        public abstract byte ResponceID { get; }

        public void SetByteSequence(byte[] recievedBytes)
        {
            _bytes = recievedBytes;
            if (recievedBytes.Length < 5)
                throw new InvalidByteSequenceException("Неверная длина ответа");
            var dataLength = _bytes.ExtractUshort(3);
            if (dataLength != _bytes.Length - 5)
                throw new InvalidByteSequenceException("Неверная длина данных");
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
            return Resultativity.DataReturned;
        }

        public abstract T Cast();

        public override string ToString()
        {
            return _bytes.ToStringExtend();
        }
    }
}
