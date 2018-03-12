using System.Collections.Generic;
using System.Data;

namespace FlopManager.PaymentsModule.DTOs
{
    public class MemberStatmentReport
    {
        public object Year { get;  set; }
        public object MemberCode { get;  set; }
        public object MemberName { get;  set; }
        public object TotalNetPayment { get;  set; }
        public DataTable DetailsTable { get;  set; }
        public IEnumerable<MemberStatmentDetail> DetailsCollection
        {
            get;
            set;
        }
        public MemberStatmentReport(string year, int memberCode, string memberName, decimal totalNetPayment, IEnumerable<MemberStatmentDetail> details)
        {
            Year = year;
            MemberCode = memberCode;
            MemberName = memberName;
            TotalNetPayment = totalNetPayment;
            DetailsCollection = details;
            DetailsTable = FillData(details);
        }
        //public DataTable AddDetails(IList<MemberStatmentDetail> details)
        //{
        //    DetailsCollection = details;
        //    DetailsTable = FillData(details);
        //    return DetailsTable;
        //}
        //public MemberStatmentReport() { }

        private static DataTable CreateTable()
        {
            DataTable table = new DataTable("MemberStatmentReport");
            AddColumns(table);
            return table;


        }
        private static void AddColumns(DataTable table)
        {
            DataColumn c1 = new DataColumn("RegistrationDate");
            table.Columns.Add(c1);
            DataColumn c2 = new DataColumn("PaymentSequence");
            table.Columns.Add(c2);

            DataColumn c3 = new DataColumn("PaymentAmount");
            table.Columns.Add(c3);

            DataColumn c4 = new DataColumn("MemberShares");
            table.Columns.Add(c4);

            DataColumn c5 = new DataColumn("PaymentTotal");
            table.Columns.Add(c5);

            DataColumn c6 = new DataColumn("LoanAmount");
            table.Columns.Add(c6);

            DataColumn c7 = new DataColumn("LoanDescription");
            table.Columns.Add(c7);

            //DataColumn c8 = new DataColumn("LoansTotal");
            //table.Columns.Add(c8);

            DataColumn c9 = new DataColumn("PaymentNetAmount");
            table.Columns.Add(c9);

            DataColumn c10 = new DataColumn("BreakRow");
            table.Columns.Add(c10);



        }

        private static DataTable FillData(IEnumerable<MemberStatmentDetail> data)
        {
            DataTable table = CreateTable();
            foreach (var report in data)
            {
                DataRow row = table.NewRow();
                row.SetField<object>("RegistrationDate", report.RegistrationDate);
                row.SetField<object>("PaymentSequence", report.PaymentSequence);
                row.SetField<object>("PaymentAmount", report.PaymentAmount);
                row.SetField<object>("MemberShares", report.MemberShares);
                row.SetField<object>("PaymentTotal", report.PaymentTotal);
                row.SetField<object>("LoanAmount", report.LoanAmount);
                row.SetField<object>("LoanDescription", report.LoanDescription);
                //row.SetField<object>("LoansTotal", report.LoansTotal);
                row.SetField<object>("PaymentNetAmount", report.PaymentNetAmount);
                row.SetField<bool>("BreakRow", report.BreakRow);

                table.Rows.Add(row);
                row.AcceptChanges();
            }
            return table;

        }
    }
    public class MemberStatmentDetail
    {

        public object RegistrationDate { get; set; }
        public object PaymentSequence { get; set; }
        public object PaymentAmount { get; set; }
        public object MemberShares { get; set; }
        public object PaymentTotal
        {
            get
            {
                if (PaymentAmount == null || MemberShares == null) return null;
                return ((decimal) PaymentAmount * (int) MemberShares);
            }
        }
        public object LoanAmount { get; set; }
        public object LoanDescription { get; set; }
        //public object LoansTotal { get; set; }
        public object PaymentNetAmount { get; set; }
        public bool BreakRow { get; set; }
        public MemberStatmentDetail(object regDate, object sequence, object paymentAmount, object memberShares,
            object loanAmount, object loanDesc, object netPay, bool breaRow)
        {
            RegistrationDate = regDate;
            PaymentSequence = sequence;
            PaymentAmount = paymentAmount;
            MemberShares = memberShares;
            LoanAmount = loanAmount;
            LoanDescription = loanDesc;
            PaymentNetAmount = netPay;
            BreakRow = breaRow;
            //if (BreakRow)
            //{
            //    LoanDescription = "الإجمالي";
            //}
        }
        public MemberStatmentDetail(decimal loanAmount, string loanDesc, bool breakRow)
            : this(null, null, null, null, loanAmount, loanDesc,null, breakRow)
        { 
        
        }

      
    }
}
