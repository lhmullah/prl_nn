using System.Web.Mvc;

namespace PayrollWeb.Utility
{
    public class ModelClientValidationNumericGreaterThanRule : ModelClientValidationRule
    {
        public ModelClientValidationNumericGreaterThanRule(string errorMessage, object other, bool allowEquality)
        {
            ErrorMessage = errorMessage;
            ValidationType = "numericgreaterthan";
            ValidationParameters["other"] = other;
            ValidationParameters["allowequality"] = allowEquality;
        }
    }
}