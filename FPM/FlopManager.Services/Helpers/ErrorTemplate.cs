using System;
using System.Windows.Input;
using Prism.Commands;

namespace FlopManager.Services.Helpers
{
    public class ErrorTemplate
    {
        dynamic _errorMessage;
        Action _closeMethod;
        DelegateCommand _closeCommand;
        public dynamic ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;

            }
        }
        public ICommand CloseCommand
        {
            get
            {
                if (_closeCommand == null)
                {
                    _closeCommand = new DelegateCommand(_closeMethod);
                }
                return _closeCommand;
            }
        }
        public ErrorTemplate(dynamic errorMessage, Action closeMethod)
        {
            _errorMessage = errorMessage;
            _closeMethod = closeMethod;

        }
    }
}
