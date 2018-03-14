using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace FlopManager.Domain
{
     
    public class LoanPayment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int DocNo { get; set; }

        [StringLength(8)]
        public string DocDate { get; set; }

        [Required]
        [StringLength(6)]
        public string LoanNo { get; set; }

        [Required]
        [StringLength(6)]
        public string PaymentNo { get; set; }

        public int TransNo { get; set; }

        [Column(TypeName = "money")]

        public decimal AmountPaid { get; set; }
        public virtual Loan Loan { get; set; }

        public virtual PaymentTransaction PaymentTransaction { get; set; }
        public virtual Payment Payment { get; set; }
    }

}
