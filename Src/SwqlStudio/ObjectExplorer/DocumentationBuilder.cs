﻿using System;
using System.Text;
using System.Windows.Forms;
using SwqlStudio.Metadata;

namespace SwqlStudio
{
    internal class DocumentationBuilder
    {
        public static Documentation Build(TreeNode node)
        {
            var data = node.Tag;
            var connectedNode = node as TreeNodeWithConnectionInfo;
            var provider = connectedNode?.Provider;

            if (provider == null)
                return Documentation.Empty;

            if (data is SwisMetaDataProvider)
                return ProviderDocumentation(provider);

            if (data is string nameSpace)
                return NamespaceDocumentation(nameSpace, node.Nodes.Count);

            if (data is Entity entity)
                return EntityDocumentation(provider, entity);

            if (data is Property property)
                return PropertyDocumentation(property);

            if (data is Verb verb)
                return VerbDocumentation(verb);

            if (data is VerbArgument verbArg)
                return VerbArgumentDocumentation(verbArg);

            return Documentation.Empty;
        }

        private static Documentation VerbArgumentDocumentation(VerbArgument verbArg)
        {
            var docs = ToToolTip(verbArg);
            return new Documentation("Verb argument", docs);
        }

        private static Documentation VerbDocumentation(Verb verb)
        {
            var docs = ToToolTip(verb);
            return new Documentation("Verb", docs);
        }

        private static Documentation EntityDocumentation(IMetadataProvider provider, Entity entity)
        {
            var docs = ToToolTip(provider.ConnectionInfo, entity);
            return new Documentation("Entity", docs);
        }

        private static Documentation NamespaceDocumentation(string nameSpace, int childsCount)
        {
            var childs = ChildsText(childsCount);
            var docs = $"Name: {nameSpace}\r\n{childs}";
            return new Documentation("Namespace", docs);
        }

        private static Documentation PropertyDocumentation(Property property)
        {
            var docs = ToToolTip(property);
            return new Documentation("Property", docs);
        }

        private static string NameAndType(ITypedMetadata metadata)
        {
            return $"Name: {metadata.Name}\r\nType: {metadata.Type}";
        }

        private static Documentation ProviderDocumentation(IMetadataProvider provider)
        {
            var documents = $@"Connection: {provider.Name}";
            return new Documentation("Database", documents);
        }

        public static string ToToolTip(VerbArgument arg)
        {
            var docs = NameAndType(arg);
            return AppendSummary(docs, arg.Summary);
        }

        public static string ToToolTip(Property property)
        {
            var docs = NameAndType(property);
            return AppendSummary(docs, property.Summary);
        }

        public static string ToToolTip(ConnectionInfo connection, Entity entity)
        {
            var builder = new StringBuilder();
            builder.Append($"Name: {entity.FullName}\r\n");
            builder.Append($"Base type: {entity.BaseType}\r\n");

            if (entity.IsIndication)
            {
                builder.Append($@"Can Subscribe: {connection.CanCreateSubscription}");
            }
            else
            {
                builder.Append($"Can Read: {entity.CanRead}\r\n");
                builder.Append($"Can Create: {entity.CanCreate}\r\n");
                builder.Append($"Can Update: {entity.CanUpdate}\r\n");
                builder.Append($"Can Delete: {entity.CanDelete}\r\n");
            }

            var summary = String.IsNullOrEmpty(entity.Summary) ? String.Empty : entity.Summary;
            builder.Append(summary);
            return builder.ToString();
        }

        public static string ToToolTip(Verb verb)
        {
            var docs = $"Name: {verb.Name}";
            return AppendSummary(docs, verb.Summary);
        }

        private static string AppendSummary(string docs, string summary)
        {
            if (!String.IsNullOrEmpty(summary))
                return $"{docs}\r\n\r\n{summary}";

            return docs;
        }

        public static string ToNodeText(string name, int childsCount)
        {
            var childs = ChildsText(childsCount);
            return $"{name} ({childs})";
        }

        private static string ChildsText(int childsCount)
        {
            var countSuffix = childsCount > 1 ? "s" : String.Empty;
            return $"{childsCount} item{countSuffix}";
        }

        public static string ToNodeText(ITypedMetadata metadata)
        {
            return $"{metadata.Name} ({metadata.Type})";
        }

        public static string ToBaseNodeText(TreeNode baseNode, int childsCount)
        {
            var entitiesSuffix = childsCount > 1 ? "ies" : "y";
            return $"{baseNode.Text} ({childsCount} derived entit{entitiesSuffix})";
        }
    }
}