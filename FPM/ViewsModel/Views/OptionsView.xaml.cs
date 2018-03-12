using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Fbm.DomainModel;
using Fbm.DomainModel.Entities;
using Fbm.ViewsModel.Properties;
using GalaSoft.MvvmLight.Command;

namespace Fbm.ViewsModel.Views
{
    /// <summary>
    /// Interaction logic for OptionsView.xaml
    /// </summary>
    public partial class OptionsView : Window, INotifyPropertyChanged
    {
        #region "Fields"
        string _memberStatmentTemplatePath;
        string _loansStatementTemplatePath;
        string _pdfsFolder;
        string _payDetailsTemplate;
        bool _showReports;
        string _familyDbPath;
        PeriodSetting _currentPeriod;
        string _logFileFolder;
        IUnitOfWork _unitOfWork;
        PeriodStatus _openPeriod;
        PeriodStatus _closePeriod;
        Settings _settings;
        RelayCommand _saveCommand;
        RelayCommand _openMemberStatementCommand;
        RelayCommand _openLoansStatementCommand;
        RelayCommand _openPdfsFolderCommand;
        RelayCommand _openLogFolderCommand;
        RelayCommand _closeYearCommand;
        RelayCommand _openPayDetailsTemplateCommand;
        RelayCommand _openFamilyDbCommand;
        #endregion

        public OptionsView()
        {
            InitializeComponent();
            Loaded += WindowLoaded;
            _settings = Settings.Default;
            _unitOfWork = new UnitOfWork();
            DataContext = this;
        }
        #region "Events"
        void WindowLoaded(object sender, RoutedEventArgs e)
        {
            ReadValues();
        }
        #endregion

        #region "Properties"
        public string FamilyDbPath
        {
            get{return _familyDbPath;}
            set
            {
                _familyDbPath =value;
                RaisePropertyChanged();
            }
        }
        public string PayDetailsTemplate
        {
            get
            {
                return _payDetailsTemplate;
            }
            set
            {
                _payDetailsTemplate = value;
                RaisePropertyChanged();
            }
        }
        public bool ShowReports
        {
            get
            {
                return _showReports;
            }
            set
            {
                _showReports = value;
                RaisePropertyChanged();
            }

        }
        public string LogFileFolder
        {
            get
            {
                return _logFileFolder;
            }
            set
            {
                _logFileFolder = value;
                RaisePropertyChanged();
            }
        }
        private PeriodStatus OpenStatus
        {
            get
            {
                if (_openPeriod == null)
                {
                    _openPeriod = _unitOfWork.PeriodStatuses.GetById(2);
                }
                return _openPeriod;
            }
        }
        private PeriodStatus CloseStatus
        {
            get
            {
                if (_closePeriod == null)
                {
                    _closePeriod = _unitOfWork.PeriodStatuses.GetById(1);
                }
                return _closePeriod;
            }
        }

        public string MemberStatmentTemplatePath
        {
            get { return _memberStatmentTemplatePath; }
            set
            {
                _memberStatmentTemplatePath = value;
                RaisePropertyChanged();
            }

        }
        public string LoansStatementTemplatePath
        {
            get { return _loansStatementTemplatePath; }
            set
            {
                _loansStatementTemplatePath = value;
                RaisePropertyChanged();
            }
        }
        public string CurrentYear
        {
            get 
            { 
                return  CurrentPeriod != null ? CurrentPeriod.YearPart :string.Empty;
            }

        }
        public PeriodSetting CurrentPeriod
        {
            get { return _currentPeriod; }
            set
            {
                _currentPeriod = value;
                RaisePropertyChanged();
                RaisePropertyChanged("CurrentYear");
            }
        }
        public string PdfsFolder
        {
            get { return _pdfsFolder; }
            set 
            {
                _pdfsFolder = value;
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

        #region "Commands"
        public ICommand OpenPayDetailsTemplateCommand
        {
            get
            {
                if (_openPayDetailsTemplateCommand == null)
                {
                    _openPayDetailsTemplateCommand = new RelayCommand(OpenPayDetailsTemp);
                }
                return _openPayDetailsTemplateCommand;
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

        public ICommand CloseYearCommand
        {
            get
            {
                if (_closeYearCommand == null)
                {
                    _closeYearCommand = new RelayCommand(CloseYear);
                }
                return _closeYearCommand;
            }
        }
        public ICommand OpenMemberStatementCommand
        {
            get
            {
                if (_openMemberStatementCommand == null)
                {
                    _openMemberStatementCommand = new RelayCommand(OpenMemberStatement);
                }
                return _openMemberStatementCommand;
            }
        }
        public ICommand OpenLoansStatementCommand
        {
            get
            {
                if(_openLoansStatementCommand == null)
                {
                    _openLoansStatementCommand = new RelayCommand(OpenLoansStatement);
                }
                return _openLoansStatementCommand;
            }
        }
        public ICommand OpenPdfsFolderCommand
        {
            get
            {
                if (_openPdfsFolderCommand == null)
                {
                    _openPdfsFolderCommand = new RelayCommand(OpenPdfsFolder);
                }
                return _openPdfsFolderCommand;
            }
        }
        public ICommand OpenLogFolderCommand
        {
            get
            {
                if (_openLogFolderCommand == null)
                {
                    _openLogFolderCommand = new RelayCommand(OpenLogFileFolder);
                }
                return _openLogFolderCommand;
            }
        }

        public ICommand OpenFamilyDbCommand
        {
            get
            {
                if (_openFamilyDbCommand == null)
                {
                    _openFamilyDbCommand = new RelayCommand(OpenFamilyDb);
                }
                return _openFamilyDbCommand;
            }
        }
        #endregion

        #region "Commands Methods"

        void Save()
        {
            WriteValues();
            _settings.Save();
            _unitOfWork.Save();
        }
        void CloseYear()
        {
            string msg = Properties.Resources.OptionView_PromptForCloseYear;
            if (Helper.UserConfirmed(msg))
            {
                CurrentPeriod.PeriodStatus = CloseStatus;
            }
            

        }
        void OpenMemberStatement()
        {
            string filter = "Excel Templates (*.xltx)|*.xltx";
            MemberStatmentTemplatePath = OpenFile(filter);
        }
        void OpenLoansStatement()
        {
            string filter = "Word Templates (*.dotx)|*.dotx";
            LoansStatementTemplatePath = OpenFile(filter);
        }
        void OpenPdfsFolder()
        {
            PdfsFolder = OpenFolder();
        }
        void OpenLogFileFolder()
        {
            LogFileFolder = OpenFolder();
        }
        void OpenPayDetailsTemp()
        {
            string filter = "Excel Templates (*.xltx)|*.xltx";
            PayDetailsTemplate = OpenFile(filter);
        }
        void OpenFamilyDb()
        {
            string filter = "Access database (*.mdb)|*.mdb";
            FamilyDbPath  = OpenFile(filter);
        }
        #endregion

        #region "Helpers"
        void ReadValues()
        {
            MemberStatmentTemplatePath = _settings.MemberStatmentTemplatePath;
            LoansStatementTemplatePath = _settings.LoansStatementTemplatePath;
            PdfsFolder = _settings.PdfsFolder;
            LogFileFolder = _settings.LogFilePath;
            FamilyDbPath = _settings.FamilyDbPath;
            PayDetailsTemplate = _settings.PaymentDetailsTemplatePath;
            ShowReports = _settings.ShowReports;
            CurrentPeriod = GetCurrentYear();
        }

        private PeriodSetting GetCurrentYear()
        {
            PeriodSetting currnet = null;
            try
            {
                var c = _unitOfWork.PeriodSettings.Query(x => x.PeriodStatus.Id == OpenStatus.Id).Single();
                currnet = c;
            }
            catch (InvalidOperationException )
            {
                
                //Sowallo as we sure nothing happend other than that there are no open year.
            }
            return currnet;
            

        }
        void WriteValues()
        {
            _settings.MemberStatmentTemplatePath = MemberStatmentTemplatePath;
            _settings.LoansStatementTemplatePath = LoansStatementTemplatePath;
            _settings.PdfsFolder = PdfsFolder;
            _settings.LogFilePath = LogFileFolder;
            _settings.PaymentDetailsTemplatePath =  PayDetailsTemplate;
            _settings.ShowReports = ShowReports ;
            _settings.FamilyDbPath = FamilyDbPath;
        }
        string OpenFile(string filter)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = filter;
            ofd.ShowDialog();
            return ofd.FileName;
        }
        string OpenFolder()
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.ShowDialog();
            return fbd.SelectedPath;
        }
        #endregion
    }
}


