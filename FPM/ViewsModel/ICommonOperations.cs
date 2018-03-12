using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Fbm.ViewsModel.Helpers;

namespace Fbm.ViewsModel
{
    public interface ICommonOperations: INotifyPropertyChanged
    {
        ICommand SaveCommand { get; }
        ICommand AddNewCommand { get; }
        ICommand SearchCommand { get; }
        ICommand DeleteCommand { get; }
        ICommand PrintCommand { get; }
        ICommand EditCommand { get; }
        void SetState(ModelState state);
        void SetChangedFlag();
        void ResetChangedFalg();
        bool OkClose();
        List<RuleViolation> RulesViolations { get; set; }
        void Initialize();
        void RaisePropertyChanged([CallerMemberName] string propertyName = null);
        

    }
}
