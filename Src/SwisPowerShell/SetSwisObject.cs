using System.Collections;
using System.Collections.Generic;

namespace SwisPowerShell
{
    public class SetSwisObject : BaseSwisCmdlet
    {
        public string Uri { get; set; }

        public Hashtable Properties { get; set; }

        private readonly List<string> uris = new List<string>();

        protected override void InternalProcessRecord()
        {
            uris.Add(Uri);
        }

        protected void EndProcessing()
        {
            CheckConnection();
            DoWithExceptionReporting(() => SwisConnection.BulkUpdate(uris.ToArray(), PropertyBagFromHashtable(Properties)));
        }
    }
}
