using System;
using System.Collections.Generic;
using System.Data;
using FlopManager.Domain;

namespace FlopManager.Services.Helpers
{
    public class PaymentDetailsReport
    {

        public int DetailNo { get; }
        public object ShareHolderCode { get;  }
        public object ShareHolderName { get;  }
        public Sex Sex { get; set; }

        public object PaymentYear { get;  }
        public object PaymentSequence { get;  }
        public object PaymentAmount { get; }
        public object ShareNo { get;  }
        public object TotalPayment { get;  }
        public object LoanAmount { get;  }
        public object LoanDescription { get;  }
        public object LoanRemarks { get; }
        public object LoansTotal { get;  }
        public object NetPayments { get; }
        public object PayMethod { get; }
        public object BankDocNo { get; }
        public object MemberSex
        {
            get
            {
                if(Sex == Sex.Female)
                {
                    return "سيدات";
                }
                return "رجال";
            }
        }

        public PaymentDetailsReport(int detailNo, object memberCode, object shareHolderName, object paymentYear,
                                    object paymentSeqeuence, object paymentAmount, object shareNo,
                                    object totalPayment, object loanAmount, object loanDescription,
                                    object loanRemraks, object loansTotal, object netPayment, Sex memberSex, object payMethod, object bankDocNo)
        {
            DetailNo = detailNo;
            ShareHolderCode = memberCode;
            ShareHolderName = shareHolderName;
            PaymentYear = paymentYear;
            PaymentSequence = paymentSeqeuence;
            PaymentAmount = paymentAmount;
            ShareNo = shareNo;
            TotalPayment = totalPayment;
            LoanAmount = loanAmount;
            LoanDescription = loanDescription;
            LoanRemarks = loanRemraks;
            LoansTotal = loansTotal;
            NetPayments = netPayment;
            Sex = memberSex;
            PayMethod = payMethod;
            BankDocNo = bankDocNo;
        }

        public PaymentDetailsReport(int detailNo, int memberCode, decimal loanAmount,
                                    string loanDescription, string loanRemraks, Sex memberSex)
            : this(detailNo,memberCode, null, null, null, null, null, null,
                  loanAmount, loanDescription, loanRemraks, null, null, memberSex, null, null)
        { }

        public static DataTable CreateTable()
        {
            DataTable table = new DataTable("PaymentDetailsReport");
            AddColumns(table);
            return table;


        }
        private static void AddColumns(DataTable table)
        {
            DataColumn c1 = new DataColumn("ShareHolderCode");
            table.Columns.Add(c1);
            DataColumn c2 = new DataColumn("ShareHolderName");
            table.Columns.Add(c2);

            DataColumn c3 = new DataColumn("PaymentYear");
            table.Columns.Add(c3);

            DataColumn c4 = new DataColumn("PaymentSequence");
            table.Columns.Add(c4);

            DataColumn c5 = new DataColumn("PaymentAmount");
            table.Columns.Add(c5);

            DataColumn c6 = new DataColumn("ShareNo");
            table.Columns.Add(c6);

            DataColumn c7 = new DataColumn("TotalPayment");
            table.Columns.Add(c7);

            DataColumn c8 = new DataColumn("LoanAmount");
            table.Columns.Add(c8);

            DataColumn c9 = new DataColumn("LoanDescription");
            table.Columns.Add(c9);

            DataColumn c10 = new DataColumn("LoanRemarks");
            table.Columns.Add(c10);

            DataColumn c11 = new DataColumn("LoansTotal");
            table.Columns.Add(c11);

            DataColumn c12 = new DataColumn("NetPayments");
            table.Columns.Add(c12);

            DataColumn c13 = new DataColumn("PayMethod");
            table.Columns.Add(c13);
            DataColumn c14 = new DataColumn("MemberSex");
            table.Columns.Add(c14);

        }

        public static DataTable FillData(List<PaymentDetailsReport> data)
        {
            DataTable table = CreateTable();
            foreach (var report in data)
            {
                DataRow row = table.NewRow();
                row.SetField<object>("ShareHolderCode", report.ShareHolderCode);
                row.SetField<object>("ShareHolderName", report.ShareHolderName);
                row.SetField<object>("PaymentYear", report.PaymentYear);
                row.SetField<object>("PaymentSequence", report.PaymentSequence);
                row.SetField<object>("PaymentAmount", report.PaymentAmount);
                row.SetField<object>("ShareNo", report.ShareNo);
                row.SetField<object>("TotalPayment", report.TotalPayment);
                row.SetField<object>("LoanAmount", report.LoanAmount);
                row.SetField<object>("LoanDescription", report.LoanDescription);
                row.SetField<object>("LoanRemarks", report.LoanRemarks);
                row.SetField<object>("LoansTotal", report.LoansTotal);
                row.SetField<object>("NetPayments", report.NetPayments);
                row.SetField<object>("PayMethod", report.PayMethod);
                row.SetField<object>("MemberSex", report.MemberSex);

                table.Rows.Add(row);
                row.AcceptChanges();
            }
            return table;

        }

    }
}
