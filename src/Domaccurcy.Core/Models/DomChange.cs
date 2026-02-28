namespace Domaccurcy.Core.Models
{
    public enum ChangeType
    {
        Added,
        Removed,
        Modified
    }

    public class DomChange
    {
        public ChangeType ChangeType { get; set; }
        public DomNode? OldNode { get; set; }
        public DomNode? NewNode { get; set; }
        public string XPath { get; set; } = string.Empty;
    }
}
