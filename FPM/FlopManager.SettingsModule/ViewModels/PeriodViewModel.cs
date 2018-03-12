using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Data.Entity;
using System.Linq;
using System.Windows.Input;
using FlopManager.Domain;
using FlopManager.Domain.EF;
using FlopManager.Services;
using FlopManager.Services.Helpers;
using FlopManager.Services.ViewModelInfrastructure;
using Prism.Commands;
using Prism.Regions;

namespace FlopManager.SettingsModule.ViewModels
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class PeriodViewModel : EditableViewModelBase,IEntityMapper<PeriodYear>
    {
        public PeriodViewModel()
        {
            Initialize();
        }

        #region "Fields"
        private const string PAYMENT_WORD = "الدفعة ";
        private DbSet<PeriodYear> _repository;
        private FamilyContext _unitOfWork;
        private DelegateCommand _addSequenceCommand;
        private KeyValuePair<YearStatus, string> _selectedStatus;
        private int _sequenceNo;
        private ObservableCollection<PaymentSequence> _yearSequences;
        private ObservableCollection<PeriodYear> _years;
        private bool ChangedFlage { get; set; }
        private Dictionary<YearStatus, string> _yearStatuses;
        private string _year;
        private PeriodYear _selectedYear;
        private bool _editEnabled;

        #endregion

        #region "Properties"

        public string Year
        {
            get { return _year; }
            set { SetProperty(ref _year, value); }
        }

        public ObservableCollection<PaymentSequence> YearSequences
        {
            get { return _yearSequences; }
            private set { SetProperty(ref _yearSequences, value); }
        }

        public ObservableCollection<PeriodYear> Years
        {
            get { return _years; }
            private set { SetProperty(ref _years, value); }
        }

        public PeriodYear SelectedYear
        {
            get { return _selectedYear; }
            set
            {
                SetProperty(ref _selectedYear, value);
                MapFrom(SelectedYear);
            }
        }

        

        public KeyValuePair<YearStatus, string> SelectedStatus
        {
            get { return _selectedStatus; }
            set
            {
                SetProperty(ref _selectedStatus, value);
                OnStateChanged(ViewModelState.InEdit);
            }
        }

        public Dictionary<YearStatus, string> YearStatuses
        {
            get { return _yearStatuses; }
            private set { SetProperty(ref _yearStatuses, value); }
        }

        public bool EditEnabled
        {
            get { return _editEnabled; }
            set { SetProperty(ref _editEnabled, value); }
        }
        #endregion

        #region "Helpers"

        public Dictionary<int, string> PaymentSeqMap = new Dictionary<int, string>
        {
            {1, "الأولى"},
            {2, "الثانية"},
            {3, "الثالثة"},
            {4, "الرابعة"},
            {5, "الخامسة"},
            {6, "السادسة"},
            {7, "السابعة"},
            {8, "الثامنة"},
            {9, "التاسعة"},
            {10, "العاشرة"},
            {11, "الحادية عشرة"},
            {12, "الثانية عشرة"}
        };


        protected override void Initialize()
        {
            Title = ViewModelsTitles.PERIOD;
            CanClose = true;
            Errors = new Dictionary<string, List<string>>();
            _unitOfWork = new FamilyContext();
            _repository = _unitOfWork.PeriodYears;
            YearSequences = new ObservableCollection<PaymentSequence>();
            YearStatuses = CreateYearStatuses();
            Years = new ObservableCollection<PeriodYear>(LoadYears());
            _sequenceNo = 0;
            OnStateChanged(ViewModelState.AddNew);
        }

        private static Dictionary<YearStatus, string> CreateYearStatuses()
        {
            return new Dictionary<YearStatus, string>
            {
                {YearStatus.Past, "مغلقة"},
                {YearStatus.Present, "حالية"},
                {YearStatus.Future, "قادمة"}
            };
        }

        public IList<PeriodYear> LoadYears()
        {
            return _repository.ToList();
        }

        
        private void CreatePaymentSeqeunces(PeriodYear year)
        {
            foreach (var paymentSequence in YearSequences)
            {
                var sequence = CreatNewSequence(year, paymentSequence);
                AddToPaymentSequences(sequence);
            }
        }

        private static PaymentSequence CreatNewSequence(PeriodYear year, PaymentSequence paymentSequence)
        {
            return new PaymentSequence
            {
                SequenceNo = paymentSequence.SequenceNo,
                SequenceDescription = paymentSequence.SequenceDescription,
                Year = year.Year
            };
        }

        private void AddToPaymentSequences(PaymentSequence sequence)
        {
            _unitOfWork.PaymentSequences.Add(sequence);
        }

        private void UpdatePaymentSequences(PeriodYear year)
        {
            foreach (var paymentSequence in YearSequences)
            {
                var exist = _unitOfWork.PaymentSequences.Any(seq => seq.Year == year.Year
                                                                    &&
                                                                    seq.SequenceNo == paymentSequence.SequenceNo);
                if (!exist)
                {
                    var sequence = CreatNewSequence(year, paymentSequence);
                    AddToPaymentSequences(sequence);
                }
            }
        }
        
        private void SyncPaymentSequences()
        {
            var storeSeque = _unitOfWork.PaymentSequences.Where(seq => seq.Year == Year);
            YearSequences = new ObservableCollection<PaymentSequence>(storeSeque);
        }
        private int GenerateSequenceNo()
        {
            return ++_sequenceNo;
        }

        #endregion

        #region "Commands"

        public ICommand AddSeqeunceCommand
        {
            get
            {
                if (_addSequenceCommand == null)
                {
                    _addSequenceCommand = new DelegateCommand(AddSequence);
                }
                return _addSequenceCommand;
            }
        }


        private void AddSequence()
        {
            var s = new PaymentSequence();
            var tempSeq = GenerateSequenceNo();
            if (tempSeq > 12)
            {
                var msg = ""; //Properties.Resources.PaymentSeqView_ExceedingSeqNo;
                Helper.ShowMessage(msg);
                return;
            }

            s.SequenceNo = tempSeq;
            s.SequenceDescription = PAYMENT_WORD + PaymentSeqMap[s.SequenceNo];
            YearSequences.Add(s);
            OnPropertyChanged(nameof(YearSequences));
        }

       
        #endregion

        #region Base
        protected override void Save()
        {
            if (IsValid())
            {
                try
                {
                    var year = _repository.Find(Year);
                    if (year == null)
                    {
                        year = new PeriodYear();
                        MapTo(year, false);
                        CreatePaymentSeqeunces(year);
                        _repository.Add(year);
                    }
                    else
                    {
                        MapTo(year, true);
                        UpdatePaymentSequences(year);
                    }
                    _unitOfWork.SaveChanges();
                    OnStateChanged(ViewModelState.Saved);
                }
                catch (Exception ex)
                {
                    Helper.LogAndShow(ex);
                }
            }
        }

        

        protected override void Delete()
        {
            throw new NotImplementedException();
        }

        

        protected override void Print()
        {
            throw new NotImplementedException();
        }

        

        protected override void Search(object criteria)
        {
            throw new NotImplementedException();
        }

        

       

        protected override void AddNew()
        {
        }

       

       

        public override bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public override void OnStateChanged(ViewModelState state)
        {
            switch (state)
            {
                case ViewModelState.AddNew:
                    _sequenceNo = 0;
                    SelectedStatus = YearStatuses.FirstOrDefault(x => x.Key == YearStatus.Present);
                    EditEnabled = true;
                    break;
                case ViewModelState.InEdit:
                    EditEnabled = true;
                    break;
                case ViewModelState.Saved:
                    SyncPaymentSequences();
                    EditEnabled = (SelectedStatus.Key != YearStatus.Past);
                    break;
                case ViewModelState.Deleted:
                    break;
            }
        }

        protected override bool IsValid()
        {
            var isValid = true;
            if (!Year.IsDigit())
            {
                AddError(nameof(Year), ValidationErrorsMessages.STRING_IS_NOT_DIGIT_ERROR);
                isValid = false;
            }
            else
            {
                RemoveError(nameof(Year), ValidationErrorsMessages.STRING_IS_NOT_DIGIT_ERROR);
            }
            if (SelectedStatus.Key == YearStatus.Present)
            {
                var presentYear = _repository.SingleOrDefault(x => x.Status == YearStatus.Present);
                if (presentYear != null)
                {
                    AddError(nameof(YearStatuses), ValidationErrorsMessages.CAN_NOT_HAVE_MORE_THAN_ONE_PRESENT_YEAR);
                    isValid = false;
                }
                else
                {
                    RemoveError(nameof(YearStatuses), ValidationErrorsMessages.CAN_NOT_HAVE_MORE_THAN_ONE_PRESENT_YEAR);
                }
            }
            if (YearSequences.Count < 1)
            {
                AddError(nameof(YearSequences), ValidationErrorsMessages.YEAR_MUST_HAVE_AT_LEAST_ONE_SEQUENCE);
                isValid = false;
            }
            else
            {
                RemoveError(nameof(YearSequences), ValidationErrorsMessages.YEAR_MUST_HAVE_AT_LEAST_ONE_SEQUENCE);
            }

            return isValid;
        }

        #endregion
        #region IEntityMapper
        public void MapTo(PeriodYear entity, bool ignoreKey)
        {
            if (!ignoreKey) entity.Year = Year;
            entity.Status = SelectedStatus.Key;
        }

        public void MapFrom(PeriodYear entity)
        {
             Year = entity.Year;
            SelectedStatus = YearStatuses.SingleOrDefault(x => x.Key == entity.Status);
            YearSequences = new ObservableCollection<PaymentSequence>(entity.PaymentSequences);
            OnStateChanged(ViewModelState.Saved);
        }

        
        #endregion

    }
}