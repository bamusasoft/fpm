using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlopManager.Services
{
    public interface IResouece
    {
        string this[string propertyName] { get; }
    }
}
