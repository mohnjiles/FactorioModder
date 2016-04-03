using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FactorioModder.Utility
{
    public class ParameterCommand : ICommand
    {
        private readonly Action<object> _action;
        private bool _canExecute;

        public ParameterCommand(Action<object> action, bool canExecute = true)
        {
            _action = action;
            _canExecute = canExecute;
        }

        protected ParameterCommand(bool canExecute)
        {
            _canExecute = canExecute;
        }

        public virtual void Execute(object parameter)
        {
            _action(parameter);
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
