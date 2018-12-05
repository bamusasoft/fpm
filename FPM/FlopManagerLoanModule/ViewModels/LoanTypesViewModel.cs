using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Data.Entity;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using FlopManager.Domain;
using FlopManager.Domain.EF;
using FlopManager.Services;
using FlopManager.Services.Helpers;
using FlopManager.Services.ViewModelInfrastructure;
using FlopManagerLoanModule.Properties;
using Prism.Events;
using Prism.Logging;
using Prism.Regions;

namespace FlopManagerLoanModule.ViewModels
{
    //[Export]
    //[PartCreationPolicy(CreationPolicy.NonShared)]
    public class LoanTypesViewModel:EditableViewModelBase, IEntityMapper<LoanType>
    {
        //[ImportingConstructor]
        public LoanTypesViewModel(ILogger logger)
        {
            _logger = logger;
            Title = ViewModelsTitles.LOAN_TYPES;
            CanClose = true;
            Initialize();
        }

        #region "Fields"
        private readonly ILogger _logger;
        int _code;
        string _description;
        FamilyContext _unitOfWork;
        DbSet<LoanType> _repository;
        private readonly IEventAggregator _eventAggregator;
        private ICollectionView _typesList;
        private LoanType _selectedType;
        #endregion


        #region "Properties"
        public int Code
        {
            get { return _code; }
            set
            {
                SetProperty(ref _code, value);
                OnStateChanged(ViewModelState.InEdit);
            }
        }
        public string Description
        {
            get { return _description; }
            set
            {
                SetProperty(ref _description, value);
                OnStateChanged(ViewModelState.InEdit);
            }
        }
        public ICollectionView TypesList
        {
            get { return _typesList; }
            set
            {
                SetProperty(ref _typesList, value);
            }
        }
        public LoanType SelectedType
        {
            get { return _selectedType; }
            set
            {
                _selectedType = value;
                ChageSelection();
            }
        }
        #endregion

        #region "Helpers"


        protected override void Initialize()
        {
            
            
            _unitOfWork = new FamilyContext();
            _repository = _unitOfWork.LoanTypes;
            Errors = new Dictionary<string, List<string>>();
            LoadTypesList();
        }

        
        #endregion

        #region "Base"

        protected override void Save()
        {
            if (IsValid())
            {
                try
                {
                    LoanType loanType = _repository.Find(Code);
                    if (loanType == null)
                    {
                        loanType = new LoanType();
                        MapTo(loanType, false);
                        _repository.Add(loanType);
                    }
                    else
                    {
                        MapTo(loanType, true);
                    }
                    _unitOfWork.SaveChanges();
                    OnStateChanged(ViewModelState.Saved);
                }
                catch (Exception ex)
                {
                    var exception = Helper.ProcessExceptionMessages(ex);
                    _logger.Log(exception.DetialsMsg, Category.Exception, Priority.High);
                    RaiseNotification(exception.UserMsg);
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

       
       protected  override void AddNew()
        {
            if (CanExit())
            {
                ClearView();
                OnStateChanged(ViewModelState.AddNew);
            }

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
            State = state;
            switch (state)
            {
                case ViewModelState.AddNew:
                    break;
                case ViewModelState.InEdit:
                    break;
                case ViewModelState.Saved:
                    LoadTypesList();
                    break;
                case ViewModelState.Deleted:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        protected override bool IsValid()
        {
            bool isValid = true;
            if (Code <= 0)
            {
                AddError(nameof(Code), ValidationErrorsMessages.LOAN_TYPE_CODE_MUST_SUPPLIED);
                isValid = false;
            }
            else
            {
                RemoveError(nameof(Code), ValidationErrorsMessages.LOAN_TYPE_CODE_MUST_SUPPLIED);

            }
            if (string.IsNullOrEmpty(Description))
            {
                AddError(nameof(Description), ValidationErrorsMessages.LOAN_TYPE_DESCR_MUST_SUPPLIED);
                isValid = false;
            }
            else
            {
                RemoveError(nameof(Description), ValidationErrorsMessages.LOAN_TYPE_DESCR_MUST_SUPPLIED);
            }
            return isValid;
        }

        #endregion

        #region Functions

        private void ClearView()
        {
            Code = 0;
            Description = string.Empty;
        }
        private void LoadTypesList()
        {
            _repository = _unitOfWork.LoanTypes;
            var list = new ObservableCollection<LoanType>(_repository.ToList());
            TypesList = new ListCollectionView(list);
        }
        
        void ChageSelection()
        {
            if (SelectedType != null)
            {
               Code = SelectedType.Code;
               Description = SelectedType.LoanDescription;
               OnStateChanged(ViewModelState.Saved);
            }
            
        }
        #endregion

        #region IEntityMapper
        public void MapTo(LoanType entity, bool ignoreKey)
        {
            if(!ignoreKey) entity.Code = Code;
            entity.LoanDescription = Description;
        }

        public void MapFrom(LoanType entity)
        {
            Code = entity.Code;
            Description = entity.LoanDescription;
        }

        
        #endregion

    }
}