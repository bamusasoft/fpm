using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlopManager.Services.ViewModelInfrastructure
{
    public enum ViewModelState
    {
        AddNew,
        InEdit,
        Busy,
        Saved,
        Deleted
    }
}
