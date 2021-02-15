using System.Collections.Generic;

namespace SwisPowerShell
{
    public class RemoveSwisObject : BaseSwisCmdlet
    {
        public string Uri { get; set; }

        private readonly List<string> uris = new List<string>();

        protected override void InternalProcessRecord()
        {
            uris.Add(Uri);
        }

        protected void EndProcessing()
        {
            CheckConnection();
            DoWithExceptionReporting(() => SwisConnection.BulkDelete(uris.ToArray()));
        }
    }
}
