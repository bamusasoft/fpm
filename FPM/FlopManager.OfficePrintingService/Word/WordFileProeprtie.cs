using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlopManager.OfficePrintingService.Core;

namespace FlopManager.OfficePrintingService.Word
{
    public class WordFileProeprtie:IOfficeFilePropties
    {
        public string Path { get; set; }
        public bool PrintDirectly { get; set; }
    }
}
