using System.Collections.ObjectModel;

namespace KaLEDoscope
{
    public class FolderNode
    {
        public string Name { get; set; }

        public ObservableCollection<DeviceNode> Devices { get; set; } = new ObservableCollection<DeviceNode>();
        public ObservableCollection<FolderNode> Folders { get; set; } = new ObservableCollection<FolderNode>();
        public ObservableCollection<AggregationNode> Aggregations { get; set; } = new ObservableCollection<AggregationNode>();
    }
}
