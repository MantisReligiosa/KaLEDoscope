using BaseDevice;

namespace CommandProcessing.Requests
{
    public class UploadBrightnessRequest : Request
    {
        public override byte RequestID => 0x05;

        public override byte[] MakeData(object o)
        {
            var brightness = o as Brightness;
            var mode = brightness.Mode;
            var result = new byte[3];
            if (mode == BrightnessMode.Scheduled)
            {
                result = new byte[3 + brightness.BrightnessPeriods.Count * 5];
            }
            result[0] = (byte)(brightness.Mode);
            result[1] = (byte)(brightness.Mode == BrightnessMode.Manual ? brightness.ManualValue : 0);
            result[2] = (byte)(brightness.Mode == BrightnessMode.Scheduled ? brightness.BrightnessPeriods.Count : 0);
            if (brightness.Mode == BrightnessMode.Scheduled)
            {
                for (int i = 0; i < brightness.BrightnessPeriods.Count; i++)
                {
                    result[3 + i * 5] = (byte)(brightness.BrightnessPeriods[i].From.Hours);
                    result[4 + i * 5] = (byte)(brightness.BrightnessPeriods[i].From.Minutes);
                    result[5 + i * 5] = (byte)(brightness.BrightnessPeriods[i].To.Hours);
                    result[6 + i * 5] = (byte)(brightness.BrightnessPeriods[i].To.Minutes);
                    result[7 + i * 5] = (byte)(brightness.BrightnessPeriods[i].Value);
                }
            }
            return result;
        }
    }
}