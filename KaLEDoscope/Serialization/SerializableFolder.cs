using BaseDevice;

namespace KaLEDoscope.Serialization
{
    public class SerializableFolder
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public static explicit operator Folder(SerializableFolder folder)
        {
            return new Folder
            {
                Id = folder.Id,
                Name = folder.Name
            };
        }

        public static explicit operator SerializableFolder(Folder folder)
        {
            return new SerializableFolder
            {
                Id = folder.Id,
                Name = folder.Name
            };
        }
    }
}
