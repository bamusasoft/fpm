using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Fbm.ViewsModel.Helpers
{
    public class MemberLoan: INotifyPropertyChanged
    {
        readonly Guid _id;
        string _loanNo;
        decimal _loanAmount;
        string _description;
        string _remarks;

        public Guid Id
        {
            get { return _id; }
            
        }
        public string LoanNo
        {
            get { return _loanNo; }
            set
            {
                _loanNo = value;
                RaisePropertyChanged();
            }
        }
        public decimal LoanAmount
        {
            get { return _loanAmount; }
            set
            {
                _loanAmount = value;
                RaisePropertyChanged();
            }
        }
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                RaisePropertyChanged();

            }
        }
        public string Remarks
        {
            get { return _remarks; }
            set
            {
                _remarks = value;
                RaisePropertyChanged();
            }
        }
        public MemberLoan(Guid id, string loanNo, decimal loanAmount, string description, string remarks)
        {
            _id = id;
            _loanNo = loanNo;
            _loanAmount = loanAmount;
            _description = description;
            _remarks = remarks;
        }
        public MemberLoan() : this(Guid.Empty,string.Empty, 0.00M, string.Empty, string.Empty) { }

        #region "INotifyPropertyChanged"

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
