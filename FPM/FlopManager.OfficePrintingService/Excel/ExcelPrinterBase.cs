using System;
using System.Runtime.InteropServices;
using FlopManager.OfficePrintingService.Core;
using Microsoft.Office.Interop.Excel;
using SysData = System.Data;

namespace FlopManager.OfficePrintingService.Excel
{
    public abstract class ExcelPrinterBase:IOfficePrinter
    {
        #region Properties
        protected _Application ExcelApp { get; private set; }
        protected Workbooks ExcelWorkbooks { get; private set; }
        protected _Workbook ExcelWorkbook { get; private set; }
        protected _Worksheet CurrentExcelSheet { get; set; }

        #endregion

        public abstract void Print(IOfficeFilePropties properties);

        protected void ForceExcleToQuit(_Application objExcel)
        {
            if (objExcel != null)
            {
                objExcel.DisplayAlerts = false;
                objExcel.Quit();
            }
        }

        protected _Application StartExcel()
        {
            return new Application();
        }
        protected void ReleaseResources(params object[] objs)
        {
            for (int i = 0; i < objs.Length; i++)
            {
                if (objs[i] != null)
                {
                    Marshal.FinalReleaseComObject(objs[i]);
                }
            }
        }
        protected void TableToExcelSheet(SysData.DataTable table, _Worksheet excelSheet, int startRow, int startCol)
        {
            for (int rowIndex = 0; rowIndex < table.Rows.Count; rowIndex++)
            {
                for (int colIndex = 0; colIndex < table.Columns.Count; colIndex++)
                {
                    int cellRowIndex = startCol + rowIndex;
                    int celColIndex = startCol + colIndex;
                    object value = table.Rows[rowIndex].ItemArray[colIndex];
                    //excelSheet.Cells[startRow + rowIndex, startCol + colIndex] = table.Rows[rowIndex].ItemArray[colIndex]; //table.Rows[nRow].Cells[nCol].Value;
                    WriteCell(excelSheet, cellRowIndex, celColIndex, value);
                }
            }
        }
        protected void WriteCell(_Worksheet excelSheet, int curentCellRowIndex, int currentCellColIndex, object value)
        {
            excelSheet.Cells[curentCellRowIndex, currentCellColIndex] = value;
        }


        protected _Worksheet GetExcelSheet(string path)
        {
            ExcelApp =StartExcel();
            ExcelWorkbooks = ExcelApp.Workbooks;
            ExcelWorkbook = ExcelWorkbooks.Open(path);
            return ExcelWorkbook.Sheets[1];
        }
        /// <summary>
        /// If you will override, do not call base method. Provide your own implementation.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="excelSheet"></param>
        /// <param name="startRow"></param>
        /// <param name="startCol"></param>
        protected virtual void FillExcelForm(SysData.DataTable table, _Worksheet excelSheet, int startRow, int startCol)
        {
            var breakValueIndex = table.Columns.IndexOf("BreakRow");
            object t = "True";
            for (int rowIndex = 0; rowIndex < table.Rows.Count; rowIndex++)
            {
                var breakRow = Convert.ToBoolean(table.Rows[rowIndex].ItemArray[breakValueIndex]);
                for (int colIndex = 0; colIndex < table.Columns.Count; colIndex++)
                {
                    if (table.Columns[colIndex].Caption == "BreakRow") continue;
                    //excelSheet.Cells[startRow + nRow, startCol + nCol] = table.Rows[nRow].ItemArray[nCol]; 
                    int cellRowIndex = startCol + rowIndex;
                    int celColIndex = startCol + colIndex;
                    object value = table.Rows[rowIndex].ItemArray[colIndex];
                    WriteCell(excelSheet, cellRowIndex, celColIndex, value);
                    if (breakRow)
                    {
                        var cell = excelSheet.Cells[startRow + rowIndex, startCol + colIndex];
                        FormatCell(cell);
                    }
                }
            }

        }

        protected virtual void FormatCell(Range cell)
        {
            cell.Borders[XlBordersIndex.xlEdgeBottom].Weight = XlBorderWeight.xlMedium;
        }

    }
}
