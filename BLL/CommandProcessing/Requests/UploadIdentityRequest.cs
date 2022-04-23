using CommandProcessing.DTO;
using SmartTechnologiesM.Base.Extensions;
using System.Collections.Generic;

namespace CommandProcessing.Requests
{
    public class UploadIdentityRequest : Request
    {
        public override byte RequestID => 0x03;

        public override byte[] MakeData(object o)
        {
            var identity = o as Identity;
            var nameLength = (byte)identity.Name.Length;
            var bytes = new List<byte>
            {
                nameLength
            };
            bytes.AddRange(identity.Name.ToBytes());
            return bytes.ToArray();
        }
    }
}
