using BaseDevice;
using Extensions;
using System;

namespace CommandProcessing.Responces
{
    public class WorkScheduleResponce : Responce<WorkSchedule>
    {
        public override byte ResponceID => 4;

        public override WorkSchedule Cast()
        {
            var startHH = _bytes[5];
            var startMM = _bytes[6];
            var endHH = _bytes[7];
            var endMM = _bytes[8];
            var statusByte = _bytes[9];
            var aroundTheClock = (startHH == 0xff && startMM == 0xff && endHH == 0xff && endMM == 0xff);
            return new WorkSchedule()
            {
                AroundTheClock = aroundTheClock,
                StartFrom = !aroundTheClock ? new TimeSpan(startHH, startMM, 0) : new TimeSpan(),
                FinishTo = !aroundTheClock ? new TimeSpan(endHH, endMM, 0) : new TimeSpan(),
                AllWeek = statusByte.GetBit(0),
                RunInMon = statusByte.GetBit(1),
                RunInTue = statusByte.GetBit(2),
                RunInWed = statusByte.GetBit(3),
                RunInThu = statusByte.GetBit(4),
                RunInFri = statusByte.GetBit(5),
                RunInSat = statusByte.GetBit(6),
                RunInSun = statusByte.GetBit(7)
            };
        }
    }
}
