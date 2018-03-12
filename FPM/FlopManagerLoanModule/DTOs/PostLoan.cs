using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FlopManagerLoanModule.DTOs
{
    public class PostLoan: INotifyPropertyChanged
    {
        bool _selected;
        int _memberCode;
        string _memberName;
        int _loanTyopeId;
        string _loanTypeDescription;
        string _periodId;
        string _periodYear;
        int _paymentSequenceId;
        string _paymentSequenceDescription;
        decimal _loanAmount;
        public bool Selected
        {
            get { return _selected; }
            set
            {
                _selected = value;
                RaisePropertyChanged();
            }
        }
        public int MemberCode
        {
            get { return _memberCode; }
            set
            {
                _memberCode = value;
                RaisePropertyChanged();
            }
        }
        public string MemberName
        {
            get { return _memberName; }
            set
            {
                _memberName = value;
                RaisePropertyChanged();
            }
        }
        public int LoanTypeId
        {
            get { return _loanTyopeId; }
            set
            {
                _loanTyopeId = value;
                RaisePropertyChanged();
            }
        }
        public string LoanTypeDescription
        {
            get { return _loanTypeDescription; }
            set
            {
                _loanTypeDescription = value;
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
        public string PeriodId
        {
            get { return _periodId; }
            set
            {
                _periodId = value;
                RaisePropertyChanged();
            }
        }

        public int PaymentSequenceId
        {
            get { return _paymentSequenceId; }
            set
            {
                _paymentSequenceId = value;
                RaisePropertyChanged();
            }
        }
        public string PaymentSequenceDescription
        {
            get { return _paymentSequenceDescription; }
            set
            {
                _paymentSequenceDescription = value;
                RaisePropertyChanged();
            }
        }
        public string PeriodYear
        {
            get { return _periodYear; }
            set
            {
                _periodYear = value;
                RaisePropertyChanged();
            }
        }

        public PostLoan(bool selected, int memberCode, string memberName, int loanTypeId, string loanTypeDescription,string periodId, 
                        string periodYear,int paySequenceId, string paySequenceDescriptin,   decimal loanAmount)
        {
            _selected = selected;
            _memberCode = memberCode;
            _memberName = memberName;
            _loanTyopeId = loanTypeId;
            _loanTypeDescription = loanTypeDescription;
            _periodId = periodId;
            _periodYear = periodYear;
            _paymentSequenceId = paySequenceId;
            _paymentSequenceDescription = paySequenceDescriptin;
            _loanAmount = loanAmount;
        }



        #region "INotifyPropertyChanged Members"
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
