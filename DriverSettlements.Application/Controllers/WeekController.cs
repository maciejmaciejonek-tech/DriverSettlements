using DriverSettlements.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverSettlements.App.Controllers
{
    public class WeekController
    {
        public Week CurrentWeek { get; private set; }
        public int DeltaWeek { get; private set; }

        public WeekController()
        {
            DeltaWeek = 0;
        }

        public T GetWeek<T>(int deltaWeek, bool setAsCurrentWeek = true)
        {
            var yearNumber = DateTime.Now.Year;
            var weekNumber = Common.Helpers.Date.GetWeekNumber(DateTime.Now) - deltaWeek;
            if (weekNumber < 1)
            {
                while (weekNumber < 1)
                {
                    yearNumber--;
                    weekNumber += 52;
                }
            }
            else if (weekNumber > 52)
            {
                while (weekNumber > 52)
                {
                    yearNumber++;
                    weekNumber -= 52;
                }
            }
            var firstDayOfWeek = Common.Helpers.Date.GetFirstDateOfWeek(yearNumber, weekNumber);
            var week = Activator.CreateInstance(typeof(T)) as Week;
            week.YearNumber = yearNumber;
            week.WeekNumber = weekNumber;
            week.DateFrom = firstDayOfWeek;
            week.DateTo = firstDayOfWeek.AddDays(6);
            if (setAsCurrentWeek)
            {
                CurrentWeek = week;
                DeltaWeek = deltaWeek;
            }
            return (T)(object)week;
        }

        public int PreviousDeltaWeek => DeltaWeek + 1;

        public int NextDeltaWeek => DeltaWeek - 1;

        public bool CanGoToNextWeek => DeltaWeek > 0;

        public void ClearController()
        {
            DeltaWeek = 0;
        }
    }
}
