using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Fbm.DomainModel;
using Fbm.DomainModel.Entities;
using Fbm.FamilyAppService;
using GalaSoft.MvvmLight.Command;

namespace Fbm.ViewsModel.Views
{
    /// <summary>
    /// Interaction logic for SyncMembersView.xaml
    /// </summary>
    public partial class SyncMembersView : INotifyPropertyChanged
    {
        #region "Fields"

        readonly IUnitOfWork _unitOfWork;
        double _progressCounter;
        RelayCommand _saveCommand;
        bool _enableSave;
        State _currentState;
        #endregion
        public SyncMembersView()
        {
            InitializeComponent();
            DataContext = this;
            _unitOfWork = new UnitOfWork();
            Loaded += WindowLoaded;
        }
        #region "Events"
        void WindowLoaded(object sender, RoutedEventArgs e)
        {
            ControlState(State.Loaded);
            
        }

        private async void SyncMembersClick(object sender, RoutedEventArgs e)
        {
            ControlState(State.InSync);
            Task tsk = SyncShareHolders();
            await tsk;
            ControlState(State.SyncCompleted);
        }
        #endregion
        #region "Commands"
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
        #endregion
        #region "Commands Methods"
        void Save()
        {
            try
            {
                _unitOfWork.Save();
            }
            catch (Exception ex)
            {
                Helper.LogAndShow(ex);
            }
        }
        bool CanSave()
        {
            return EnableSave;
        }
        #endregion

        #region "Properties"

        public bool EnableSave
        {
            get { return _enableSave; }
            set
            {
                _enableSave = value;
                RaisePropertyChanged();
                CommandManager.InvalidateRequerySuggested();
            }
        }
        public double ProgressCounter
        {
            get { return _progressCounter; }
            set
            {
                _progressCounter = value;
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

        #region "Sync Service"
        private async Task SyncShareHolders()
        {

            string familyDbPath = Properties.Settings.Default.FamilyDbPath;
            if (string.IsNullOrEmpty(familyDbPath))
            {
                string msg = Properties.Resources.SyncMembersView_FamilyDbMissing;
                Helper.ShowMessage(msg);
                return;

            }
            await Task.Run(() =>
            {
                var familyAppRepo = new Repository(familyDbPath);

                var familyAppMembers = familyAppRepo.GetShareHolders();
                var fbmRepo = _unitOfWork.FamilyMembers;
                var fbmMembers = fbmRepo.GetAll();
                int current = 0;
                int count = familyAppMembers.Count();
                foreach (FamilyMember familyMember in familyAppMembers)
                {
                    //bool exist = sgeCustomers.Any(cus => cus.CustomerId == irsCustomer.CustomerId);
                    bool exist = fbmMembers.Any(shHo => shHo.Code == familyMember.Code);
                    if (!exist)
                    {
                        var members = new FamilyMember
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
                            Sex = familyMember.Sex
                        };

                        fbmRepo.Add(members);
                    }
                    else
                    {
                        FamilyMember existingMember = fbmMembers.Single(sh => sh.Code == familyMember.Code);
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
        
        #endregion

        #region "Helpers"
        void UpdateProgress(double count, double current)
        {
            double c = (current / count) * 100;
            ProgressCounter = c;
        }
        void ControlState(State currentState)
        {
            _currentState = currentState;
            switch (_currentState)
            {
                case State.Loaded:
                    progBar.Visibility = Visibility.Hidden;
                    EnableSave = false;
                    break;
                case State.InSync:
                    _enableSave = false;
                    progBar.Visibility = Visibility.Visible;
                    btnSync.IsEnabled = false;
                    break;
                case State.SyncCompleted:
                   progBar.Visibility = Visibility.Hidden;
                   btnSync.IsEnabled = true;
                   EnableSave = true;  
                    break;
            }
        }
        enum State
        {
            Loaded,
            InSync,
            SyncCompleted,
        }
        #endregion


    }
}
