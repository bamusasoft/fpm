using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Fbm.DomainModel;
using Fbm.DomainModel.Entities;
using Fbm.DomainModel.Repositories;
using Fbm.ViewsModel.Helpers;
using GalaSoft.MvvmLight.Command;

namespace Fbm.ViewsModel.Views
{
    /// <summary>
    /// Interaction logic for LoansStatementView.xaml
    /// </summary>
    public partial class LoansStatementView : Window, INotifyPropertyChanged
    {
        #region "Fields"
        IUnitOfWork _unitOfWork;
        RepositoryBase<Loan> _repLoans;
        LoansStatementReport _statement;
        RelayCommand _searchCommand;
        RelayCommand _printCommand;
        ObservableCollection<PeriodSetting> _years;
        PeriodSetting _selectedYear;
        #endregion
        public LoansStatementView()
        {
            InitializeComponent();

            _unitOfWork = new UnitOfWork();
            _repLoans = _unitOfWork.Loans;
            Loaded += WindowLoaded;
            DataContext = this;
        }

        void WindowLoaded(object sender, RoutedEventArgs e)
        {
            Years = LoadYears();
        }

        private ObservableCollection<PeriodSetting> LoadYears()
        {
            var y = _unitOfWork.PeriodSettings.GetAll();
            return new ObservableCollection<PeriodSetting>(y);
        }

        #region "Proeprties"
        public ObservableCollection<PeriodSetting> Years
        {
            get { return _years;}
            set
            {
                _years = value;
                RaisePropertyChanged();
            }
            
        }
        public PeriodSetting SelectedYear
        {
            get { return _selectedYear; }
            set
            {
                _selectedYear = value;
                RaisePropertyChanged();
            }
        }
        public LoansStatementReport Statement
        {
            get { return _statement; }
            set
            {
                _statement = value;
                RaisePropertyChanged();
            }
        }
        #endregion
        #region "Commands"
        public ICommand SearchCommand
        {
            get
            {
                if (_searchCommand == null)
                {
                    _searchCommand = new RelayCommand(Search);

                }
                return _searchCommand;

                }
        }
        public ICommand PrintCommand
        {
            get
            {
                if (_printCommand == null)
                {
                    _printCommand = new RelayCommand(Print, CanPrint);

                }
                return _printCommand;

            }
        }
        #endregion
        #region "Commands Methods"
        void Search()
        {
            string s = txtMemberCode.Text;
            if (string.IsNullOrEmpty(s) || SelectedYear == null) return;

            int code;
            if (int.TryParse(s, out code))
            {
                var result = _repLoans.Query(
                                    l => l.FamilyMember.Code == code
                                    &&
                                    l.PeriodSetting.Id == SelectedYear.Id
                                    ).OrderBy(l => l.LoanNo);
                int memberCode = 0;
                string name = null;
                decimal loansTotal = 0.0M;
                decimal paidTotal = 0.0M;
                decimal balanceTotal = 0.0M;
                List<LoansStatementDetailes> details = new List<LoansStatementDetailes>();
                int counter = 0;
                foreach (var loan in result)
                {
                    if (counter == 0)
                    {
                        memberCode = loan.FamilyMember.Code;
                        name = loan.FamilyMember.FullName;
                        loansTotal = result.Sum(x => x.Amount);
                        paidTotal = result.Sum(x => x.Paid);
                        balanceTotal = result.Sum(x => x.Balance);
                        counter++;
                    }
                    LoansStatementDetailes d = new LoansStatementDetailes(
                        "1433-01-01", loan.Description,
                        loan.Amount, loan.Paid, loan.Balance);
                    details.Add(d);

                }
                Statement = new LoansStatementReport(
                    memberCode, name, loansTotal, paidTotal, balanceTotal, details);
            }
        }   
        void Print()
        {
            string fileName = Properties.Settings.Default.LoansStatementTemplatePath;
            string pdf = Properties.Settings.Default.PdfsFolder;
            WordMail wm = new WordMail();
            try
            {
                wm.Send(Statement, fileName, pdf);
            }
            catch (Exception ex)
            {
                
                Helper.LogAndShow(ex);
            }
            

        }
        bool CanPrint()
        {
            return Statement != null && Statement.DetailsTable != null && Statement.DetailsTable.Rows.Count > 0;
        }
        #endregion

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
