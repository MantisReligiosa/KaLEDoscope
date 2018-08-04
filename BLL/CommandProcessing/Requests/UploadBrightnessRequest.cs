using BaseDevice;
using System.Collections.Generic;
using System.Linq;

namespace CommandProcessing.Requests
{
    public class UploadBrightnessRequest : Request
    {
        public override byte RequestID => 0x05;

        public override byte[] MakeData(object o)
        {
            var brightness = o as Brightness;
            var result = new List<byte>
            {
                (byte)(brightness.Mode),
                (byte)(brightness.Mode == BrightnessMode.Manual ? brightness.ManualValue : 0),
                (byte)(brightness.Mode == BrightnessMode.Scheduled ? brightness.BrightnessPeriods.Count : 0)
            };
            if (brightness.Mode == BrightnessMode.Scheduled)
            {
                result.AddRange(brightness.BrightnessPeriods.SelectMany(period => new[]
                    {
                    (byte)(period.From.Hours),
                    (byte)(period.From.Minutes),
                    (byte)(period.To.Hours),
                    (byte)(period.To.Minutes),
                    (byte)(period.Value)
                    }));
            }
            return result.ToArray();
        }
    }
}
