using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using FlopManager.Services;

namespace FlopManager.Domain.EF
{
    public class FamilyContext : DbContext
    {
        public FamilyContext():base(ConnectionString.FamilyConnection)
        {
        }

        public virtual DbSet<FamilyMember> FamilyMembers { get; set; }
        public virtual DbSet<LoanPayment> LoanPayments { get; set; }
        public virtual DbSet<Loan> Loans { get; set; }
        public virtual DbSet<LoanType> LoanTypes { get; set; }
        public virtual DbSet<PaymentInstruction> PaymentInstructions { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<PaymentSequence> PaymentSequences { get; set; }
        public virtual DbSet<PaymentTransaction> PaymentTransactions { get; set; }
        public virtual DbSet<PeriodYear> PeriodYears { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FamilyMember>()
                .Property(e => e.IndependentDate)
                .IsFixedLength();

            modelBuilder.Entity<FamilyMember>()
                .HasMany(e => e.Loans)
                .WithRequired(e => e.FamilyMember)
                .HasForeignKey(e => e.MemberCode)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<FamilyMember>()
                .HasMany(e => e.PaymentTransactions)
                .WithRequired(e => e.FamilyMember)
                .HasForeignKey(e => e.MemberCode)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<LoanPayment>()
                .Property(e => e.DocDate)
                .IsFixedLength();

            modelBuilder.Entity<LoanPayment>()
                .Property(e => e.LoanNo)
                .IsFixedLength();

            modelBuilder.Entity<LoanPayment>()
                .Property(e => e.AmountPaid)
                .HasPrecision(19, 4);

            modelBuilder.Entity<Loan>()
                .Property(e => e.LoanNo)
                .IsFixedLength();

            modelBuilder.Entity<Loan>()
                .Property(e => e.Year)
                .IsFixedLength();

            modelBuilder.Entity<Loan>()
                .Property(e => e.Amount)
                .HasPrecision(19, 4);

            modelBuilder.Entity<Loan>()
                .HasMany(e => e.LoanPayments)
                .WithRequired(e => e.Loan)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<LoanType>()
                .HasMany(e => e.Loans)
                .WithRequired(e => e.LoanType)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<LoanType>()
                .HasMany(e => e.PaymentInstructions)
                .WithRequired(e => e.LoanType)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<PaymentInstruction>()
                .Property(e => e.PaymentNo)
                .IsFixedLength();

            modelBuilder.Entity<PaymentInstruction>()
                .Property(e => e.Year)
                .IsFixedLength();

            modelBuilder.Entity<Payment>()
                .Property(e => e.PaymentNo)
                .IsFixedLength();

            modelBuilder.Entity<Payment>()
                .Property(e => e.PaymentDate)
                .IsFixedLength();

            modelBuilder.Entity<Payment>()
                .Property(e => e.Year)
                .IsFixedLength();

            modelBuilder.Entity<Payment>()
                .Property(e => e.Amount)
                .HasPrecision(19, 4);

            modelBuilder.Entity<Payment>()
                .HasMany(e => e.PaymentInstructions)
                .WithRequired(e => e.Payment)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Payment>()
                .HasMany(e => e.PaymentTransactions)
                .WithRequired(e => e.Payment)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<PaymentSequence>()
                .Property(e => e.Year)
                .IsFixedLength();

            modelBuilder.Entity<PaymentSequence>()
                .HasMany(e => e.Loans)
                .WithRequired(e => e.PaymentSequence)
                .HasForeignKey(e => e.PaySeqDue)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<PaymentSequence>()
                .HasMany(e => e.Payments)
                .WithRequired(e => e.PaymentSequence)
                .HasForeignKey(e => e.PaySequence)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<PaymentTransaction>()
                .Property(e => e.TransDate)
                .IsFixedLength();

            modelBuilder.Entity<PaymentTransaction>()
                .Property(e => e.PaymentNo)
                .IsFixedLength();

            modelBuilder.Entity<PaymentTransaction>()
                .Property(e => e.AmountDue)
                .HasPrecision(19, 4);

            modelBuilder.Entity<PaymentTransaction>()
                .Property(e => e.NetPayments)
                .HasPrecision(19, 4);

            modelBuilder.Entity<PaymentTransaction>()
                .HasMany(e => e.LoanPayments)
                .WithRequired(e => e.PaymentTransaction)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<PeriodYear>()
                .Property(e => e.Year)
                .IsFixedLength();

            modelBuilder.Entity<PeriodYear>()
                .HasMany(e => e.Loans)
                .WithRequired(e => e.PeriodYear)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<PeriodYear>()
                .HasMany(e => e.PaymentInstructions)
                .WithRequired(e => e.PeriodYear)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<PeriodYear>()
                .HasMany(e => e.Payments)
                .WithRequired(e => e.PeriodYear)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<PeriodYear>()
                .HasMany(e => e.PaymentSequences)
                .WithRequired(e => e.PeriodYear)
                .WillCascadeOnDelete(false);
        }


    }
}
