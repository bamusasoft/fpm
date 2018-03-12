using System;
using System.Diagnostics;
using System.Text;
using Microsoft.Office.Interop.Word;

namespace FlopManager.Services.Helpers
{
    public static class Helper
    {
        //static PeriodSetting _currentPeriod;
        /// <summary>
        /// Gets all exception messages in the supplied exception.
        /// </summary>
        /// <param name="ex">The exception.</param>
        /// <returns></returns>
        public static ExceptionEncapsulation ProcessExceptionMessages(Exception ex)
        {
            return GetAllExceptionMessages(ex, new ExceptionEncapsulation());
        }

        private static ExceptionEncapsulation GetAllExceptionMessages(Exception exception, ExceptionEncapsulation encapsulation)
        {
            if (exception != null)
            {
                StackTrace s = new StackTrace(exception);
                string detailedMsg =$"{Environment.NewLine} Exception: {exception.GetType().Name}{Environment.NewLine} At method: {s.GetFrame(0).GetMethod().Name}{Environment.NewLine} Details: {exception.Message}";
                string userMag = $"{Environment.NewLine}  {exception.Message}";
                encapsulation.DetialsMsg = encapsulation.DetialsMsg + detailedMsg;
                encapsulation.UserMsg = encapsulation.UserMsg + userMag;
                GetAllExceptionMessages(exception.InnerException, encapsulation);
            }
            return encapsulation;
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
        
        /// <summary>
        /// Get the current Period.
        /// </summary>
        /// <returns>Current Period if set, otherwise null.</returns>
        //private static PeriodSetting CurrentPeriod
        //{
        //    get
        //    {
        //        if (_currentPeriod == null)
        //        {
        //            try
        //            {
        //                using (IUnitOfWork unit = new UnitOfWork())
        //                {

        //                    var result = unit.PeriodSettings.Query(x => x.PeriodStatus.Id == 2).Single();
        //                    _currentPeriod = new PeriodSetting()
        //                    {
        //                        Id = result.Id,
        //                        StartDate = result.StartDate,
        //                        EndDate = result.EndDate,
        //                        YearPart = result.YearPart,
        //                        Loans = result.Loans,
        //                        PaymentInstructions = result.PaymentInstructions,
        //                        Payments = result.Payments,
        //                        PeriodStatus = result.PeriodStatus,
        //                        StatusId = result.StatusId,
        //                    };
        //                }
        //            }
        //            catch
        //            {
        //                return _currentPeriod;
        //            }
        //        }
        //        return _currentPeriod;
        //    }
        //}

        public static string GenerateLoanNo(string currentYear, string maxNo)
        {
            if (string.IsNullOrEmpty(currentYear)) throw new ArgumentNullException(nameof(currentYear));
            string currentYearPortion = currentYear.Substring(2, 2);

            if (!string.IsNullOrEmpty(maxNo))
            {
                string dbYearPortion = maxNo.Substring(0, 2);
                if (dbYearPortion.Equals(currentYearPortion))
                {
                    string incrementedPortion = maxNo.Substring(3, 3);
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
            string exceptionMsg = "";//ProcessExceptionMessages(e);
            string general = "";
           //Properties.Resources.DbErrorMsg;
            return general + Environment.NewLine + exceptionMsg;
        }
        public static void LogAndShow(Exception ex)
        {
            string msg = "";//ProcessExceptionMessages(ex);
            Logger.Log(LogMessageTypes.Error, msg, ex.TargetSite.Name, ex.StackTrace);
            //ShowMessage(msg);
        }
        public static void LogOnly(Exception ex)
        {
            string msg = "";//ProcessExceptionMessages(ex);
            Logger.Log(LogMessageTypes.Error, msg, ex.TargetSite.Name, ex.StackTrace);
        }

        public static bool UserConfirmed(string msg)
        {
            return false;
        }

        public static void ShowMessage(string msg)
        {
            
        }
        
    }
}
