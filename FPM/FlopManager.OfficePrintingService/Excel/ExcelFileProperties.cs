
using System.Collections.Generic;
using System.Data;
using FlopManager.OfficePrintingService.Core;

namespace FlopManager.OfficePrintingService.Excel
{
    public class ExcelFileProperties : IOfficeFilePropties
    {
        public string Path { get; set; }
        public bool PrintDirectly { get; set; }
        public DataTable Source { get; set; }
        public int StartRowIndex { get; set; }
        public int StartcolumnIndex { get; set; }
        public List<object> ReportHeaderValues { get; set; }
        public string SaveFolderPath { get; set; }
        
    }
}
