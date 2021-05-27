using System.Web.Mvc;

namespace PayrollWeb.Utility
{
    public class ModelClientValidationDateGreaterThanRule : ModelClientValidationRule
    {
        public ModelClientValidationDateGreaterThanRule(string errorMessage, object other, bool allowEquality)
        {
            ErrorMessage = errorMessage;
            ValidationType = "dategreaterthan";
            ValidationParameters["other"] = other;
            ValidationParameters["allowequality"] = allowEquality;
        }
    }
}