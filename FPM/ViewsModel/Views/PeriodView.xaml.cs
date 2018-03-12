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
    /// Interaction logic for PeriodView.xaml
    /// </summary>
    public partial class PeriodView : INotifyPropertyChanged
    {
        public PeriodView()
        {
            InitializeComponent();
            DataContext = this;
            Loaded += WindowLoaded;
            _unitOfWork = new UnitOfWork();
            _repository = _unitOfWork.PeriodSettings;
            _statusesRepository = _unitOfWork.PeriodStatuses;
        }

        #region "Fields"

        private const string PaymentWord = "الدفعة ";
        private readonly RepositoryBase<PeriodSetting> _repository;
        private readonly RepositoryBase<PeriodStatus> _statusesRepository;
        private readonly IUnitOfWork _unitOfWork;
        private RelayCommand _addNewCommand;
        private RelayCommand _addSequenceCommand;
        private bool _canSave;
        private bool _canSearch;
        private string _closedStatus;
        private PeriodSetting _currnetModel;
        private bool _enableUi;
        private string _endDate;
        private ModelState _modelState;
        private ObservableCollection<PeriodStatus> _periodStatuses;
        private RelayCommand _saveCommand;
        private RelayCommand _searchCommand;
        private string _searchField;
        private PeriodStatus _selectedStatus;
        private int _sequenceNo;
        private string _startDate;
        private string _yearPart;
        private ObservableCollection<PaymentSequence> _yearSequences;
        private bool ChangedFlage { get; set; }
        private List<RuleViolation> RulesViolations { get; set; }

        #endregion

        #region "Events"

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            IEnumerable<PeriodStatus> ps = _statusesRepository.GetAll().Where(x => x.Id != 1);
            PeriodStatuses = new ObservableCollection<PeriodStatus>(ps);
            Initialize();
        }

        #endregion

        #region "Properties"

        public ObservableCollection<PaymentSequence> YearSequences
        {
            get { return _yearSequences; }
            set
            {
                _yearSequences = value;
                RaisePropertyChanged();
            }
        }

        public PeriodSetting CurrentPeriod
        {
            get
            {
                PeriodSetting p = null;
                try
                {
                    p = _repository.Query(x => x.PeriodStatus.Id == 2).Single();
                }
                catch (Exception ex)
                {
                    string msg = Helper.ProcessExceptionMessages(ex);
                    Logger.Log(LogMessageTypes.Error, msg, ex.TargetSite.Name, ex.StackTrace);
                }
                return p;
            }
        }

        public string ClosedStatus
        {
            get { return _closedStatus; }
            set
            {
                _closedStatus = value;
                RaisePropertyChanged();
            }
        }

        public bool EnableUi
        {
            get { return _enableUi; }
            set
            {
                _enableUi = value;
                RaisePropertyChanged();
            }
        }

        public string StartDate
        {
            get { return _startDate; }
            set
            {
                _startDate = value;
                RaisePropertyChanged();
                SetChangedFlage();
            }
        }

        public string EndDate
        {
            get { return _endDate; }
            set
            {
                _endDate = value;
                RaisePropertyChanged();
                SetChangedFlage();
            }
        }

        public string YearPart
        {
            get { return _yearPart; }
            set
            {
                if (value != _yearPart)
                {
                    _yearPart = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string SearchField
        {
            get { return _searchField; }
            set
            {
                _searchField = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<PeriodStatus> PeriodStatuses
        {
            get { return _periodStatuses; }
            set
            {
                _periodStatuses = value;
                RaisePropertyChanged();
            }
        }

        public PeriodStatus SelectedStatus
        {
            get { return _selectedStatus; }
            set
            {
                _selectedStatus = value;
                RaisePropertyChanged();
                SetChangedFlage();
            }
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

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
                                                               {12, "الثانية عشرة"},
                                                           };

        private void SetState(ModelState state)
        {
            _modelState = state;
            switch (_modelState)
            {
                case ModelState.New:
                    txtStartDate.Focus();
                    ResetChangedFlage();
                    EnableUi = true;
                    _canSave = true;
                    _canSearch = true;
                    _sequenceNo = 0;
                    txtSearch.Text = "";
                    break;
                case ModelState.Saved:
                    ResetChangedFlage();
                    _canSearch = false;
                    EnableUi = SelectedStatus == null || SelectedStatus.Id != 1;
                    if (SelectedStatus != null && SelectedStatus.Id == 1) _canSave = false;
                    break;
                case ModelState.Deleted:
                    ResetChangedFlage();
                    _sequenceNo = 0;
                    _canSearch = true;
                    break;
            }
        }

        private void Initialize()
        {
            _currnetModel = new PeriodSetting();
            RulesViolations = new List<RuleViolation>();
            YearSequences = new ObservableCollection<PaymentSequence>();
            ReadModelValues(_currnetModel);
            SetState(ModelState.New);
        }

        private void ReadModelValues(PeriodSetting model)
        {
            if (model == null) throw new ArgumentNullException("model");

            YearPart = model.YearPart;
            StartDate = model.StartDate;
            EndDate = model.EndDate;
            SelectedStatus = model.PeriodStatus;
            _sequenceNo = model.PaymentSequences.Count();
            
            foreach (PaymentSequence seq in model.PaymentSequences)
            {
                YearSequences.Add(seq);
            }
            if (SelectedStatus != null && SelectedStatus.Id == 1)
            {
                ClosedStatus = SelectedStatus.Description;
            }
            else
            {
                ClosedStatus = null;
            }
            RaisePropertyChanged("");
        }

        private void WriteModelValues(PeriodSetting model)
        {
            if (model == null) throw new ArgumentNullException("model");
            model.YearPart = ExtractYearPart();
            //model.Id = model.YearPart;
            model.StartDate = StartDate;
            model.EndDate = EndDate;
            model.PeriodStatus = SelectedStatus;

            //if (_modelState == ModelState.New)
            //{
                foreach (PaymentSequence seq in YearSequences)
                {
                    bool exisitSeq = seq.Id != 0;//==0 means it has not been sent to database.
                    if(!exisitSeq) model.PaymentSequences.Add(seq);
                }
            //}
            //else
            //{
            //    foreach (PaymentSequence seq in YearSequences)
            //    {
            //        bool exisitSeq = model.PaymentSequences.Any(ps => ps.Id == 0);
            //        model.PaymentSequences.Add(seq);
            //    }
               
            //}

        }

        private string ExtractYearPart()
        {
            return StartDate.Substring(0, 4);
        }

        private void SetChangedFlage()
        {
            ChangedFlage = true;
        }

        private void ResetChangedFlage()
        {
            ChangedFlage = false;
        }

        public bool OkExit()
        {
            if (!ChangedFlage) return true;
            string msg = Properties.Resources.PromptForSaveMsg;
            return Helper.UserConfirmed(msg);
        }

        private bool ValidData()
        {
            if (string.IsNullOrEmpty(StartDate) || ! Helper.ValidDate(StartDate))
            {
                RulesViolations.Add(new RuleViolation(Properties.Resources.PeriodView_InvalidStartDate));
                return false;
            }
            if (string.IsNullOrEmpty(EndDate) || ! Helper.ValidDate(EndDate))
            {
                RulesViolations.Add(new RuleViolation(Properties.Resources.PeriodView_InvalidEndDate));
                return false;
            }
            int i = String.CompareOrdinal(StartDate, EndDate);
            if (i >= 0)
            {
                RulesViolations.Add(new RuleViolation(Properties.Resources.PeriodView_InconsistentStartEndDate));
                return false;
            }
            string startYearPart = StartDate.Substring(0, 4);
            string endYearPart = EndDate.Substring(0, 4);
            if (startYearPart != endYearPart)
            {
                RulesViolations.Add(new RuleViolation(Properties.Resources.PeriodView_YearPartNotMatch));
                return false;
            }
            if (_modelState == ModelState.New && _repository.GetAll().Any(x => x.StartDate == StartDate))
            {
                RulesViolations.Add(new RuleViolation(Properties.Resources.PeriodView_AlreadyRegistredYear));
                return false;
            }
            if (SelectedStatus == null)
            {
                RulesViolations.Add(new RuleViolation(Properties.Resources.PeriodView_PeriodStutsMissing));
                return false;
            }
            if (CurrentPeriod != null && _currnetModel.Id != CurrentPeriod.Id &&
                SelectedStatus.Id == 2)
            {
                RulesViolations.Add(new RuleViolation(Properties.Resources.PeriodView_InvalidPeriodStatus));
                return false;
            }
            if (YearSequences.Count == 0)
            {
                RulesViolations.Add(new RuleViolation(Properties.Resources.PeriodView_YearSeqMissing));
                return false;
            }


            return true;
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
                    _addSequenceCommand = new RelayCommand(AddSequence, CanAddSequence);
                }
                return _addSequenceCommand;
            }
        }

        public ICommand SaveCommand
        {
            get
            {
                if (_saveCommand == null)
                {
                    _saveCommand = new RelayCommand(Save, CanSave);
                }
                return _saveCommand;
            }
        }

        public ICommand SearchCommand
        {
            get
            {
                if (_searchCommand == null)
                {
                    _searchCommand = new RelayCommand(Search, CanSearch);
                }
                return _searchCommand;
            }
        }

        public ICommand AddNewCommand
        {
            get
            {
                if (_addNewCommand == null)
                {
                    _addNewCommand = new RelayCommand(AddNew);
                }
                return _addNewCommand;
            }
        }

        private void AddSequence()
        {
            var s = new PaymentSequence();
            int tempSeq = GenerateSequenceNo();
            if (tempSeq > 12)
            {
                string msg = Properties.Resources.PaymentSeqView_ExceedingSeqNo;
                Helper.ShowMessage(msg);
                return;
            }

            s.SequenceNo = tempSeq;
            s.SequenceDescription = PaymentWord + PaymentSeqMap[s.SequenceNo];
            YearSequences.Add(s);
            RaisePropertyChanged("YearSequences");
        }

        private bool CanAddSequence()
        {
            return _currnetModel != null;
        }

        private void Save()
        {
            if (!ValidData())
            {
                string msg = null;
                foreach (RuleViolation viol in RulesViolations)
                {
                    msg += viol.ErrorMessage;
                }
                RulesViolations.Clear();
                Helper.ShowMessage(msg);
                return;
            }
            try
            {
                WriteModelValues(_currnetModel);
                if (_modelState == ModelState.New)
                {
                    _repository.Add(_currnetModel);
                }
                _unitOfWork.Save();
                SetState(ModelState.Saved);
            }
            catch (Exception ex)
            {
                Helper.LogAndShow(ex);
            }
        }

        private bool CanSave()
        {
            return _canSave;
        }

        private void Search()
        {
            if (string.IsNullOrEmpty(SearchField)) return;
            IQueryable<PeriodSetting> result = _repository.Query(setting => setting.YearPart == SearchField);
            if (result == null || result.Count() != 1) return;
            _currnetModel = result.First();
            ReadModelValues(_currnetModel);
            SetState(ModelState.Saved);
        }

        private bool CanSearch()
        {
            return _canSearch;
        }

        private void AddNew()
        {
            if (!OkExit()) return;
            Initialize();
        }

        #endregion
    }
}