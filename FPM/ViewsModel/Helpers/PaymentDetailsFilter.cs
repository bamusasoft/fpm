using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Fbm.DomainModel;
using Fbm.DomainModel.Entities;

namespace Fbm.ViewsModel.Helpers
{
    public class PaymentDetailsFilter :INotifyPropertyChanged
    {

            bool _female;
            bool _male;
            string _memberName;
            int _memberCode;
            decimal _loanAmount;

            public bool Female 
            {
                get { return _female; }
                set
                {
                    _female = value;
                    RaisePropertyChanged();
                }
            }
            public bool Male
            {
                get { return _male; }
                set
                {
                    _male = value;
                    RaisePropertyChanged();
                }
            }
            public int MemberCode
            {
                get { return _memberCode; }
                set
                {
                    _memberCode = value;
                    RaisePropertyChanged();
                }
            }
            
            public string MemberName
            {
                get { return _memberName; }
                set
                {
                    _memberName = value;
                    RaisePropertyChanged();
                }
            }
            public decimal LoanAmount
            {
                get { return _loanAmount; }
                set
                {
                    _loanAmount = value;
                    RaisePropertyChanged();
                }
            }
            


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

            /// <summary>
            /// Build criteria based on current instance's search values.
            /// </summary>
            /// <returns>The Expression built. Null of not any of the proeprties has value.</returns>
            public Func<SearchablePaymentDetails, bool> BuildCriteria()
            {
                MethodInfo stringNullOrEmpty = typeof(string).GetMethod("IsNullOrEmpty");
                MethodInfo contains = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                MethodInfo compareTo = typeof(string).GetMethod("CompareTo", new[] { typeof(string) });
                MethodInfo startsWith = typeof(string).GetMethod("StartsWith", new[] { typeof(string) });
                ParameterExpression param = Expression.Parameter(typeof(SearchablePaymentDetails), "searchablePaymentDetails");
                Expression expr = null;

                Expression sexProperty = Expression.PropertyOrField(param, "Sex");
                Expression memberCodeProperty = Expression.PropertyOrField(param, "MemberCode");
                Expression memberNamePropery = Expression.PropertyOrField(param, "MemberName");
                Expression loanAmountProperty = Expression.PropertyOrField(param, "LoanAmount");


                Expression femaleValue = ConstantExpression.Constant(Sex.Female, typeof(Sex));
                Expression maleValue = ConstantExpression.Constant(Sex.Male, typeof(Sex));
                Expression memberCodeValue = ConstantExpression.Constant(this.MemberCode, typeof(int));
                Expression memberNameValue = ConstantExpression.Constant(this.MemberName, typeof(string));
                Expression loanAmountValue = ConstantExpression.Constant(this.LoanAmount, typeof(decimal));



                bool expressionAssigned = false;
                if (Female)
                {
                    //ConstantExpression femalConstant = Expression.Constant(Sex.Female);
                    Expression temp = Expression.Equal(sexProperty, femaleValue);
                    expr = temp;
                    if (!expressionAssigned) expressionAssigned = true;
                }
                if (Male)
                {
                    //ConstantExpression maleConst = Expression.Constant(Sex.Male);
                    if (!expressionAssigned)
                    {
                        Expression temp = Expression.Equal(sexProperty, maleValue);
                        expr = temp;
                        expressionAssigned = true;
                    }
                    else
                    {
                        Expression temp = Expression.Equal(sexProperty, maleValue);
                        expr = Expression.AndAlso(expr, temp);

                    }
                }


                if (!string.IsNullOrEmpty(MemberName))
                {

                    if (!expressionAssigned)
                    {
                        
                        Expression temp = Expression.Call(memberNamePropery, contains, memberNameValue);
                        expr = temp;
                        expressionAssigned = true;
                    }
                    else
                    {

                        Expression temp = Expression.Call(memberNamePropery, contains, memberNameValue);
                        expr = Expression.AndAlso(expr, temp);
                    }
                }
                if (LoanAmount > 0)
                {
                    if (!expressionAssigned)
                    {
                        Expression temp = Expression.Equal(loanAmountProperty, loanAmountValue);
                        expr = temp;
                        expressionAssigned = true;
                    }
                    else
                    {
                        Expression temp = Expression.Equal(loanAmountValue, loanAmountProperty);
                            
                        expr = Expression.AndAlso(expr, temp);
                    }
                }
                if (MemberCode > 0)
                {
                    if (!expressionAssigned)
                    {
                        Expression temp = Expression.Equal(Expression.Convert(memberCodeProperty, typeof(int)), memberCodeValue);
                        expr = temp;
                        expressionAssigned = true;
                    }
                    else
                    {
                        Expression temp = Expression.Equal(memberCodeProperty, memberCodeValue);
                        expr = Expression.AndAlso(expr, temp);
                    }
                }
                Expression<Func<SearchablePaymentDetails, bool>> criteria = null;
                if (expr != null)
                {
                    criteria = Expression.Lambda<Func<SearchablePaymentDetails, bool>>(expr, param);
                }
                if (criteria != null)
                {
                    return criteria.Compile();
                }
                else
                {
                    return null;
                }
            }

            #region "Search region"
            int _loanAmountFilter;
            public Dictionary<int, string> ArabicFilterChoices
            {
                get
                {
                    return new Dictionary<int, string>()
                {
                 {1, "يساوي"},
                 {2, "أكبر من"},
                 {3, "أصغر من"}
                };
                }
            }
            public Dictionary<int, string> EnglishFilterChoices
            {
                get
                {
                    return new Dictionary<int, string>()
                {
                 {1, "Equal"},
                 {2, "Greater Than"},
                 {3, "Less Than"}
                };
                }
            }

            public int LoanAmountFilter
            {
                get { return _loanAmountFilter; }
                set
                {
                    _loanAmountFilter = value;
                    RaisePropertyChanged();
                }
            }

            #endregion

    }
}
