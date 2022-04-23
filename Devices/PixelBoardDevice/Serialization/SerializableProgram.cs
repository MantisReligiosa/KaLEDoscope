using PixelBoardDevice.DomainObjects;
using System.Collections.Generic;
using System.Linq;

namespace PixelBoardDevice.Serialization
{
    public class SerializableProgram
    {
        public byte Id { get; set; }
        public string Name { get; set; }
        public byte Order { get; set; }
        public ushort Period { get; set; }
        public List<SeriazableZone> Zones { get; set; }

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

        public static explicit operator Program(SerializableProgram serializableProgram)
        {
            return new Program
            {
                Id = serializableProgram.Id,
                Name = serializableProgram.Name,
                Order = serializableProgram.Order,
                Period = serializableProgram.Period,
                Zones = serializableProgram.Zones.Select(z => (Zone)z).ToList()
            };
        }
    }
}
