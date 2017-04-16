using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ExHentaiDownloader.Command
{
    public class CommandHandler : ICommand
    {
        private readonly Action<object> _command;
        private readonly Predicate<object> _canExecute;

        public CommandHandler(Action<object> command, Predicate<object> canExecute = null)
        {
            _command = command;
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            if(_canExecute == null)
            {
                return true;
            }
            return _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _command(parameter);
        }
    }
}
