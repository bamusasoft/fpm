using System.ComponentModel.DataAnnotations;

namespace FlopManager.Domain
{
    public class PaymentInstruction
    {
        [Key]
        public int AutoKey { get; set; }

        [Required]
        [StringLength(6)]
        public string PaymentNo { get; set; }

        public int LoanTypeCode { get; set; }

        public short EarnPercent { get; set; }

        [Required]
        [StringLength(4)]
        public string Year { get; set; }

        public bool OldLoan { get; set; }

        public virtual LoanType LoanType { get; set; }

        public virtual Payment Payment { get; set; }

        public virtual PeriodYear PeriodYear { get; set; }
    }
}