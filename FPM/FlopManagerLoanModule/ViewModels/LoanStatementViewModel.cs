using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Data.Entity;
using System.Linq;
using System.Windows.Input;
using FlopManager.Domain;
using FlopManager.Domain.EF;
using FlopManager.Services.Helpers;
using FlopManager.Services.ViewModelInfrastructure;
using FlopManagerLoanModule.DTOs;
using Prism.Commands;
using Prism.Regions;
using WordMail = FlopManagerLoanModule.ModuleServices.WordMail;

namespace FlopManagerLoanModule.ViewModels
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class LoanStatementViewModel:ListViewModelBase
    {
        #region "Fields"

        private FamilyContext _unitOfWork;
        private DbSet<Loan> _repLoans;
        private LoansStatementReport _statement;
        
        private ObservableCollection<PeriodYear> _years;
        private PeriodYear _selectedYear;

        #endregion

        public LoanStatementViewModel()
        {
            _unitOfWork = new FamilyContext();
            _repLoans = _unitOfWork.Loans;
            WindowLoaded();
        }

        private void WindowLoaded()
        {
            Years = LoadYears();
        }

        private ObservableCollection<PeriodYear> LoadYears()
        {
            var y = _unitOfWork.PeriodYears.ToList();
            return new ObservableCollection<PeriodYear>(y);
        }

        #region "Proeprties"

        public ObservableCollection<PeriodYear> Years
        {
            get { return _years; }
            set { SetProperty(ref _years, value); }

        }

        public PeriodYear SelectedYear
        {
            get { return _selectedYear; }
            set
            {
                SetProperty(ref _selectedYear, value);
            }
        }

        public LoansStatementReport Statement
        {
            get { return _statement; }
            set
            {
                SetProperty(ref _statement, value);
            }
        }

        #endregion

        #region "Commands"

        

       

        #endregion

        #region "Commands Methods"
        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            
        }

        public override bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            
        }

        protected override void Search(object term)
        {
            string s = (string) term;
            if (string.IsNullOrEmpty(s) || SelectedYear == null) return;

            int code;
            if (int.TryParse(s, out code))
            {
                var result = _repLoans.Where(
                    l => l.FamilyMember.Code == code
                         &&
                         l.PeriodYear.Year == SelectedYear.Year
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

        protected override bool CanSearch(object term)
        {
            return true;
        }

        protected override void Print()
        {
            string fileName = ""; //Properties.Settings.Default.LoansStatementTemplatePath;
            string pdf = ""; //Properties.Settings.Default.PdfsFolder;
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

        protected override bool CanPrint()
        {
            return Statement != null && Statement.DetailsTable != null && Statement.DetailsTable.Rows.Count > 0;
        }

        #endregion


    }
}
