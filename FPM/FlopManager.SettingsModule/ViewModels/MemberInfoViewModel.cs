using FlopManager.Domain;
using FlopManager.Domain.EF;
using FlopManager.Services;
using FlopManager.Services.Helpers;
using FlopManager.Services.ViewModelInfrastructure;
using Prism.Logging;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlopManager.SettingsModule.ViewModels
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class MemberInfoViewModel : EditableViewModelBase, IEntityMapper<FamilyMember>
    {
        [ImportingConstructor]
        public MemberInfoViewModel(ILogger logger)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            _logger = logger;
            CanClose = true;
            Title = ViewModelsTitles.MEMBER_INFO;
           
        }
        #region Fields
        private string _memberCode;
        private string _memberName;
        private KeyValuePair<PayMethod, string> _SelectedPayMethod;
        private Dictionary<PayMethod, string> _payMethods;
        private ILogger _logger;
        private FamilyContext _unitOfWork;
        private bool _enableMemberCode;
       
        #endregion
        #region Properties
        public string MemberCode
        {
            get { return _memberCode; }
            set
            {
                SetProperty(ref _memberCode, value);
            }
        }
        public string MemberName
        {
            get { return _memberName; }
            set
            {
                SetProperty(ref _memberName, value);
            }
        }
        public KeyValuePair<PayMethod, String> SelectedPayMethod
        {
            get { return _SelectedPayMethod; }
            set
            {
                SetProperty(ref _SelectedPayMethod, value);
                OnStateChanged(ViewModelState.InEdit);
            }
        }
        public Dictionary<PayMethod, string> PayMethods
        {
            get { return _payMethods; }
            set
            {
                SetProperty(ref _payMethods, value);
            }
        }
        public bool EnableMemberCode
        {
            get { return _enableMemberCode; }
            set
            {
                SetProperty(ref _enableMemberCode, value);
            }
        }
        
        #endregion

        #region Base
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
                    EnableSave = false;
                    EnableMemberCode = true;
                    break;
                case ViewModelState.InEdit:
                     EnableSave = true;
                    EnableMemberCode = false;
                    break;
                case ViewModelState.Busy:
                    break;
                case ViewModelState.Saved:
                     EnableSave = true;
                    EnableMemberCode = false;
                    break;
                case ViewModelState.Deleted:
                    break;
            }
        }

        protected override void AddNew()
        {
            if (CanExit())
            {
                ClearView();
                OnStateChanged(ViewModelState.AddNew);
            }
        }

        protected override void Delete()
        {
            throw new NotImplementedException();
        }

        protected override void Initialize()
        {
            Errors = new Dictionary<string, List<string>>();
            _unitOfWork = new FamilyContext();
            LoadPayMethods();
            OnStateChanged(ViewModelState.AddNew);

        }

        protected override bool IsValid()
        {
            var isValid = true;
            if (SelectedPayMethod.Key != PayMethod.BankTransfer && SelectedPayMethod.Key != PayMethod.Check)
            {
                AddError(nameof(SelectedPayMethod), ValidationErrorsMessages.PAY_METHOD_NOT_SELECTED);
                isValid = false;
            }
            else
            {
                RemoveError(nameof(SelectedPayMethod), ValidationErrorsMessages.PAY_METHOD_NOT_SELECTED);
            }

            return isValid;
        }

        protected override void Print()
        {
            throw new NotImplementedException();
        }

        protected override void Save()
        {
            if (IsValid())
            {
                try
                {
                    int code = int.Parse(MemberCode);
                    var member = FindMember(code);
                    MapTo(member, true);
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

        protected override void Search(object criteria)
        {
            if (criteria == null) return;
            int code = int.Parse(criteria.ToString());
            var member = FindMember(code);
            if(member != null)
            {
                MapFrom(member);
            }
            OnStateChanged(ViewModelState.Saved);
        }
        #endregion
        #region Functions
        FamilyMember FindMember(int code)
        {
            return _unitOfWork.FamilyMembers.Find(code);
        }
        void ClearView()
        {
            MemberCode = "";
            MemberName = "";
           
        }
        void LoadPayMethods()
        {
            PayMethods = new Dictionary<PayMethod, string>()
            {
                {PayMethod.BankTransfer, "حوالة" },
                {PayMethod.Check, "شيك"}

            };
        }
        private KeyValuePair<PayMethod, string> SelectMethod(PayMethod method)
        {
            return PayMethods.Single(x => x.Key == method);
        }
        #endregion
        #region "IEntity Mapper"
        public void MapTo(FamilyMember entity, bool ignoreKey)
        {
            //Alawys ignore key; because we just updating pay method of a member 
            entity.PayMethod = SelectedPayMethod.Key;
        }

        public void MapFrom(FamilyMember entity)
        {
            MemberCode = entity.Code.ToString();
            MemberName = entity.FullName;
            SelectedPayMethod = SelectMethod(entity.PayMethod);
        }

        
        #endregion

    }
}
