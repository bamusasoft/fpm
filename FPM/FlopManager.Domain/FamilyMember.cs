using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlopManager.Domain
{
    [Flags]
    public enum Sex
    {
        Male = 1,
        Female = 2
    }

    public enum PayMethod
    {
        Check = 1,
        BankTransfer = 2,
    }
    public class FamilyMember
    {
        public FamilyMember()
        {
            Loans = new HashSet<Loan>();
            PaymentTransactions = new HashSet<PaymentTransaction>();
        }
        public virtual ICollection<Loan> Loans { get; set; }
        public virtual ICollection<PaymentTransaction> PaymentTransactions { get; set; }

        #region Table Properties

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Code { get; set; }

        public int Parent { get; set; }

        [Required]
        [StringLength(15)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        public int MotherCode { get; set; }

        public byte Independent { get; set; }

        [StringLength(10)]
        public string IndependentDate { get; set; }

        public byte Alive { get; set; }

        public byte HasChildren { get; set; }

        public int Shares { get; set; }

        public int XShares { get; set; }

        public int Buffer { get; set; }

        public byte ShareHolderLevel { get; set; }

        public Sex Sex { get; set; }
        public PayMethod PayMethod { get; set; }

        #endregion
    }
}