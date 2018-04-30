using BaseDevice;

namespace KaLEDoscope.Serialization
{
    public class SerializableAggregation
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public static explicit operator Aggregation(SerializableAggregation aggregation)
        {
            return new Aggregation
            {
                Id = aggregation.Id,
                Name = aggregation.Name
            };
        }

        public static explicit operator SerializableAggregation(Aggregation aggregation)
        {
            return new SerializableAggregation
            {
                Id = aggregation.Id,
                Name = aggregation.Name
            };
        }
    }
}
