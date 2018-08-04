namespace ServiceInterfaces
{
    public interface IRequest
    {
        ushort DeviceID { get; set; }
        byte RequestID { get; }

        byte[] GetBytes();
        byte[] MakeData(object o);
        void SetRequestData(object data);
        string ToString();
    }
}
