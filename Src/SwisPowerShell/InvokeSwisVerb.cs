using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;
using SolarWinds.InformationService.Contract2;

namespace SwisPowerShell
{
    public class InvokeSwisVerb : BaseSwisCmdlet
    {
        private static readonly XmlSerializer propertyBagSerializer = new XmlSerializer(typeof(PropertyBag));

        public string EntityName { get; set; }

        public string Verb { get; set; }


        public List<object> Arguments { get; set; }

        protected override void InternalProcessRecord()
        {
            var serializedArguments = Arguments.Select(SerializeArgument).ToArray();
            CheckConnection();
            XmlElement result = null;
            DoWithExceptionReporting(() => result = SwisConnection.Invoke(EntityName, Verb, serializedArguments));

        }

        internal static XmlElement SerializeArgument(object arg)
        {
            var doc = new XmlDocument();

            using (XmlWriter writer = doc.CreateNavigator().AppendChild())
            {
                writer.WriteStartDocument();
                if (arg is IDictionary)
                {
                    propertyBagSerializer.Serialize(writer, PropertyBagFromDictionary(arg));
                }
                else
                {
                    var dcs = new DataContractSerializer(arg?.GetType() ?? typeof(object));
                    // ReSharper disable AssignNullToNotNullAttribute
                    dcs.WriteObject(writer, arg);
                    // ReSharper restore AssignNullToNotNullAttribute
                }

                writer.WriteEndDocument();
            }

            return doc.DocumentElement;
        }
    }
}
