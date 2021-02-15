using System.Collections.Generic;

namespace SwisPowerShell
{
    public class GetSwisObject : BaseSwisCmdlet
    {
        public string Uri { get; set; }

        protected override void InternalProcessRecord()
        {
            CheckConnection();
            Dictionary<string, object> obj = null;
            DoWithExceptionReporting(() => obj = SwisConnection.Read(Uri));

        }
    }
}
