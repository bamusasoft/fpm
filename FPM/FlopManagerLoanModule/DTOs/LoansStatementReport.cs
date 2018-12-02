using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlopManagerLoanModule.DTOs
{
    public class LoansStatementReport
    {
        static DataTable _internalTable;
        DataTable _detailsTable;
        public object MemberCode { get; set; }
        public object MemberName { get; set; }
        public decimal LoansTotal { get; set; }
        public decimal PaidTotal { get; set; }
        public decimal BalanceTotal { get; set; }

        public IList<LoansStatementDetailes> DetailsCollection { get; private set; }
        public LoansStatementReport(object memberCode, object memberName, decimal loansTotal, decimal paidTotal,
            decimal balanceTotal, IList<LoansStatementDetailes> details)
        {
            MemberCode = memberCode;
            MemberName = memberName;
            LoansTotal = loansTotal;
            PaidTotal = paidTotal;
            BalanceTotal = balanceTotal;
            DetailsCollection = details;


        }

        public DataTable DetailsTable
        {
            get
            {
                if (_detailsTable == null && DetailsCollection != null)
                {
                    _detailsTable = FillData(DetailsCollection);
                }
                return _detailsTable;
            }

        }

        private static DataTable CreateTable()
        {
            if (_internalTable == null)
            {
                _internalTable = new DataTable("LoansStatmentReport");
                AddColumns(_internalTable);
            }
            _internalTable.Clear();
            return _internalTable;


        }
        private static void AddColumns(DataTable table)
        {
            DataColumn c1 = new DataColumn("DateCreated");
            table.Columns.Add(c1);
            DataColumn c2 = new DataColumn("Description");
            table.Columns.Add(c2);

            DataColumn c3 = new DataColumn("LoanAmount");
            table.Columns.Add(c3);

            DataColumn c4 = new DataColumn("Paid");
            table.Columns.Add(c4);

            DataColumn c5 = new DataColumn("Balance");
            table.Columns.Add(c5);

            //DataColumn c6 = new DataColumn("LoanAmount");
            //table.Columns.Add(c6);

            //DataColumn c7 = new DataColumn("LoanDescription");
            //table.Columns.Add(c7);

            ////DataColumn c8 = new DataColumn("LoansTotal");
            ////table.Columns.Add(c8);

            //DataColumn c9 = new DataColumn("PaymentNetAmount");
            //table.Columns.Add(c9);

            //DataColumn c10 = new DataColumn("BreakRow");
            //table.Columns.Add(c10);



        }
        private DataTable FillData(IList<LoansStatementDetailes> data)
        {
            DataTable table = CreateTable();
            foreach (var report in data)
            {
                DataRow row = table.NewRow();
                row.SetField<object>("DateCreated", report.DateCreated);
                row.SetField<object>("Description", report.Description);
                row.SetField<decimal>("LoanAmount", report.LoanAmount);
                row.SetField<decimal>("Paid", report.Paid);
                row.SetField<decimal>("Balance", report.Balance);
                //row.SetField<object>("LoanAmount", report.LoanAmount);
                //row.SetField<object>("LoanDescription", report.LoanDescription);
                ////row.SetField<object>("LoansTotal", report.LoansTotal);
                //row.SetField<object>("PaymentNetAmount", report.PaymentNetAmount);
                //row.SetField<bool>("BreakRow", report.BreakRow);

                table.Rows.Add(row);
                row.AcceptChanges();
            }
            return table;
        }
        void Print()
        {

        }

    }
    public class LoansStatementDetailes
    {
        [Bindable(true)]
        public object DateCreated { get; set; }
        [Bindable(true)]
        public object Description { get; set; }
        [Bindable(true)]
        public decimal LoanAmount { get; set; }
        [Bindable(true)]
        public decimal Paid { get; set; }
        [Bindable(true)]
        public decimal Balance { get; set; }
        [Bindable(true)]
        public LoansStatementDetailes(object dateCreated, object description, decimal loanAmount, decimal paid, decimal balance)
        {
            DateCreated = dateCreated;
            Description = description;
            LoanAmount = loanAmount;
            Paid = paid;
            Balance = balance;
        }

    }
}
