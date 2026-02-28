using System.Collections.Generic;
using System.Linq;
using Domaccurcy.Core.Models;

namespace Domaccurcy.Core.Services
{
    public class ChangeDetector
    {
        public List<DomChange> Detect(DomSnapshot oldSnapshot, DomSnapshot newSnapshot)
        {
            var changes = new List<DomChange>();

            var oldNodes = FlattenNodes(oldSnapshot.Nodes).ToDictionary(n => n.XPath, n => n);
            var newNodes = FlattenNodes(newSnapshot.Nodes).ToDictionary(n => n.XPath, n => n);

            // Detect removed nodes
            foreach (var (xpath, oldNode) in oldNodes)
            {
                if (!newNodes.ContainsKey(xpath))
                {
                    changes.Add(new DomChange
                    {
                        ChangeType = ChangeType.Removed,
                        OldNode = oldNode,
                        XPath = xpath
                    });
                }
            }

            // Detect added or modified nodes
            foreach (var (xpath, newNode) in newNodes)
            {
                if (!oldNodes.TryGetValue(xpath, out var oldNode))
                {
                    changes.Add(new DomChange
                    {
                        ChangeType = ChangeType.Added,
                        NewNode = newNode,
                        XPath = xpath
                    });
                }
                else if (IsModified(oldNode, newNode))
                {
                    changes.Add(new DomChange
                    {
                        ChangeType = ChangeType.Modified,
                        OldNode = oldNode,
                        NewNode = newNode,
                        XPath = xpath
                    });
                }
            }

            return changes;
        }

        private static bool IsModified(DomNode oldNode, DomNode newNode)
        {
            if (oldNode.InnerText != newNode.InnerText) return true;
            if (oldNode.InlineStyle != newNode.InlineStyle) return true;

            if (oldNode.Attributes.Count != newNode.Attributes.Count) return true;

            foreach (var (key, value) in oldNode.Attributes)
            {
                if (!newNode.Attributes.TryGetValue(key, out var newValue) || newValue != value)
                    return true;
            }

            return false;
        }

        private static IEnumerable<DomNode> FlattenNodes(IEnumerable<DomNode> nodes)
        {
            foreach (var node in nodes)
            {
                yield return node;
                foreach (var child in FlattenNodes(node.Children))
                    yield return child;
            }
        }
    }
}
