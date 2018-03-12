using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using Fbm.DomainModel;
using Fbm.DomainModel.Entities;
using Fbm.ViewsModel.Helpers;
using Fbm.ViewsModel.Properties;


namespace Fbm.ViewsModel
{
    public static class Helper
    {
        static PeriodSetting _currentPeriod;
        /// <summary>
        /// Gets all exception messages in the supplied exception.
        /// </summary>
        /// <param name="ex">The exception.</param>
        /// <returns></returns>
        public static string ProcessExceptionMessages(Exception ex)
        {
            var sb = new StringBuilder();
            return GetAllExceptionMessages(ex, sb);
        }

        private static string GetAllExceptionMessages(Exception exception, StringBuilder sb)
        {
            if (null == sb) throw new ArgumentNullException("sb");
            if (exception != null)
            {
                sb.AppendLine(exception.Message);
                GetAllExceptionMessages(exception.InnerException, sb);
            }
            return sb.ToString();
        }
        public static void ShowMessage(string msg)
        {
         MessageBox.Show(msg, "JSA", MessageBoxButton.OK, 
                          MessageBoxImage.Information, 
                          MessageBoxResult.OK, 
                          MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
        }
        public static int StartNewIncrement(string activeYear)
        {
            string s = activeYear + "0001";
            return int.Parse(s);
        }
        public static bool DateInYearRange(string dateString, string yearRange)
        {
            bool result = false;
            if (string.IsNullOrEmpty(dateString)) { return false; }
            if (dateString.Length != 8) { return false; }
            int currentYear;
            if (!int.TryParse(yearRange, out currentYear))
            {
                return false;
            }
            string d = dateString.Substring(6, 2);
            string m = dateString.Substring(4, 2);
            string y = dateString.Substring(0, 4);

            int day;
            int month;
            int year;

            if (int.TryParse(d, out day) &&
                int.TryParse(m, out month) &&
                int.TryParse(y, out year))
            {
                if (
                     (day > 0 && day < 31)
                     &&
                     (month > 0 && month <= 12)
                     &&
                     (year == currentYear))
                {
                    result = true;
                }
            }
            return result;
        }

        public static bool ValidDate(string dateString)
        {

            bool result = false;
            if (string.IsNullOrEmpty(dateString)) { return false; }
            if (dateString.Length != 8) { return false; }
            string d = dateString.Substring(6, 2);
            string m = dateString.Substring(4, 2);
            string y = dateString.Substring(0, 4);

            int day;
            int month;
            int year;

            if (int.TryParse(d, out day) &&
                int.TryParse(m, out month) &&
                int.TryParse(y, out year))
            {
                if (
                     (day > 0 && day < 31)
                     &&
                     (month > 0 && month <= 12)
                     &&
                     (year <= 1500))
                {
                    result = true;
                }
            }
            return result;
        }
        public static bool UserConfirmed(string msg)
        {
            var result = MessageBox.Show(msg, "FPM", MessageBoxButton.YesNo,
                                          MessageBoxImage.Question, MessageBoxResult.No,
                                          MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
            if (result == MessageBoxResult.Yes) return true;
            return false;

        }
        /// <summary>
        /// Get the current Period.
        /// </summary>
        /// <returns>Current Period if set, otherwise null.</returns>
        private static PeriodSetting CurrentPeriod
        {
            get
            {
                if (_currentPeriod == null)
                {
                    try
                    {
                        using (IUnitOfWork unit = new UnitOfWork())
                        {

                            var result = unit.PeriodSettings.Query(x => x.PeriodStatus.Id == 2).Single();
                            _currentPeriod = new PeriodSetting()
                            {
                                Id = result.Id,
                                StartDate = result.StartDate,
                                EndDate = result.EndDate,
                                YearPart = result.YearPart,
                                Loans = result.Loans,
                                PaymentInstructions = result.PaymentInstructions,
                                Payments = result.Payments,
                                PeriodStatus = result.PeriodStatus,
                                StatusId = result.StatusId,
                            };
                        }
                    }
                    catch
                    {
                        return _currentPeriod;
                    }
                }
                return _currentPeriod;
            }
        }

        public static string GenerateLoanNo(string activeYear)
        {
            if (string.IsNullOrEmpty(activeYear)) throw new ArgumentNullException("activeYear");
            string currentYearPortion = activeYear.Substring(2, 2);
            string dbMaxNo = Loan.MaxNo;

            if (!string.IsNullOrEmpty(dbMaxNo))
            {
                string dbYearPortion = dbMaxNo.Substring(0, 2);
                if (dbYearPortion.Equals(currentYearPortion))
                {
                    string incrementedPortion = dbMaxNo.Substring(3, 3);
                    int incrementedNo;

                    if (int.TryParse(incrementedPortion, out incrementedNo))
                    {
                        incrementedNo++;
                    }
                    return currentYearPortion + DecorateNo(incrementedNo); ;
                }
            }
            return StartNewIncrement(currentYearPortion).ToString();
        }
        public static string AutoIncrement(string maxNo, int counter)
        {
            string yearPortion = maxNo.Substring(0, 2);
            string incrementedPortion = maxNo.Substring(3, 3);
            int incrementedNo;

            if (int.TryParse(incrementedPortion, out incrementedNo))
            {
                incrementedNo = incrementedNo + counter;
            }
            return yearPortion + DecorateNo(incrementedNo); ;
        }
        
        private static string DecorateNo(int i)
        {
            string s = i.ToString();
            switch (s.Length)
            {
                case 1:
                    return "000" + s;
                case 2:
                    return "00" + s;
                case 3:
                    return "0" + s;
                case 4:
                    return s;
                default:
                    throw new IndexOutOfRangeException("Schedule No. can't be greater than 9999");
            }

        }
        public static string ProcessDbError(Exception e)
        {
            string exceptionMsg = ProcessExceptionMessages(e);
            string general = Properties.Resources.DbErrorMsg;
            return general + Environment.NewLine + exceptionMsg;
        }
        public static void LogAndShow(Exception ex)
        {
            string msg = ProcessExceptionMessages(ex);
            Logger.Log(LogMessageTypes.Error, msg, ex.TargetSite.Name, ex.StackTrace);
            ShowMessage(msg);
        }
        public static void LogOnly(Exception ex)
        {
            string msg = ProcessExceptionMessages(ex);
            Logger.Log(LogMessageTypes.Error, msg, ex.TargetSite.Name, ex.StackTrace);
        }
        
    }
}
