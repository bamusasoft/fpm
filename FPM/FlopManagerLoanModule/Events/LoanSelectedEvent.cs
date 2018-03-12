using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlopManager.Domain;
using Prism.Events;

namespace FlopManagerLoanModule.Events
{
    public class LoanSelectedEvent:PubSubEvent<Loan>
    {
    }
}
