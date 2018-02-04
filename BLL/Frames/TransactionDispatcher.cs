using Interfaces.Infrastructure;
using DomainEntities;
using Interfaces.Bll;
using System.Threading.Tasks;
using System.Timers;
using System.Collections.Generic;

namespace Bll.Commands
{
    public class TransactionDispatcher
    {
        private readonly IConnection _connection;
        private readonly IFrameBuilder _frameBulider;
        private ushort _frameIndex;
        private bool isAnswerRecieved = false;
        private Frame _answer;

        public TransactionDispatcher(IConnection connection, IFrameBuilder frameBulider)
        {
            _connection = connection;
            _frameBulider = frameBulider;
            _connection.RecponceRecieved += OnDataRecieved;
        }

        private void OnDataRecieved(object sender, byte[] bytes)
        {
            _answer = _frameBulider.ParseResponce(bytes);
            isAnswerRecieved = _answer.Index == _frameIndex;
        }

        public virtual void Send(Frame command)
        {
            var bytes = _frameBulider.BuildRequest(command);
            _frameIndex = command.Index;
            _connection.Send(bytes);
        }

        public async Task<Frame> RecieveAnswerAsync()
        {
            return await Task.Run(() =>
            {
                while (!isAnswerRecieved) { }
                return _answer;
            });
        }

        public async Task<List<Frame>> RecieveAnswersAsync(double awaitingInterval)
        {
            return await Task.Run(() =>
            {
                var result = new List<Frame>();
                var timer = new Timer(awaitingInterval);
                bool timeIsUp = false;
                timer.Elapsed += (o, e) => { timeIsUp = true; };
                while (!timeIsUp)
                {
                    if (isAnswerRecieved)
                    {
                        result.Add(_answer);
                        isAnswerRecieved = false;
                    }
                };
                return result;
            });
        }
    }
}
