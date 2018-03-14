using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using FlopManager.Domain.EF;

namespace FlopManager.Domain
{
    public class Payment
    {
        public Payment()
        {
            PaymentInstructions = new HashSet<PaymentInstruction>();
            PaymentTransactions = new HashSet<PaymentTransaction>();
            LoanPayments = new HashSet<LoanPayment>();

        }

        [Key]
        [StringLength(6)]
        public string PaymentNo { get; set; }

        [Required]
        [StringLength(8)]
        public string PaymentDate { get; set; }

        [Required]
        [StringLength(4)]
        public string Year { get; set; }

        public int PaySequence { get; set; }

        [Column(TypeName = "money")]
        public decimal Amount { get; set; }

        public bool Posted { get; set; }

        public virtual ICollection<PaymentInstruction> PaymentInstructions { get; set; }

        public virtual PaymentSequence PaymentSequence { get; set; }


        public virtual PeriodYear PeriodYear { get; set; }

        public virtual ICollection<PaymentTransaction> PaymentTransactions { get; set; }
        public virtual ICollection<LoanPayment> LoanPayments { get; set; }

        public static string MaxNo
        {
            get
            {
                var unit = new FamilyContext();
                var max = unit.Payments.Max(paym => paym.PaymentNo);
                return max;
            }
        }
    }
}