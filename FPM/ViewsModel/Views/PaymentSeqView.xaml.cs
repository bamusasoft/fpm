using System;
using System.Collections.Generic;
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
    /// Interaction logic for PaymentSeqView.xaml
    /// </summary>
    public partial class PaymentSeqView : Window, IModelOperations<PaymentSequence>, ICommonOperations
    {
        public PaymentSeqView()
        {
            InitializeComponent();
            Loaded += WindowLoad;
            DataContext = this;
            _unitOfWork = new UnitOfWork();
            _repository = _unitOfWork.PaymentSequences;
        }

        #region "Fields"

        private const string PaymentWord = "الدفعة ";
        private readonly RepositoryBase<PaymentSequence> _repository;
        private readonly IUnitOfWork _unitOfWork;
        private RelayCommand _addNewCommand;
        private PaymentSequence _currnetModel;
        private ModelState _modelState;
        //
        private RelayCommand _saveCommand;
        private RelayCommand _searchCommand;
        private string _sequenceDescription;
        private int _sequenceNo;
        private bool ChangedFlage { get; set; }
        //

        #endregion

        #region "Events"

        private void WindowLoad(object sender, RoutedEventArgs e)
        {
            SetState(ModelState.New);
        }

        #endregion

        #region "Proeprties"

        public int SequenceNo
        {
            get { return _sequenceNo; }
            set
            {
                _sequenceNo = value;
                RaisePropertyChanged();
            }
        }

        public string SequenceDescription
        {
            get { return _sequenceDescription; }
            set
            {
                _sequenceDescription = value;
                RaisePropertyChanged();
                SetChangedFlag();
            }
        }

        #endregion

        #region "IModelOperations"

        public void ReadModelValues(PaymentSequence model)
        {
            if (model == null) throw new ArgumentNullException("model");
            SequenceNo = model.SequenceNo;
            SequenceDescription = model.SequenceDescription;
        }

        public void WriteModelValues(PaymentSequence model)
        {
            if (model == null) throw new ArgumentNullException("model");
            model.SequenceNo = SequenceNo;
            model.SequenceDescription = SequenceDescription;
        }

        public bool ValidData()
        {
            return true;
        }

        #endregion

        #region "ICommonOperations"

        public void SetState(ModelState state)
        {
            _modelState = state;
            switch (_modelState)
            {
                case ModelState.New:
                    Initialize();
                    ResetChangedFalg();
                    break;
                case ModelState.Saved:
                    ResetChangedFalg();
                    break;
                case ModelState.Deleted:
                    break;
                default:
                    break;
            }
        }

        public void SetChangedFlag()
        {
            ChangedFlage = true;
        }

        public void ResetChangedFalg()
        {
            ChangedFlage = false;
        }

        public bool OkClose()
        {
            return !ChangedFlage;
        }

        public List<RuleViolation> RulesViolations
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public void Initialize()
        {
            _currnetModel = new PaymentSequence();
            int tempSeq = GenerateSequenceNo();
            if (tempSeq > 12)
            {
                string msg = Properties.Resources.PaymentSeqView_ExceedingSeqNo;
                Helper.ShowMessage(msg);
                return;
            }
            SequenceNo = tempSeq;
            SequenceDescription = PaymentWord + PaymentSeqMap[SequenceNo];
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public ICommand SaveCommand
        {
            get
            {
                if (_saveCommand == null)
                {
                    _saveCommand = new RelayCommand(Save);
                }
                return _saveCommand;
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

        public ICommand DeleteCommand
        {
            get { throw new NotImplementedException(); }
        }

        public ICommand PrintCommand
        {
            get { throw new NotImplementedException(); }
        }

        public ICommand EditCommand
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region "Commands Methods"

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
            catch (Exception)
            {
                throw;
            }
        }

        private void AddNew()
        {
            if (!OkClose())
            {
                string msg = Properties.Resources.PromptForSaveMsg;
                if (!Helper.UserConfirmed(msg)) return;
            }
            SetState(ModelState.New);
        }

        private void Search()
        {
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
                                                               {12, "الثانية عشرة"},
                                                           };

        private int GenerateSequenceNo()
        {
            IList<PaymentSequence> all = _repository.GetAll();
            if (all.Count == 0)
            {
                return 1;
            }
            int max = all.Max(x => x.SequenceNo);
            return ++max;
        }

        #endregion
    }
}