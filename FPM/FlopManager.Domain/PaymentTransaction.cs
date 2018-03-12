using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlopManager.Domain
{
    public class PaymentTransaction
    {
        public PaymentTransaction()
        {
            LoanPayments = new HashSet<LoanPayment>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int TransNo { get; set; }

        [Required]
        [StringLength(8)]
        public string TransDate { get; set; }

        [Required]
        [StringLength(6)]
        public string PaymentNo { get; set; }

        public int MemberCode { get; set; }

        public int ShareNumbers { get; set; }

        [Column(TypeName = "money")]
        public decimal AmountDue { get; set; }

        [Column(TypeName = "money")]
        public decimal NetPayments { get; set; }

        public PayMethod PayMethod { get; set; }

        public int BankDocNo { get; set; }

        public virtual FamilyMember FamilyMember { get; set; }

        public virtual ICollection<LoanPayment> LoanPayments { get; set; }

        public virtual Payment Payment { get; set; }
    }
}