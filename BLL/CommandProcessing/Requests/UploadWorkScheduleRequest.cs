using BaseDevice;

namespace CommandProcessing.Requests
{
    public class UploadWorkScheduleRequest : Request
    {
        public override byte RequestID => 0x04;

        public override byte[] MakeData(object o)
        {
            var schedule = o as WorkSchedule;
            byte stateFlag = 0;
            if (schedule.AllWeek)
            {
                stateFlag = 0b10000000;
            }
            else
            {
                if (schedule.RunInMon)
                    stateFlag |= 0b1000000;
                if (schedule.RunInTue)
                    stateFlag |= 0b100000;
                if (schedule.RunInWed)
                    stateFlag |= 0b10000;
                if (schedule.RunInThu)
                    stateFlag |= 0b1000;
                if (schedule.RunInFri)
                    stateFlag |= 0b100;
                if (schedule.RunInSat)
                    stateFlag |= 0b10;
                if (schedule.RunInSun)
                    stateFlag |= 0b1;
            }
            return new byte[]
            {
                (byte)(schedule.AroundTheClock? 0xff: schedule.StartFrom.Hours),
                (byte)(schedule.AroundTheClock? 0xff: schedule.StartFrom.Minutes),
                (byte)(schedule.AroundTheClock? 0xff: schedule.FinishTo.Hours),
                (byte)(schedule.AroundTheClock? 0xff: schedule.FinishTo.Minutes),
                stateFlag
            };
        }
    }
}
