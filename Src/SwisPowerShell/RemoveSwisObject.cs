using System.Collections.Generic;

namespace SwisPowerShell
{
    public class RemoveSwisObject : BaseSwisCmdlet
    {
        private readonly List<string> uris = new List<string>();

        public string Uri { get; set; }


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
