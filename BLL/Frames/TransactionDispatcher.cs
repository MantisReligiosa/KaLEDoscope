using Interfaces.Infrastructure;
using DomainEntities;
using Interfaces.Bll;
using System.Threading.Tasks;
using System.Timers;
using System.Collections.Generic;
using System;

namespace Bll.Commands
{
    public class TransactionDispatcher
    {
        private readonly IConnection _connection;
        private readonly IFrameBuilder _frameBulider;
        private ushort _frameIndex;
        private bool isAnswerRecieved = false;
        private Frame _answer;
        private string _senderAddress;

        public int RecievingTimeout { get; set; } = 500;

        public TransactionDispatcher(IConnection connection, IFrameBuilder frameBulider)
        {
            _connection = connection;
            _frameBulider = frameBulider;
            _connection.ResponceRecieved += OnDataRecieved;
        }

        private void OnDataRecieved(object sender, ResponceInformation responceInformation)
        {
            _answer = _frameBulider.ParseResponce(responceInformation.Data);
            _senderAddress = responceInformation.SenderAddress;
            isAnswerRecieved = (_answer.Index == _frameIndex);
        }

        public virtual void Send(Frame command)
        {
            var bytes = _frameBulider.BuildRequest(command);
            _frameIndex = command.Index;
            _connection.Send(bytes);
        }

        public Frame RecieveAnswer()
        {
            var timer = new Timer(RecievingTimeout);
            bool timeIsUp = false;
            timer.Elapsed += (o, e) => { timeIsUp = true; };
            timer.Start();
            while (!isAnswerRecieved && !timeIsUp) { }
            _connection.Close();
            if (timeIsUp)
                throw new TimeoutException();
            return _answer;
        }

        public List<Tuple<string, Frame>> RecieveAnswers(double awaitingInterval)
        {
            var result = new List<Tuple<string, Frame>>();
            var timer = new Timer(awaitingInterval);
            bool timeIsUp = false;
            timer.Elapsed += (o, e) => { timeIsUp = true; };
            timer.Start();
            while (!timeIsUp)
            {
                if (isAnswerRecieved)
                {
                    result.Add(new Tuple<string, Frame>(_senderAddress, _answer));
                    isAnswerRecieved = false;
                }
            };
            _connection.Close();
            return result;
        }
    }
}
