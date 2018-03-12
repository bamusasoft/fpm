using System;
using FlopManager.Domain;

namespace FlopManager.PaymentsModule.DTOs
{
    public class SearchablePaymentDetails
    {
        public int DetailNo { get; private set; }
        public int MemberCode { get; private set; }
        public string MemberName { get; private set; }
        public Sex Sex { get; set; }
        public string PaymentYear { get; private set; }
        public string PaymentSequence { get; private set; }
        public decimal PaymentAmount { get; private set; }
        public int ShareNo { get; private set; }
        public decimal TotalPayment { get; private set; }
        public decimal LoanAmount { get; private set; }
        public string LoanDescription { get; private set; }
        public string LoanRemarks { get; private set; }
        public decimal LoansTotal { get; private set; }
        public decimal NetPayments { get; private set; }
        public bool IsHeader { get; private set; }
        public SearchablePaymentDetails(int detailNo, int memberCode, string memberName, string paymentYear, string paymentSeqeuence, 
                                    decimal paymentAmount,int shareNo, decimal totalPayment, decimal loanAmount, string loanDescription,
                                    string loanRemraks, decimal loansTotal, decimal netPayment,Sex memberSex,  bool isHeader)
        {
            DetailNo = detailNo;
            MemberCode = memberCode;
            MemberName = memberName;
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
            IsHeader = isHeader;
        }

    }
}
