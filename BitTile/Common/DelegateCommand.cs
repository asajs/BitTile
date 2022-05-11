using System;
using System.Windows.Input;

namespace BitTile.Common
{
	public record DelegateCommand : ICommand
	{
		private readonly Action _executeAction;
		private readonly Func<bool> _canExecuteAction;
		private readonly Action<object> _executeWithObject;

		public DelegateCommand(Action<object> action, Func<bool> canExecute = null)
		{
			_executeWithObject = action;
			_executeAction = null;
			_canExecuteAction = canExecute;
		}

		public DelegateCommand(Action action, Func<bool> canExecute = null)
		{
			_executeAction = action;
			_canExecuteAction = canExecute;
			_executeWithObject = null;
		}

		public event EventHandler CanExecuteChanged;

		public bool CanExecute(object parameter)
		{
			return _canExecuteAction?.Invoke() ?? true;
		}

		public void Execute(object parameter)
		{
			if (!CanExecute(parameter))
			{
				return;
			}
			else if(_executeAction is not null)
			{ 	
				_executeAction.Invoke();
			}
			else if(_executeWithObject is not null)
			{
				_executeWithObject.Invoke(parameter);
			}
		}
	}

	public record DelegateCommand<T> : ICommand
	{
		private readonly Func<T, bool> _canExecuteAction;
		private readonly Action<T> _executeWithObject;

		public DelegateCommand(Action<T> action, Func<T, bool> canExecute = null)
		{
			_executeWithObject = action;
			_canExecuteAction = canExecute;
		}

		public event EventHandler CanExecuteChanged;

		public bool CanExecute(object parameter)
		{
			if (parameter is T param)
			{
				return _canExecuteAction?.Invoke(param) ?? true;
			}
			return false;
		}

		public void Execute(object parameter)
		{
			if (!CanExecute(parameter))
			{
				return;
			}
			else if (_executeWithObject is not null && parameter is T param)
			{
				_executeWithObject.Invoke(param);
			}
		}
	}
}
