using PixelBoardDevice.DomainObjects;
using System.Linq;

namespace PixelBoardDevice.Serialization
{
    public class SerializableProgram
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public int Period { get; set; }
        public object Zones { get; set; }

        public static explicit operator SerializableProgram(Program program)
        {
            return new SerializableProgram
            {
                Id = program.Id,
                Name = program.Name,
                Order = program.Order,
                Period = program.Period,
                Zones = program.Zones.Select(z => (SeriazableZone)z).ToList()
            };
        }
    }
}