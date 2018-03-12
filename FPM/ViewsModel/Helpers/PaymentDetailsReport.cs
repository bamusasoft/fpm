using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fbm.DomainModel;
using Fbm.DomainModel.Entities;

namespace Fbm.ViewsModel.Helpers
{
    public class PaymentDetailsReport
    {

        public Guid DetailNo { get; private set; }
        public object ShareHolderCode { get; private set; }
        public object ShareHolderName { get; private set; }
        public Sex Sex { get; set; }

        public object PaymentYear { get; private set; }
        public object PaymentSequence { get; private set; }
        public object PaymentAmount { get; private set; }
        public object ShareNo { get; private set; }
        public object TotalPayment { get; private set; }
        public object LoanAmount { get; private set; }
        public object LoanDescription { get; private set; }
        public object LoanRemarks { get; private set; }
        public object LoansTotal { get; private set; }
        public object NetPayments { get; private set; }

        public PaymentDetailsReport(Guid detailNo, object shareHolderCode, object shareHolderName, object paymentYear,
                                    object paymentSeqeuence, object paymentAmount, object shareNo,
                                    object totalPayment, object loanAmount, object loanDescription,
                                    object loanRemraks, object loansTotal, object netPayment, Sex memberSex)
        {
            DetailNo = detailNo;
            ShareHolderCode = shareHolderCode;
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
        }

        public PaymentDetailsReport(Guid detailNo, decimal loanAmount,
                                    string loanDescription, string loanRemraks, Sex memberSex)
            : this(detailNo, null, null, null, null, null, null, null,
                  loanAmount, loanDescription, loanRemraks, null, null, memberSex)
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
                table.Rows.Add(row);
                row.AcceptChanges();
            }
            return table;

        }

    }
}
