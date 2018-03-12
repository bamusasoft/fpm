using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Fbm.FamilyAppService;
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
    public class SyncMembersViewModel : EditableViewModelBase
    {
        #region Constructor

        [ImportingConstructor]
        public SyncMembersViewModel(ISettings settings)
        {
            _settings = new GlobalConfigService(settings);
            CanClose = true;
            Title = ViewModelsTitles.SYNC_MEMBERS;
            Errors = new Dictionary<string, List<string>>();
            OnStateChanged(ViewModelState.AddNew);

        }

        #endregion

        #region "Fields"

        private FamilyContext _unitOfWork;
        private double _progressCounter;
        private bool _syncEnabled;
        private bool _progressVisibility;
        private readonly IGlobalConfigService _settings;
        private DelegateCommand _syncCommand;

        #endregion

        #region "Commands"

        public ICommand SyncCommand
        {
            get { return _syncCommand ?? (_syncCommand = new DelegateCommand(SyncMembers, CanSync)); }
        }

        private async void SyncMembers()
        {
            OnStateChanged(ViewModelState.InEdit);
            var tsk = SyncShareHolders();
            await tsk;
        }

        private bool CanSync()
        {
            return SyncEnabled;
        }

        #endregion

        #region Base

        protected override void Initialize()
        {
            _unitOfWork = new FamilyContext();
        }

        protected override void Save()
        {
            try
            {
                _unitOfWork.SaveChanges();
                Initialize();
            }
            catch (Exception ex)
            {
                Helper.LogAndShow(ex);
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
            throw new NotImplementedException();
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
                    EnableSave = true;
                    SyncEnabled = true;
                    ProgressVisibility = false;
                    break;
                case ViewModelState.InEdit:
                    SyncEnabled = false;
                    ProgressVisibility = true;
                    break;
                case ViewModelState.Saved:
                    break;
                case ViewModelState.Deleted:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        protected override bool IsValid()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region "Properties"

       

        public double ProgressCounter
        {
            get { return _progressCounter; }
            set { SetProperty(ref _progressCounter, value); }
        }

        public bool SyncEnabled
        {
            get { return _syncEnabled; }
            set { SetProperty(ref _syncEnabled, value); }
        }

        public bool ProgressVisibility
        {
            get { return _progressVisibility; }
            set { SetProperty(ref _progressVisibility, value); }
        }

        #endregion

        #region "Sync Service"

        private async Task SyncShareHolders()
        {
            var familyDbPath = (string) _settings.Get(SettingsNames.FAMILY_DB_PATH);
            if (string.IsNullOrEmpty(familyDbPath))
            {
                var msg = ""; // Properties.Resources.SyncMembersView_FamilyDbMissing;
                Helper.ShowMessage(msg);
                return;
            }
            await Task.Run(() =>
            {
                var familyAppRepo = new Repository(familyDbPath);
                var familyAppMembers = familyAppRepo.GetShareHolders().ToList();
                var myFlopMembers = _unitOfWork.FamilyMembers;
                var current = 0;
                var count = familyAppMembers.Count;
                foreach (var familyMember in familyAppMembers)
                {
                    var exist = myFlopMembers.Any(shHo => shHo.Code == familyMember.Code);
                    if (!exist)
                    {
                        var newMember = new FamilyMember
                        {
                            Code = familyMember.Code,
                            Parent = familyMember.Parent,
                            FirstName = familyMember.FirstName,
                            FullName = familyMember.FullName,
                            MotherCode = familyMember.MotherCode,
                            Independent = familyMember.Independent,
                            IndependentDate = familyMember.IndependentDate,
                            Alive = familyMember.Alive,
                            HasChildren = familyMember.HasChildren,
                            Shares = familyMember.Shares,
                            XShares = familyMember.XShares,
                            Buffer = familyMember.Buffer,
                            ShareHolderLevel = familyMember.ShareHolderLevel,
                            Sex = familyMember.Sex,
                            PayMethod = PayMethod.BankTransfer
                        };

                        myFlopMembers.Add(newMember);
                    }
                    else
                    {
                        var existingMember = myFlopMembers.Single(sh => sh.Code == familyMember.Code);
                        existingMember.Parent = familyMember.Parent;
                        existingMember.FirstName = familyMember.FirstName;
                        existingMember.FullName = familyMember.FullName;
                        existingMember.MotherCode = familyMember.MotherCode;
                        existingMember.Independent = familyMember.Independent;
                        existingMember.IndependentDate = familyMember.IndependentDate;
                        existingMember.Alive = familyMember.Alive;
                        existingMember.HasChildren = familyMember.HasChildren;
                        existingMember.Shares = familyMember.Shares;
                        existingMember.XShares = familyMember.XShares;
                        existingMember.Buffer = familyMember.Buffer;
                        existingMember.ShareHolderLevel = familyMember.ShareHolderLevel;
                        existingMember.Sex = familyMember.Sex;
                    }
                    UpdateProgress(count, current);
                    current++;
                }
            });
        }


        private void UpdateProgress(double count, double current)
        {
            var c = (current/count)*100;
            ProgressCounter = c;
        }

        #endregion
    }
}