using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FlopManager.Services.DTOs
{
    public class MemberLoan: INotifyPropertyChanged
    {
        string _loanNo;
        decimal _loanAmount;
        string _description;
        string _remarks;

        public int DocNo { get; }
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
        public MemberLoan(int id, string loanNo, decimal loanAmount, string description, string remarks)
        {
            DocNo = id;
            _loanNo = loanNo;
            _loanAmount = loanAmount;
            _description = description;
            _remarks = remarks;
        }

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
