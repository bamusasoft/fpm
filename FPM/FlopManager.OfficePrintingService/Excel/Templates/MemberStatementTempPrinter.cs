using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlopManager.OfficePrintingService.Core;
using Microsoft.Office.Interop.Excel;

namespace FlopManager.OfficePrintingService.Excel.Templates
{
    class MemberStatementTempPrinter:ExcelPrinterBase
    {
        public override void Print(IOfficeFilePropties properties)
        {
            ExcelFileProperties p = properties as ExcelFileProperties;
            if (p == null)
            {
                throw new InvalidOperationException("You have to supply valid Excel File Properties");
            }
            try
            {
                CurrentExcelSheet = GetExcelSheet(p.Path);

                //Excel Cell C1 = Statment Year
                WriteCell(CurrentExcelSheet, 1, 6, p.ReportHeaderValues[0]);
                //Excel Cell A3 = Member Code
                WriteCell(CurrentExcelSheet, 3, 1, p.ReportHeaderValues[1]);
                //Excel Cell F3 = Member Full Name
                WriteCell(CurrentExcelSheet, 3, 6, p.ReportHeaderValues[2]);
                //Excel Cell G3 = Statment Net Total
                WriteCell(CurrentExcelSheet, 4, 7, p.ReportHeaderValues[3]);
                //Write Report Detials
                FillExcelForm(p.Source, CurrentExcelSheet, 8, 1);
                if (p.PrintDirectly)
                {
                    string defaultName = p.ReportHeaderValues[2].ToString(); //get member code from header values, which supposed to be at index 2.
                    CurrentExcelSheet.ExportAsFixedFormat(XlFixedFormatType.xlTypePDF,
                                                     $"{p.SaveFolderPath}\\{defaultName}.pdf",
                                                       XlFixedFormatQuality.xlQualityStandard,
                                                       true);
                    ReleaseResources(ExcelApp, ExcelWorkbooks, ExcelWorkbook, CurrentExcelSheet);
                    return;
                }

                ExcelApp.Visible = true;
            }

            catch (Exception ex)
            {
                ForceExcleToQuit(ExcelApp);
                ReleaseResources(ExcelApp, ExcelWorkbooks, ExcelWorkbook, CurrentExcelSheet);
                throw;
            }
        }

        
    

   
}
}
