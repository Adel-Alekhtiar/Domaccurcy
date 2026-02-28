using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using Domaccurcy.Core.Models;

namespace Domaccurcy.Core.Services
{
    public class DomExtractor
    {
        public List<DomNode> Extract(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var root = doc.DocumentNode;
            var nodes = new List<DomNode>();

            foreach (var child in root.ChildNodes)
            {
                var node = BuildNode(child, "");
                if (node != null)
                    nodes.Add(node);
            }

            return nodes;
        }

        private DomNode? BuildNode(HtmlNode htmlNode, string parentXPath)
        {
            if (htmlNode.NodeType == HtmlNodeType.Comment)
                return null;

            if (htmlNode.NodeType == HtmlNodeType.Text)
            {
                var text = htmlNode.InnerText.Trim();
                if (string.IsNullOrEmpty(text))
                    return null;

                return new DomNode
                {
                    TagName = "#text",
                    InnerText = text,
                    XPath = parentXPath + "/#text"
                };
            }

            var xpath = string.IsNullOrEmpty(parentXPath)
                ? "/" + htmlNode.Name
                : parentXPath + "/" + htmlNode.Name;

            var domNode = new DomNode
            {
                TagName = htmlNode.Name,
                XPath = xpath,
                InnerText = htmlNode.InnerText.Trim()
            };

            // Extract attributes
            foreach (var attr in htmlNode.Attributes)
            {
                domNode.Attributes[attr.Name] = attr.Value;

                if (attr.Name == "id")
                    domNode.Id = attr.Value;
                else if (attr.Name == "class")
                    domNode.CssClasses = attr.Value.Split(' ', System.StringSplitOptions.RemoveEmptyEntries).ToList();
                else if (attr.Name == "style")
                    domNode.InlineStyle = attr.Value;
            }

            // Recursively process children
            foreach (var child in htmlNode.ChildNodes)
            {
                var childNode = BuildNode(child, xpath);
                if (childNode != null)
                    domNode.Children.Add(childNode);
            }

            return domNode;
        }
    }
}
