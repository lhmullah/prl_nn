using System.Web.Mvc;

namespace PayrollWeb.Utility
{
    public class ModelClientValidationNumericLessThanRule : ModelClientValidationRule
    {
        public ModelClientValidationNumericLessThanRule(string errorMessage, object other, bool allowEquality)
        {
            ErrorMessage = errorMessage;
            ValidationType = "numericlessthan";
            ValidationParameters["other"] = other;
            ValidationParameters["allowequality"] = allowEquality;
        }
    }
}