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
        public string DateCreated { get; set; }
        public string Description { get; set; }
        public string Year { get; set; }
        public string PaySequence { get; set; }
        public decimal Amount { get; set; }
        public decimal Paid { get; set; }
        public decimal Balance { get; set; }
    }
}
