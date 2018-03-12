using System;
using FlopManager.OfficePrintingService.Core;

namespace FlopManager.OfficePrintingService.Excel.Templates
{
    public class PaymentTransTempPrinter : ExcelPrinterBase
    {
        public override void Print(IOfficeFilePropties properties)
        {
            if (properties == null) throw new ArgumentNullException(nameof(properties));
            var p = properties as ExcelFileProperties;
            if (p == null)
            {
                throw new InvalidOperationException("You have to supply valid Excel File Properties");
            }
            try
            {
                CurrentExcelSheet = GetExcelSheet(p.Path);
                TableToExcelSheet(p.Source, CurrentExcelSheet, p.StartRowIndex, p.StartcolumnIndex);
                if (p.PrintDirectly)
                {
                    CurrentExcelSheet.PrintOut();
                    ExcelApp.DisplayAlerts = false;
                    ExcelApp.Quit();
                    ReleaseResources(ExcelApp, ExcelWorkbooks, ExcelWorkbook, CurrentExcelSheet);
                    return;
                }
                ExcelApp.Visible = true;
                //ReleaseResources(excelApp, excelWorkbooks, excelWorkbook, excelSheet);
            }
            catch
            {
                ForceExcleToQuit(ExcelApp);
                ReleaseResources(ExcelApp, ExcelWorkbooks, ExcelWorkbook, CurrentExcelSheet);
                throw;
            }
        }
    }
}