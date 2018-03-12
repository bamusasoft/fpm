using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using FlopManager.Domain.EF;

namespace FlopManager.Domain
{
    public enum LoanStatus : byte
    {
       NotPaid = 0,
       Paid = 1,
    }
    public class Loan
    {
        public Loan()
        {
            LoanPayments = new HashSet<LoanPayment>();
        }

        [Key]
        [StringLength(6)]
        public string LoanNo { get; set; }

        [Required]
        [StringLength(100)]
        public string Description { get; set; }

        public int MemberCode { get; set; }

        public int LoanTypeCode { get; set; }
        [Required]
        [StringLength(4)]
        public string Year { get; set; }

        public int PaySeqDue { get; set; }

        [Column(TypeName = "money")]
        public decimal Amount { get; set; }


        public decimal Paid
        {
            get { return LoanPayments.Sum(x => x.AmountPaid); }
        }


        public decimal Balance
        {
            get { return (Amount - Paid); }
        }

        [StringLength(100)]
        public string Remarks { get; set; }

        public LoanStatus Status { get; set; }

        public virtual FamilyMember FamilyMember { get; set; }

        public virtual ICollection<LoanPayment> LoanPayments { get; set; }


        public virtual LoanType LoanType { get; set; }

        public virtual PaymentSequence PaymentSequence { get; set; }

        public virtual PeriodYear PeriodYear { get; set; }


        public static string MaxNo
        {
            get
            {
                using (var unit = new FamilyContext())
                {
                    var max = unit.Loans.ToList().Max(lon => lon.LoanNo);
                    return max;
                }
            }
        }
    }
}