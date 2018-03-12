﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using FlopManager.Domain;
using FlopManager.Domain.EF;
using FlopManager.Services.Helpers;
using FlopManager.Services.ViewModelInfrastructure;
using FlopManager.SettingsModule.Properties;
using Prism.Commands;
using Prism.Regions;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace FlopManager.SettingsModule.ViewModels
{
    public class OptionsViewModel:EditableViewModelBase
    {
        #region "Fields"
        string _memberStatmentTemplatePath;
        string _loansStatementTemplatePath;
        string _pdfsFolder;
        string _payDetailsTemplate;
        bool _showReports;
        string _familyDbPath;
        PeriodYear _currentPeriod;
        string _logFileFolder;
        FamilyContext  _unitOfWork;
        Settings _settings;

        DelegateCommand _openMemberStatementCommand;
        DelegateCommand _openLoansStatementCommand;
        DelegateCommand _openPdfsFolderCommand;
        DelegateCommand _openLogFolderCommand;
        DelegateCommand _closeYearCommand;
        DelegateCommand _openPayDetailsTemplateCommand;
        DelegateCommand _openFamilyDbCommand;
        #endregion

        public OptionsViewModel()
        {
            WindowLoaded();
            _settings = Settings.Default;
            _unitOfWork = new FamilyContext();
        }
        #region "Events"
        void WindowLoaded()
        {
            ReadValues();
        }

        protected override void Initialize()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region "Properties"
        public string FamilyDbPath
        {
            get { return _familyDbPath; }
            set { SetProperty(ref _familyDbPath, value); }
        }
        public string PayDetailsTemplate
        {
            get
            {
                return _payDetailsTemplate;
            }
            set
            {
                SetProperty(ref _familyDbPath, value);
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
                SetProperty(ref _showReports, value);
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
                SetProperty(ref _familyDbPath, value);
            }
        }
        private YearStatus OpenStatus
        {
            get { return YearStatus.Present; }
        }
        private YearStatus CloseStatus
        {
            get { return YearStatus.Past; }
        }

        public string MemberStatmentTemplatePath
        {
            get { return _memberStatmentTemplatePath; }
            set
            {
                SetProperty(ref _memberStatmentTemplatePath, value);
            }

        }
        public string LoansStatementTemplatePath
        {
            get { return _loansStatementTemplatePath; }
            set
            {
                SetProperty(ref _loansStatementTemplatePath, value);
            }
        }
        public string CurrentYear
        {
            get
            {
                return CurrentPeriod != null ? CurrentPeriod.Year : string.Empty;
            }

        }
        public PeriodYear CurrentPeriod
        {
            get { return _currentPeriod; }
            set
            {
                SetProperty(ref _currentPeriod, value);
                OnPropertyChanged(nameof(CurrentYear));
            }
        }
        public string PdfsFolder
        {
            get { return _pdfsFolder; }
            set
            {
                SetProperty(ref _pdfsFolder, value);
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
                    _openPayDetailsTemplateCommand = new DelegateCommand(OpenPayDetailsTemp);
                }
                return _openPayDetailsTemplateCommand;
            }
        }

      

        public ICommand CloseYearCommand
        {
            get
            {
                if (_closeYearCommand == null)
                {
                    _closeYearCommand = new DelegateCommand(CloseYear);
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
                    _openMemberStatementCommand = new DelegateCommand(OpenMemberStatement);
                }
                return _openMemberStatementCommand;
            }
        }
        public ICommand OpenLoansStatementCommand
        {
            get
            {
                if (_openLoansStatementCommand == null)
                {
                    _openLoansStatementCommand = new DelegateCommand(OpenLoansStatement);
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
                    _openPdfsFolderCommand = new DelegateCommand(OpenPdfsFolder);
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
                    _openLogFolderCommand = new DelegateCommand(OpenLogFileFolder);
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
                    _openFamilyDbCommand = new DelegateCommand(OpenFamilyDb);
                }
                return _openFamilyDbCommand;
            }
        }
        #endregion

        #region "Commands Methods"

        protected override void Save()
        {
            WriteValues();
            _settings.Save();
            _unitOfWork.SaveChanges();
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
            throw new NotImplementedException();
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

        void CloseYear()
        {
            string msg = "";//Properties.Resources.OptionView_PromptForCloseYear;
            if (Helper.UserConfirmed(msg))
            {
                CurrentPeriod.Status = CloseStatus;
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
            FamilyDbPath = OpenFile(filter);
        }
        #endregion

        #region "Helpers"
        void ReadValues()
        {
            MemberStatmentTemplatePath =  "" ; //      _settings.MemberStatmentTemplatePath;
            LoansStatementTemplatePath =  "" ; //      _settings.LoansStatementTemplatePath;
            PdfsFolder =                  "" ; //     _settings.PdfsFolder;
            LogFileFolder =               "" ; //     _settings.LogFilePath;
            FamilyDbPath =                "" ; //     _settings.FamilyDbPath;
            PayDetailsTemplate =          "" ; //     _settings.PaymentDetailsTemplatePath;
            ShowReports =                 false ; //     _settings.ShowReports;
            CurrentPeriod =                             GetCurrentYear();
        }

        private PeriodYear GetCurrentYear()
        {
            PeriodYear currnet = null;
            try
            {
                var c = _unitOfWork.PeriodYears.Where(x => x.Status == YearStatus.Present).Single();
                currnet = c;
            }
            catch (InvalidOperationException)
            {

                //Sowallo as we sure nothing happend other than that there are no open year.
            }
            return currnet;


        }
        void WriteValues()
        {
            //_settings.MemberStatmentTemplatePath = MemberStatmentTemplatePath;
            //_settings.LoansStatementTemplatePath = LoansStatementTemplatePath;
            //_settings.PdfsFolder = PdfsFolder;
            //_settings.LogFilePath = LogFileFolder;
            //_settings.PaymentDetailsTemplatePath = PayDetailsTemplate;
            //_settings.ShowReports = ShowReports;
            //_settings.FamilyDbPath = FamilyDbPath;
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
