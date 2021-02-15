using System.Collections;

namespace SwisPowerShell
{
    public class NewSwisObject : BaseSwisCmdlet
    {
        public string EntityType { get; set; }

        public Hashtable Properties { get; set; }

        protected override void InternalProcessRecord()
        {
            CheckConnection();
            string uri = null;
            DoWithExceptionReporting(() => uri = SwisConnection.Create(EntityType, PropertyBagFromHashtable(Properties)));

        }
    }
}
