using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using System.Web.Mvc;

namespace PayrollWeb.Utility
{
    public class InputBoxBinaryChoiceAttribute : ValidationAttribute, IClientValidatable
    {
        private const string _customErrorMessage = "{0} or {1} must have value.";
       

        public string OtherProperty { get; private set; }


        public InputBoxBinaryChoiceAttribute(string otherProperty)
            : base(_customErrorMessage)
        {
            if (otherProperty == null)
            {
                throw new ArgumentNullException("otherProperty");
            }

            this.OtherProperty = otherProperty;            
        }        

        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, this.OtherProperty);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            PropertyInfo otherPropertyInfo = validationContext.ObjectType.GetProperty(OtherProperty);   
            
            if (otherPropertyInfo == null)
            {
                return new ValidationResult(String.Format(CultureInfo.CurrentCulture, "Could not find a property named {0}.", OtherProperty));
            }

            object otherPropertyValue = otherPropertyInfo.GetValue(validationContext.ObjectInstance, null);

            if (value == null && otherPropertyValue == null)
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }

            if (!string.IsNullOrWhiteSpace(value.ToString().Trim()) && otherPropertyValue !=null)
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }

            return null;
        }

        public static string FormatPropertyForClientValidation(string property)
        {
            if (property == null)
            {
                throw new ArgumentException("Value cannot be null or empty.", "property");
            }
            return "*." + property;
        }       

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {           
            yield return new ModelClientValidationInputBoxBinaryChoice(FormatErrorMessage(metadata.DisplayName), FormatPropertyForClientValidation(this.OtherProperty));
        }
    }
}