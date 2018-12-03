using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Builders;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlopManagerLoanModule.DTOs
{
    public class LoanStatementDto
    {
        public string LoanNo { get; set; }    
        public string Description { get; set; }
        public  int MemberCode { get; set; }
        public string FullName { get; set; }
        public int LoanTypeCode { get; set; }
        public string LoanDescription { get; set; }
        public string Year { get; set; }
        public int PaySeqDue { get; set; }
        public string SequenceDescription { get; set; }
        public decimal Amount { get; set; }
        public decimal? AmountPaid { get; set; }

        public decimal Balance
        {
            get
            {
                if (AmountPaid == null)
                {
                    return Amount;
                }

                return (decimal) (Amount - AmountPaid);
            }
        }
        public string Remarks { get; set; }
        public byte Status { get; set; }

        public string StatusArabic
        {
            get
            {
                switch (Status)
                {
                    case 0:
                        return "غير مسدد";
                    case 1:
                        return "مسدد";
                    default:
                        throw new InvalidOperationException("Undefined Status");
                }
            }
        }
    }
}
