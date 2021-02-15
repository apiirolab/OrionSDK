using System.Collections.Generic;

namespace SwisPowerShell
{
    public class GetSwisObject : BaseSwisCmdlet
    {
        public string Uri { get; set; }

        protected override void InternalProcessRecord()
        {
            Dictionary<string, object> obj = null;
            CheckConnection();
            DoWithExceptionReporting(() => obj = SwisConnection.Read(Uri));

        }
    }
}
