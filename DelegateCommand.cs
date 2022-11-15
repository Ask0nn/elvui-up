using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ElvUiUpdater
{
    internal class DelegateCommand : ICommand
    {
        private readonly Action<object> _execute;

        private readonly Func<object, bool>? _canExecute;

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public DelegateCommand(Action<object> execute, Func<object, bool>? canExecute = null)
        {
            this._execute = execute;
            this._canExecute = canExecute;
        }

        public bool CanExecute(object? parameter)
        {
            return this._canExecute == null || this.CanExecute(parameter);
        }

        public void Execute(object? parameter)
        {
            this._execute(parameter);
        }
    }
}
