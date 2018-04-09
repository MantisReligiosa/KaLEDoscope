using Aggregations;

namespace KaLEDoscope
{
    public class FolderNode : NodeItem
    {
        public override string Name { get; set; }
        public Folder Folder { get; internal set; }
    }
}
