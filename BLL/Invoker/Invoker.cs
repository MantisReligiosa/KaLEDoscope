using ServiceInterfaces;

namespace CommandProcessing
{
    public class Invoker
    {
        private ICommand _command;
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
