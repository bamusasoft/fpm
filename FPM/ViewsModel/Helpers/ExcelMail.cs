using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;
using SysData = System.Data;
namespace Fbm.ViewsModel.Helpers
{
    public class ExcelMail
    {
        void ForceExcleToQuit(_Application objExcel)
        {
            if (objExcel != null)
            {
                objExcel.DisplayAlerts = false;
                objExcel.Quit();
            }
        }
        public void SendTable(SysData.DataTable table, string excelTemplatePath, string pdfFilePath)
        {
            if (table == null) throw new ArgumentNullException("table");
            if (string.IsNullOrEmpty(excelTemplatePath))
            {
                throw new ArgumentNullException("excelTemplatePath", "You must specify path to the excel workbook");
            }
            if (string.IsNullOrEmpty(pdfFilePath))
            {
                throw new ArgumentNullException("pdfFilePath", "You must specify path to the excel workbook");
            }
            _Application excelApp = null;
            Workbooks excelWorkbooks = null;
            _Workbook excelWorkbook = null;
            _Worksheet excelSheet = null;
            string fileNamePath = excelTemplatePath; ;
            bool showExcel = Properties.Settings.Default.ShowReports;
            //Start Excel and create new workbook from the template
            excelApp = StartExcel();
            try
            {
                excelWorkbooks = excelApp.Workbooks;
                excelWorkbook = excelWorkbooks.Open(fileNamePath);
                excelSheet = excelWorkbook.Sheets[1];
                //Insert the DataGridView into the excel spreadsheet
                TableToExcelSheet(table, excelSheet, 3, 1);
                if (!showExcel)
                {
                    excelSheet.PrintOut();
                    excelApp.DisplayAlerts = false;
                    excelApp.Quit();
                    ReleaseResources(excelApp, excelWorkbooks, excelWorkbook, excelSheet);
                    return;
                }
                excelApp.Visible = true;
                ReleaseResources(excelApp, excelWorkbooks, excelWorkbook, excelSheet);
            }
            catch (Exception ex)
            {
                Helper.LogOnly(ex);
                ForceExcleToQuit(excelApp);
                ReleaseResources(excelApp, excelWorkbooks, excelWorkbook, excelSheet);
                throw;
            }
        }
       private void FillExcelForm(SysData.DataTable table,_Worksheet excelSheet, int startRow,int startCol)
        {
           var breakValueIndex = table.Columns.IndexOf("BreakRow");
           object t = "True";
            for (int nRow = 0; nRow < table.Rows.Count; nRow++)
            {
                var breakRow =  Convert.ToBoolean(table.Rows[nRow].ItemArray[breakValueIndex]);
                for (int nCol = 0; nCol < table.Columns.Count; nCol++)
                {
                   if (table.Columns[nCol].Caption == "BreakRow") continue;
                   excelSheet.Cells[startRow + nRow, startCol + nCol] = table.Rows[nRow].ItemArray[nCol]; //table.Rows[nRow].Cells[nCol].Value;
                   if (breakRow)
                   {
                       var cell = excelSheet.Cells[startRow + nRow, startCol + nCol];
                       FormatCell(cell);
                   }
                }
            }

        }
       private void TableToExcelSheet(SysData.DataTable table, _Worksheet excelSheet, int startRow,int startCol)
       {
           for (int nRow = 0; nRow < table.Rows.Count; nRow++)
           {
               for (int nCol = 0; nCol < table.Columns.Count; nCol++)
               {
                   excelSheet.Cells[startRow + nRow, startCol + nCol] = table.Rows[nRow].ItemArray[nCol]; //table.Rows[nRow].Cells[nCol].Value;
               }
           }
       }

       private void WriteCell(_Worksheet excelSheet, int rowIndex, int colIndex, object value)
       {
           excelSheet.Cells[rowIndex, colIndex] = value;
       }
       private void FormatCell(Range cell)
       {
           cell.Borders[XlBordersIndex.xlEdgeBottom].Weight = XlBorderWeight.xlMedium;
       }
        private _Application StartExcel()
        {
            return new Application();
        }
        void ReleaseResources(params object[] objs)
        {
            for (int i = 0; i < objs.Length; i++)
            {
                if (objs[i] != null)
                {
                    Marshal.FinalReleaseComObject(objs[i]);
                }
            }
        }
        public void SendMemberStatment(MemberStatmentReport report, string templatePath, string pdfPath)
        {
            if (report == null)
            {
                throw new ArgumentNullException("report");
            }
            _Application excelApp = null;
            Workbooks excelWorkbooks = null;
            _Workbook excelWorkbook = null;
            _Worksheet excelSheet = null;
            string fileNamePath = templatePath; 
            bool showExcel = Properties.Settings.Default.ShowReports;
            //Start Excel and create new workbook from the template
            excelApp = StartExcel();
            try
            {
                excelWorkbooks = excelApp.Workbooks;
                excelWorkbook = excelWorkbooks.Open(fileNamePath);
                excelSheet = excelWorkbook.Sheets[1];
                //Excel Cell C1 = Statment Year
                WriteCell(excelSheet, 1, 6, report.Year);
                //Excel Cell A3 = Member Code
                WriteCell(excelSheet, 3, 1, report.MemberCode);
                //Excel Cell F3 = Member Full Name
                WriteCell(excelSheet, 3, 6, report.MemberName);
                //Excel Cell G3 = Statment Net Total
                WriteCell(excelSheet, 4, 7, report.TotalNetPayment);
                //Write Report Detials
                FillExcelForm(report.DetailsTable, excelSheet, 8, 1);
                if (!showExcel)
                {
                    excelWorkbook.ExportAsFixedFormat(XlFixedFormatType.xlTypePDF,
                                                      pdfPath + "\\" + report.MemberCode.ToString() + ".pdf",
                                                       XlFixedFormatQuality.xlQualityStandard,
                                                       true);
                    ReleaseResources(excelApp, excelWorkbooks, excelWorkbook, excelSheet);
                    return;
                }
               
                excelApp.Visible = true;
                ReleaseResources(excelApp, excelWorkbooks, excelWorkbook, excelSheet);
            }
            catch (Exception ex)
            {
                Helper.LogOnly(ex);
                ForceExcleToQuit(excelApp);
                ReleaseResources(excelApp, excelWorkbooks, excelWorkbook, excelSheet);
                throw;
            }


        }


    }
}
