using DomainEntities;
using Interfaces.Bll;
using Interfaces.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bll.Commands
{
    public class FrameBuilder : IFrameBuilder
    {
        private readonly IFrameIndexer _indexer;
        private readonly IHashProvider _hashProvider;
        public FrameBuilder(IFrameIndexer indexer, IHashProvider hashProvider)
        {
            _indexer = indexer;
            _hashProvider = hashProvider;
        }

        public byte[] BuildRequest(Frame frame)
        {
            var lenght = Convert.ToUInt16(frame.Data.Length + 2 + _hashProvider.HashLenght);
            var index = _indexer.GetIdentifier();
            frame.Index = index;
            frame.Lenght = lenght;
            var command = new List<byte>();
            command.AddRange(BitConverter.GetBytes(index).Reverse());
            command.AddRange(BitConverter.GetBytes(lenght).Reverse());
            command.Add(frame.UnitId);
            command.Add(frame.CommandId.Value);
            command.AddRange(frame.Data);
            var hash = _hashProvider.GetHash(command.ToArray()).Reverse();
            command.AddRange(hash);
            return command.ToArray();
        }

        public Frame ParseResponce(byte[] bytes)
        {
            var indexBytes = new byte[2];
            Array.Copy(bytes, 0, indexBytes, 0, 2);
            indexBytes = indexBytes.Reverse().ToArray();
            var index = BitConverter.ToUInt16(indexBytes, 0);
            var lengthBytes = new byte[2];
            Array.Copy(bytes, 2, lengthBytes, 0, 2);
            lengthBytes = lengthBytes.Reverse().ToArray();
            var lenght = BitConverter.ToUInt16(lengthBytes, 0);
            var unitId = bytes[4];
            var data = new byte[lenght- _hashProvider.HashLenght-1];
            Array.Copy(bytes, 5, data, 0, lenght - _hashProvider.HashLenght-1);
            var actualCRCbytes = new byte[_hashProvider.HashLenght];
            Array.Copy(bytes, 5 + lenght - 1 - _hashProvider.HashLenght, actualCRCbytes, 0, _hashProvider.HashLenght);
            var message = new byte[bytes.Length - _hashProvider.HashLenght];
            Array.Copy(bytes, message, bytes.Length - _hashProvider.HashLenght);
            var expectedCRCbytes = _hashProvider.GetHash(message).Reverse().ToArray();
            var expectedCRC = BitConverter.ToInt16(actualCRCbytes, 0);
            var actualCRC = BitConverter.ToInt16(actualCRCbytes, 0);
            if (expectedCRC != actualCRC)
            {
                throw new Exception("Incorrect  responce CRC");
            }
            return new Frame
            {
                Index = index,
                Lenght = lenght,
                UnitId = unitId,
                Data = data
            };
        }
    }
}
