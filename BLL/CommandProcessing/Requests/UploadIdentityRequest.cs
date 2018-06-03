using CommandProcessing.DTO;
using Extensions;
using System;

namespace CommandProcessing.Requests
{
    public class UploadIdentityRequest : Request
    {
        public override byte RequestID => 0x03;

        public override byte[] MakeData(object o)
        {
            var identity = o as Identity;
            var nameLength = (byte)identity.Name.Length;
            var bytes = new byte[nameLength + 3];
            Array.Copy(((ushort)(identity.Id)).ToBytes(), 0, bytes, 0, 2);
            bytes[2] = nameLength;
            Array.Copy(identity.Name.ToBytes(), 0, bytes, 3, nameLength);
            return bytes;
        }
    }
}
