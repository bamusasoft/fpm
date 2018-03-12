using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using FlopManager.Domain;
using FlopManager.Domain.EF;
using FlopManager.Services.Helpers;
using FlopManager.Services.ViewModelInfrastructure;
using Prism.Regions;

namespace FlopManager.PaymentsModule.ViewModels
{
    public class PaymentSeqViewModel:EditableViewModelBase
    {

        #region "Fields"

        private const string PaymentWord = "الدفعة ";
        private readonly DbSet<PaymentSequence> _repository;
        private readonly FamilyContext _unitOfWork;
        private PaymentSequence _currnetModel;
        private ModelState _modelState;
        private string _sequenceDescription;
        private int _sequenceNo;
        private bool ChangedFlage { get; set; }
        //

        #endregion

        public PaymentSeqViewModel()
        {
            WindowLoad();
            _unitOfWork = new FamilyContext();
            _repository = _unitOfWork.PaymentSequences;
        }

       
        #region "Events"

        private void WindowLoad()
        {
            SetState(ModelState.New);
        }

        #endregion

        #region "Proeprties"

        public int SequenceNo
        {
            get { return _sequenceNo; }
            set { SetProperty(ref _sequenceNo, value); }
        }

        public string SequenceDescription
        {
            get { return _sequenceDescription; }
            set
            {
                SetProperty(ref _sequenceDescription, value);
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

        protected override void Initialize()
        {
            _currnetModel = new PaymentSequence();
            int tempSeq = GenerateSequenceNo();
            if (tempSeq > 12)
            {
                string msg = "";//Properties.Resources.PaymentSeqView_ExceedingSeqNo;
                //Helper.ShowMessage(msg);
                return;
            }
            SequenceNo = tempSeq;
            SequenceDescription = PaymentWord + PaymentSeqMap[SequenceNo];
        }


        

     

        #endregion

        #region "Commands Methods"

        protected override void Save()
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
                _unitOfWork.SaveChanges();
                SetState(ModelState.Saved);
            }
            catch (Exception)
            {
                throw;
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
            if (!OkClose())
            {
                string msg = ""; //Properties.Resources.PromptForSaveMsg;
                if (!Helper.UserConfirmed(msg)) return;
            }
            SetState(ModelState.New);
        }

        

        

        public override bool IsNavigationTarget(NavigationContext navigationContext)
        {
            throw new NotImplementedException();
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            throw new NotImplementedException();
        }

        public override void OnStateChanged(ViewModelState state)
        {
            throw new NotImplementedException();
        }

        protected override bool IsValid()
        {
            throw new NotImplementedException();
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
            IList<PaymentSequence> all = _repository.ToList();
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