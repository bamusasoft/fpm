using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Fbm.ViewsModel
{
    public interface IModelOperations<TModel>
    {
        void ReadModelValues(TModel model);
        void WriteModelValues(TModel model);
        bool ValidData();
        
    }
}
