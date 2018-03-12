using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using FlopManager.Services.Helpers;
using FlopManagerLoanModule.DTOs;
using Microsoft.Office.Interop.Word;

namespace FlopManagerLoanModule.ModuleServices
{
    public class WordMail
    {
        private Dictionary<string, string> Values { get; set; }
        
        public void Send(LoansStatementReport report, string fileName, string pdfFileName)
        {
            if (report == null) throw new ArgumentNullException("report");
            if (string.IsNullOrEmpty(fileName)) throw new ArgumentNullException("fileName");
            _Application wordApp = null;
            Documents wordDocs = null;
            Document wordDoc = null;
            bool showWord = false;//Properties.Settings.Default.ShowReports;
            try
            {
                wordApp = StartWord();
                wordDocs = wordApp.Documents;
                wordDoc = wordDocs.Add(fileName);

                Fields docFields = wordDoc.Fields;
                Values = BuildValues(report);
                string pdf = pdfFileName + "\\" + report.MemberCode.ToString();
                foreach (Field f in docFields)
                {
                    if (!WriteField(f)) f.Result.Text = string.Empty;
                }
                if (!showWord)
                {
                    wordDoc.ExportAsFixedFormat(pdf, WdExportFormat.wdExportFormatPDF, true, WdExportOptimizeFor.wdExportOptimizeForOnScreen);
                    ReleaseResources(wordApp, wordDocs, wordDoc);
                    return;
                }
                wordApp.Visible = true;
                ReleaseResources(wordApp, wordDocs, wordDoc);
            }
            catch (Exception ex)
            {
                Helper.LogOnly(ex);
                ForceExcleToQuit(wordApp);
                ReleaseResources(wordApp, wordDocs, wordDoc);
                throw;
            }
            

           
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
        void ForceExcleToQuit(_Application wordApp)
        {
            if (wordApp != null)
            {
                wordApp.DisplayAlerts = WdAlertLevel.wdAlertsNone;
                wordApp.Quit();
            }
        }

        private bool WriteField(Field f)
        {
            bool found = false;
            foreach (var key in Values.Keys)
            {
                if (f.Code.Text.Contains(key))
                {
                    f.Result.Text = Values[key];
                    
                    found = true;
                    break;
                }
                
                
            }
            return found;
        }

        Dictionary<string,string> BuildValues(LoansStatementReport report)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("R101", report.MemberCode.ToString());
            dic.Add("R102", report.MemberName.ToString());
            dic.Add("R103", report.LoansTotal.ToString("0,0.00"));
            dic.Add("R104", report.PaidTotal.ToString("0,0.00"));
            dic.Add("R105", report.BalanceTotal.ToString("0,0.00"));
            int counter = 101;
            foreach (var detail in report.DetailsCollection)
            {
                var properties = from p in typeof(LoansStatementDetailes).GetProperties()
                                 where
                                     p.GetCustomAttributes(true).Cast<Attribute>()
                                     .Contains(new BindableAttribute(true))
                                 select p;
                foreach (var pro in properties)
                {
                    string key = "D" + counter.ToString();
                    string lastNo = key.Substring(3, 1);

                    switch (lastNo)
                    {
                        case "1":
                        case "6":
                            dic.Add(key, pro.GetValue(detail).ToString());
                            break;
                        case "2":
                        case "7":
                            dic.Add(key, pro.GetValue(detail).ToString());
                            break;
                        case "3":
                        case "8":
                            dic.Add(key, ((decimal) pro.GetValue(detail)).ToString("0,0.00"));
                            break;
                        case "4":
                        case "9":
                            dic.Add(key, ((decimal)pro.GetValue(detail)).ToString("0,0.00"));
                            break;
                        case "5":
                        case "0":
                            dic.Add(key, ((decimal)pro.GetValue(detail)).ToString("0,0.00"));
                            break;

                        default:
                            throw new NotSupportedException(); ;
                    }
                    counter++;
                }
              
                
            }
            return dic;


        }

        private _Application StartWord()
        {
            return new Application();
        }

    }
}
