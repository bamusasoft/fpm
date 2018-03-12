using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FlopManager.Domain
{
    public enum YearStatus : byte
    {
        Past = 0,
        Present =1,
        Future = 2,
    }
    public class PeriodYear
    {
        public PeriodYear()
        {
            Loans = new HashSet<Loan>();
            PaymentInstructions = new HashSet<PaymentInstruction>();
            Payments = new HashSet<Payment>();
            PaymentSequences = new HashSet<PaymentSequence>();
        }
        [Key]
        [StringLength(4)]
        public string Year { get; set; }

        public YearStatus Status { get; set; }

        public virtual ICollection<Loan> Loans { get; set; }

        public virtual ICollection<PaymentInstruction> PaymentInstructions { get; set; }

        public virtual ICollection<Payment> Payments { get; set; }

        public virtual ICollection<PaymentSequence> PaymentSequences { get; set; }

        //Important override these for WPF ComboBox to work correctly.
        public override int GetHashCode()
        {
            int hash = 13;
            hash = hash * 31 + Year.GetHashCode();
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            PeriodYear py = (PeriodYear)obj;
            return Year.Equals(py.Year);
        }

    }
}