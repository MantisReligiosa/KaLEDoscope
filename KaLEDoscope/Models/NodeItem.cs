using System.Collections.ObjectModel;

namespace KaLEDoscope
{
    public abstract class NodeItem
    {
        public virtual string Name { get; set; }

        public void AddChildNode(NodeItem nodeItem)
        {
            Nodes.Add(nodeItem);
            nodeItem.Parent = this;
        }

        public ObservableCollection<NodeItem> Nodes { get; set; } = new ObservableCollection<NodeItem>();
        public NodeItem Parent { get; set; }
    }
}
