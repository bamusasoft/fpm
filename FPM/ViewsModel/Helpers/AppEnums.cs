using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fbm.ViewsModel.Helpers
{
    public enum ModelState
    { 
        New,
        Saved,
        Deleted,
    }
    public enum ViewState
    { 
        New,
        Saved,
        Edited,
        HasErrors,
        Deleted,
    }
    public enum LogMessageTypes
    {
        Error,
        Info,
    }
}
