using BaseDevice;

namespace KaLEDoscope
{
    public class FolderNode : NodeItem
    {
        public override string Name { get; set; }
        public Folder Folder { get; set; }
    }
}
