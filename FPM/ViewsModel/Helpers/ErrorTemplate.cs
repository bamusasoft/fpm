using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;

namespace Fbm.ViewsModel.Helpers
{
    public class ErrorTemplate
    {
        dynamic _errorMessage;
        Action _closeMethod;
        RelayCommand _closeCommand;
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
                    _closeCommand = new RelayCommand(_closeMethod);
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
