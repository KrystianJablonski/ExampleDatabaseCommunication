using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ViewModel.MVVM
{
    /// <summary>
    /// View model command class allows for binding actions with WPF commands.
    /// </summary>
    public class ViewModelCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private bool _canExecute;
        public bool CanExecuteValue
        {
            get => _canExecute;
            set
            {
                _canExecute = value;
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        private Action _executeAction;

        /// <summary>
        /// ViewModelCommand constructor.
        /// Throws <seealso cref="ArgumentOutOfRangeException"/> when passed <paramref name="executeAction"/> is null.
        /// </summary>
        /// <param name="executeAction">Command action to execute when <seealso cref="ICommand.Execute(object?)"/> is invoked</param>
        /// <param name="canExecute">Current can execute command value</param>
        public ViewModelCommand(Action executeAction, bool canExecute = true)
        {
            if (executeAction == null)
                throw new ArgumentNullException(nameof(executeAction));
            _executeAction = executeAction;
            _canExecute = canExecute;
        }

        /// <summary>
        /// <seealso cref="ICommand"/> implementation of <seealso cref="ICommand.CanExecute(object?)"/> method. 
        /// </summary>
        /// <returns><seealso cref="_canExecute"/></returns>
        public bool CanExecute(object parameter) => _canExecute;

        /// <summary>
        /// <seealso cref="ICommand"/> implementation of <seealso cref="ICommand.Execute(object?)"/> method. 
        /// Invokes <seealso cref="_executeAction"/>.
        /// </summary>
        public void Execute(object parameter)
        {
            _executeAction.Invoke();
        }
    }
}
