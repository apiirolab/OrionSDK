using System.Collections;

namespace SwisPowerShell
{
    public class NewSwisObject : BaseSwisCmdlet
    {
        public Hashtable Properties { get; set; }
        
        public string EntityType { get; set; }

        protected override void InternalProcessRecord()
        {
            CheckConnection();
            string uri = null;
            DoWithExceptionReporting(() => uri = SwisConnection.Create(EntityType, PropertyBagFromHashtable(Properties)));

        }
    }
}
