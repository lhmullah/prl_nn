using System.Collections.Generic;

namespace PayrollWeb.Models
{
    public interface IProcessResult
    {
        ProcessType GetProcessTypeType { get; }
        List<string> GetErrors { get; }
        bool ErrorOccured { get; set; }
        void AddToErrorList(string errorMsg);
        void AddCompletedResultObjects(object lstSuccessfullObjects);
        object GetCompletedResultObjects();
    }
}