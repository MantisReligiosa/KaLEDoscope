using BaseDevice;
using SmartTechnologiesM.Base.Extensions;
using System;

namespace CommandProcessing.Responces
{
#warning Нужны тесты
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
                AllWeek = statusByte.Bits()[7],
                RunInMon = statusByte.Bits()[6],
                RunInTue = statusByte.Bits()[5],
                RunInWed = statusByte.Bits()[4],
                RunInThu = statusByte.Bits()[3],
                RunInFri = statusByte.Bits()[2],
                RunInSat = statusByte.Bits()[1],
                RunInSun = statusByte.Bits()[0]
            };
        }
    }
}
