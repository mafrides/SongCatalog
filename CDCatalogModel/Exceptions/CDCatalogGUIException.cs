using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace CDCatalogModel
{
    public class CDCatalogGUIException : CDCatalogException
    {
        public CDCatalogGUIException(Exception ex, string message = "")
            : this(ex, message, false) { }

        protected CDCatalogGUIException(
            Exception innerException,
            string message = "",
            bool publicConstructor = false,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] int callerLineNumber = 0)
            : base(innerException, message, false, callerMemberName, callerFilePath, callerLineNumber) { }
    }
}
