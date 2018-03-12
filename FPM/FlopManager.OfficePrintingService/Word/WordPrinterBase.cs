using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlopManager.OfficePrintingService.Core;

namespace FlopManager.OfficePrintingService.Word
{
    public abstract class WordPrinterBase:IOfficePrinter
    {
        public abstract void Print(IOfficeFilePropties properties);

    }
}
