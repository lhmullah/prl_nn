using System.Collections.Generic;

namespace PayrollWeb.Models
{
    public class AllowanceProcessResult : IProcessResult
    {
        private readonly ProcessType processType;
        private readonly List<string> errors;
        private bool errorFound;
        private object _lstOfSuccessfulObj;

        public AllowanceProcessResult(ProcessType processType)
        {
            this.processType = processType;
            errors= new List<string>();
        }

        public ProcessType GetProcessTypeType
        {
            get
            {
                return processType;
            }
        }

        public List<string> GetErrors
        {
            get
            {
                return errors;
            }
        }

        public void AddToErrorList(string errorMsg)
        {
            errors.Add(errorMsg);
        }

        public bool ErrorOccured
        {
           get { return errorFound; }
           set { this.errorFound = value; }
        }

        public void AddCompletedResultObjects(object lstSuccessfullObjects)
        {
            this._lstOfSuccessfulObj = lstSuccessfullObjects;
        }

        public object GetCompletedResultObjects()
        {
            return _lstOfSuccessfulObj;
        }
    }

    public enum ProcessType
    {
        ALLOWANCE=1,
        DEDUCTION=2,
        SALARY =3
    }
    
}