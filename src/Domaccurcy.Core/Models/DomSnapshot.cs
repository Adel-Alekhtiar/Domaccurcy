using System;
using System.Collections.Generic;

namespace Domaccurcy.Core.Models
{
    public class DomSnapshot
    {
        public string Url { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public List<DomNode> Nodes { get; set; } = new();
        public string HtmlContent { get; set; } = string.Empty;
        public string CssContent { get; set; } = string.Empty;
    }
}
