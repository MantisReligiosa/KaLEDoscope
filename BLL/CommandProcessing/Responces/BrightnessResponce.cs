using BaseDevice;
using System;

namespace CommandProcessing.Responces
{
    public class BrightnessResponce : Responce<Brightness>
    {
        public override byte ResponceID => 5;

        public override Brightness Cast()
        {
            var brightness = new Brightness()
            {
                Mode = (BrightnessMode)_bytes[5],
                ManualValue = _bytes[6],
                BrightnessPeriods = new System.Collections.Generic.List<BrightnessPeriod>()
            };
            var periods = _bytes[7];
            if (periods > 0)
            {
                for (int i = 0; i < periods; i++)
                {
                    brightness.BrightnessPeriods.Add(new BrightnessPeriod
                    {
                        From = new TimeSpan(_bytes[i * 5 + 8], _bytes[i * 5 + 9], 0),
                        To = new TimeSpan(_bytes[i * 5 + 10], _bytes[i * 5 + 11], 0),
                        Value = _bytes[i * 5 + 12]
                    });
                }
            }
            return brightness;
        }
    }
}
