namespace ServiceInterfaces
{
    public interface IResponce<out T> where T : class
    {
        byte ResponceID { get; }
        Resultativity Resultativity { get; }
        T Cast();
        void SetByteSequence(byte[] recievedBytes);
        string ToString();
    }

    public enum Resultativity
    {
        IncorrectRequestLength = -3,
        IncorrectRequestData = -2,
        IncorrectRequestObject = -1,
        Busy = 0,
        DataReturned = 1,
        Accepted = 2
    }
}
