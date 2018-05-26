namespace ServiceInterfaces
{
    public interface IRequestBuilder
    {
        void SetRequest(Request request);
        string GetString();
        byte[] GetBytes();
    }
}
