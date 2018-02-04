using Bll.Commands;
using DomainEntities;
using Infrastructure.HashProvider;
using Infrastructure.Indexer;
using Infrastructure.UdpConnection;
using Interfaces.Bll;
using Interfaces.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransactionDispatcherTesting
{
    class Program
    {
        static void Main(string[] args)
        {
            IConnection cnn = new UdpConnection(57343);
            IFrameIndexer frameIndexer = new Indexer();
            frameIndexer.Set(1);
            IHashProvider hashProvider = new HashProvider();
            IFrameBuilder frameBuilder = new FrameBuilder(frameIndexer, hashProvider);
            var transactionDispatcher = new TransactionDispatcher(cnn, frameBuilder);
            transactionDispatcher.Send(new Frame
            {
                CommandId = 10,
                Data = new byte[] { 1, 2, 3, 4, 5 },
                UnitId = 20,
            });
            var answers = transactionDispatcher.RecieveAnswers(60000);
        }
    }
}
