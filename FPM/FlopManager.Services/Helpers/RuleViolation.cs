namespace FlopManager.Services.Helpers
{
    public class RuleViolation
    {
        public string ErrorMessage { get; private set; }
        public string PropertyName { get; private set; }
        public RuleViolation(string errorMessage)
            : this(errorMessage, null)
        { }
        
        public RuleViolation(string errorMessage, string propertyName)
        {
            ErrorMessage = errorMessage;
            PropertyName = propertyName;
        }
    }
}
