using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FlopManager.Domain
{
    public class PaymentSequence
    {
        public PaymentSequence()
        {
            Loans = new HashSet<Loan>();
            Payments = new HashSet<Payment>();
        }

        public int Id { get; set; }

        public int SequenceNo { get; set; }

        [Required]
        [StringLength(50)]
        public string SequenceDescription { get; set; }

        [Required]
        [StringLength(4)]
        public string Year { get; set; }

        public virtual ICollection<Loan> Loans { get; set; }

        public virtual ICollection<Payment> Payments { get; set; }

        public virtual PeriodYear PeriodYear { get; set; }

        public override int GetHashCode()
        {
            int hash = 13;
            hash = hash * 31 + Id.GetHashCode();
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            PaymentSequence ps = (PaymentSequence)obj;
            return Id == ps.Id;
        }
    }
}