using System.Collections.Generic;

namespace Domaccurcy.Core.Models
{
    public class DomNode
    {
        public string Id { get; set; } = string.Empty;
        public string TagName { get; set; } = string.Empty;
        public Dictionary<string, string> Attributes { get; set; } = new();
        public string InnerText { get; set; } = string.Empty;
        public List<DomNode> Children { get; set; } = new();
        public List<string> CssClasses { get; set; } = new();
        public string InlineStyle { get; set; } = string.Empty;
        public string XPath { get; set; } = string.Empty;
    }
}
