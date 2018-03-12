﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlopManager.Services.ViewModelInfrastructure
{
    public interface ISettings
    {
        object this[string propertyName] { get; set; }
        void Save();
    }
}
