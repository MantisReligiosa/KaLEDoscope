using BaseDevice;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BaseDeviceSerialization
{
    public class SerializableBrightness
    {
        public List<SerializableBrightnessPeriod> BrightnessPeriods { get; set; }
        public ushort ManualValue { get; set; }
        public int Mode { get; set; }

        public static explicit operator SerializableBrightness(Brightness brightness)
        {
            return new SerializableBrightness
            {
                BrightnessPeriods = brightness.BrightnessPeriods.Select(p => (SerializableBrightnessPeriod)p).ToList(),
                ManualValue = brightness.ManualValue,
                Mode = (int)brightness.Mode
            };
        }

        public static explicit operator Brightness(SerializableBrightness serializableBrightness)
        {
            return new Brightness
            {
                BrightnessPeriods = serializableBrightness.BrightnessPeriods.Select(p => (BrightnessPeriod)p).ToList(),
                ManualValue = serializableBrightness.ManualValue,
                Mode = (BrightnessMode)serializableBrightness.Mode
            };
        }
    }

    public class SerializableBrightnessPeriod
    {
        public TimeSpan From { get; private set; }
        public TimeSpan To { get; private set; }
        public int Value { get; private set; }

        public static explicit operator SerializableBrightnessPeriod(BrightnessPeriod period)
        {
            return new SerializableBrightnessPeriod
            {
                From = period.From,
                To = period.To,
                Value = period.Value
            };
        }

        public static explicit operator BrightnessPeriod(SerializableBrightnessPeriod serializableBrightnessPeriod)
        {
            return new BrightnessPeriod
            {
                From = serializableBrightnessPeriod.From,
                To = serializableBrightnessPeriod.To,
                Value = serializableBrightnessPeriod.Value
            };
        }
    }
}
