using DomainEntities;

namespace Interfaces.Bll
{
    public interface IFrameBuilder
    {
        byte[] BuildRequest(Frame frame);
        Frame ParseResponce(byte[] bytes);
    }
}