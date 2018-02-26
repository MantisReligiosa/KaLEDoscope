using ServiceInterfaces;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace CommandProcessing
{
    public class Invoker
    {
        private ICommand _command;
        public ICommand Command
        {
            get
            {
                return _command;
            }
        }
        public void SetCommand(ICommand c)
        {
            _command = c;
        }
        public void Run()
        {
            _command.Execute();
        }
    }
}
