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
using Fbm.ViewsModel.Properties;
using GalaSoft.MvvmLight.Command;

namespace Fbm.ViewsModel.Views
{
    /// <summary>
    /// Interaction logic for LoanTypesView.xaml
    /// </summary>
    public partial class LoanTypesView : Window, INotifyPropertyChanged
    {
       
        public LoanTypesView()
        {
            InitializeComponent();
            DataContext = this;
            this.Loaded += WindowLoaded;
            _unitOfWork = new UnitOfWork();
            _repository = _unitOfWork.LoanTypes;
            _settings = Settings.Default;
            RulesViolations = new List<RuleViolation>();
        }


        #region "Fields"
        int _code;
        string _description;
        string _searchField;
        ModelState _modelState;
        IUnitOfWork _unitOfWork;
        RepositoryBase<LoanType> _repository;
        LoanType _currnetModel;
        bool ChangedFlage { get; set; }
        RelayCommand _saveCommand;
        RelayCommand _searchCommand;
        RelayCommand _addNewcommand;
        Settings _settings;
        List<RuleViolation> RulesViolations { get; set; }
        #endregion
        
        #region "Events"
        void WindowLoaded(object sender, RoutedEventArgs e)
        {

            SetState(ModelState.New);

        }
        #endregion
        #region "Properties"
        public int Code
        {
            get { return _code; }
            set
            {
                _code = value;
                RaisePropertyChanged();
                SetChangedFlage();
            }
        }
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                RaisePropertyChanged();
                SetChangedFlage();
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

        #region "Helpers"
        void SetState(ModelState state)
        {
            _modelState = state;
            switch (_modelState)
            {
                case ModelState.New:
                    Initialize();
                    ResetChangedFlage();
                    break;
                case ModelState.Saved:
                    txtCode.IsEnabled = false;
                    ResetChangedFlage();
                    break;
                case ModelState.Deleted:
                    ResetChangedFlage();
                    break;
                default:
                    break;
            }
        }

        private void Initialize()
        {
            
            _currnetModel = new LoanType();
            ReadModelValues(_currnetModel);
            txtCode.IsEnabled = true;
            txtCode.Focus();
            txtCode.SelectAll();
        }

        private void ReadModelValues(LoanType model)
        {
            if (model == null) throw new ArgumentNullException("model");
            Code = model.Code;
            Description = model.LoanDescription;
            RaisePropertyChanged("");
        }
        void WriteModelValues(LoanType model)
        { 
            if (model == null) throw new ArgumentNullException("model");
            model.Code = Code;
            model.LoanDescription = Description;

        }

        
        void SetChangedFlage()
        {
            ChangedFlage = true;
        }
        void ResetChangedFlage()
        {
            ChangedFlage = false;
        }

        bool ValidData()
        {
            bool valid = true;

            if (Code < 10 )
            {
                RulesViolations.Add(new RuleViolation(Properties.Resources.LoanTypesView_InvalidCode));
                return false;
            }
            if (string.IsNullOrEmpty(Description))
            {
                RulesViolations.Add(new RuleViolation(Properties.Resources.LoanTypesView_MissingDescription));
                return false;
            }
            

            return valid;
        }
        bool OkClose()
        {
            return !ChangedFlage;
        }
        #endregion

        #region "Commands"
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
        void Save()
        {
            if (!ValidData())
            {
                string msg = null;
                foreach (var viol in RulesViolations)
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
        void Search()
        {
            if (string.IsNullOrEmpty(SearchField)) return;
            int searchedCode;
            if (int.TryParse(SearchField, out searchedCode))
            {
                var result = _repository.Query(loan => loan.Code == searchedCode);
                if (result == null || result.Count() != 1) return;
                _currnetModel = result.First();
                ReadModelValues(_currnetModel);
                SetState(ModelState.Saved);
            }
        }
        public ICommand AddNewCommand
        {
            get
            {
                if (_addNewcommand == null)
                {
                    _addNewcommand = new RelayCommand(AddNew);
                }
                return _addNewcommand;
            }
        }
        void AddNew()
        {
            if (!OkClose())
            {
                string msg = Properties.Resources.PromptForSaveMsg;
                if (!Helper.UserConfirmed(msg)) return;
              
            }
            SetState(ModelState.New);

        }
        #endregion




    }
}

    