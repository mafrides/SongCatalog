using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace CDCatalogModel
{
    public class CDCatalogException : ApplicationException
    {
        public string Caller { get; private set; }
        public string CallerFilePath { get; private set; }
        public int CallerLineNumber { get; private set; }

        public CDCatalogException(Exception ex, string message = "")
            : this(ex, message, false) { }
       
        protected CDCatalogException(
            Exception innerException,
            string message = "",
            bool publicConstructor = false,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] int callerLineNumber = 0)
            : base(message, innerException)
        {
            Caller = callerMemberName;
            CallerFilePath = callerFilePath;
            CallerLineNumber = callerLineNumber;
        }
    }
}
