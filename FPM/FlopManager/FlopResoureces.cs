using FlopManager.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlopManager
{
    [Export(typeof(IResouece))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class FlopResoureces : IResouece
    {
        public string this[string propertyName]
        {
            get
            {
                return FlopManager.Properties.Resources.ResourceManager.GetString(propertyName);
            }
        }
    }
}
