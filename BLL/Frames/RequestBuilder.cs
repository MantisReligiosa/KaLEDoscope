using Interfaces.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bll.Frames
{
    public class RequestBuilder
    {
        private readonly IIndexer _indexer;
        private readonly IHashProvider _hashProvider;
        public RequestBuilder(IIndexer indexer, IHashProvider hashProvider)
        {
            _indexer = indexer;
            _hashProvider = hashProvider;
        }

        public byte[] Build(byte unitId, byte commandId, byte[] parameters)
        {
            var lenght = Convert.ToUInt16(parameters.Length + _hashProvider.HashLenght);
            var index = _indexer.GetIdentifier();
            var command = new List<byte>();
            command.AddRange(BitConverter.GetBytes(index).Reverse());
            command.AddRange(BitConverter.GetBytes(lenght).Reverse());
            command.Add(unitId);
            command.Add(commandId);
            command.AddRange(parameters);
            var hash = _hashProvider.GetHash(command.ToArray()).Reverse();
            command.AddRange(hash);
            return command.ToArray();
        }
    }
}
