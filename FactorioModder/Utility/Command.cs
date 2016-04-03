using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FactorioModder.Utility
{
    public class Command : ICommand
    {
        private readonly Action _action;
        private bool _canExecute;

        public Command(Action action, bool canExecute = true)
        {
            _action = action;
            _canExecute = canExecute;
        }

        protected Command(bool canExecute)
        {
            _canExecute = canExecute;
        }

        public virtual void Execute(object parameter)
        {
            _action();
        }

        public virtual bool CanExecute(object parameter)
        {
            //Temp in here for warning as error
            return _canExecute;
        }

        public void DisableCommand()
        {
            _canExecute = false;
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, new EventArgs());
        }

        public void EnabledCommand()
        {
            _canExecute = true;
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, new EventArgs());
        }

        public event EventHandler CanExecuteChanged;
    }
}
