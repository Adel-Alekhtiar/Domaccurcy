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

            var oldNodes = FlattenNodes(oldSnapshot.Nodes)
                .GroupBy(n => n.XPath)
                .ToDictionary(g => g.Key, g => g.ToList());
            var newNodes = FlattenNodes(newSnapshot.Nodes)
                .GroupBy(n => n.XPath)
                .ToDictionary(g => g.Key, g => g.ToList());

            // Detect removed nodes
            foreach (var (xpath, oldNodeList) in oldNodes)
            {
                if (!newNodes.TryGetValue(xpath, out var newNodeList))
                {
                    // All nodes with this xpath were removed
                    foreach (var oldNode in oldNodeList)
                    {
                        changes.Add(new DomChange
                        {
                            ChangeType = ChangeType.Removed,
                            OldNode = oldNode,
                            XPath = xpath
                        });
                    }
                }
                else
                {
                    // Some nodes might have been removed if counts differ
                    for (int i = newNodeList.Count; i < oldNodeList.Count; i++)
                    {
                        changes.Add(new DomChange
                        {
                            ChangeType = ChangeType.Removed,
                            OldNode = oldNodeList[i],
                            XPath = xpath
                        });
                    }
                }
            }

            // Detect added or modified nodes
            foreach (var (xpath, newNodeList) in newNodes)
            {
                if (!oldNodes.TryGetValue(xpath, out var oldNodeList))
                {
                    // All nodes with this xpath were added
                    foreach (var newNode in newNodeList)
                    {
                        changes.Add(new DomChange
                        {
                            ChangeType = ChangeType.Added,
                            NewNode = newNode,
                            XPath = xpath
                        });
                    }
                }
                else
                {
                    // Check for modifications and additions
                    int minCount = System.Math.Min(oldNodeList.Count, newNodeList.Count);
                    
                    // Compare existing nodes
                    for (int i = 0; i < minCount; i++)
                    {
                        if (IsModified(oldNodeList[i], newNodeList[i]))
                        {
                            changes.Add(new DomChange
                            {
                                ChangeType = ChangeType.Modified,
                                OldNode = oldNodeList[i],
                                NewNode = newNodeList[i],
                                XPath = xpath
                            });
                        }
                    }
                    
                    // Add new nodes if count increased
                    for (int i = oldNodeList.Count; i < newNodeList.Count; i++)
                    {
                        changes.Add(new DomChange
                        {
                            ChangeType = ChangeType.Added,
                            NewNode = newNodeList[i],
                            XPath = xpath
                        });
                    }
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
