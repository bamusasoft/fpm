using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlopManager.Services
{
    public class ValidationErrorsMessages
    {
        public const string STRING_IS_NOT_DIGIT_ERROR = "القيمة يجب ان تكون ارقام فقط";
        public const string CAN_NOT_HAVE_MORE_THAN_ONE_PRESENT_YEAR = "لا يمكن ان يكون هناك اكثر من سنة حالية في النظام";
        public const string YEAR_MUST_HAVE_AT_LEAST_ONE_SEQUENCE = "لا بد من إدخال دفعة واحدة على الأقل";
        public const string LOAN_TYPE_CODE_MUST_SUPPLIED = "أدخل رمز نوع الإلتزام";
        public const string LOAN_TYPE_DESCR_MUST_SUPPLIED = "أدخل وصف نوع الإلتزام";

        public const string LOAN_NO_MUST_SUPPLIED = "ادخل رقم الإلتزام";
        public const string MEMBER_MUST_SUUPLIED = "حدد المستحق";
        public const string LOAN_TYPE_MUST_SUPPLIED = "حدد نوع الإلتزام";
        public const string YEAR_MUST_SUPPLIED = "حدد سنة الإلتزام";
        public const string SEQUENCE_MUST_SUPPLIED = "حدد الدفعة";
        public const string LOAN_DESCR_MUST_SUPPLIED = "ادخل وصف مختصر";
        public const string AMOUNT_MUST_SUPPLIED = "ادخل المبلغ";
        public const string PAYMENT_NO_MUST_SUPPLIED = "ادخل رقم الصرفية";
        public const string SEQUENCE_MUST_NOT_ALREADY_PAID = "الصرفية المحددة سبق صرفها";
        public  const string PAST_PAYMENTS_MUST_POSTED = "توجد دفعات سابقة لم ترحل";

        public const string PAY_METHOD_NOT_SELECTED = "يجب تحديد طريقة الدفع";

        public const string INVALID_DATE = "التاريخ غير صحيح";




    }
}
