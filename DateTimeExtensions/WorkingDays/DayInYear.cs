#region License

// 
// Copyright (c) 2011-2012, João Matos Silva <kappy@acydburne.com.pt>
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace DateTimeExtensions.WorkingDays
{
    public class DayInYear
    {
        public DayInYear(int month, int day)
            : this(month, day, new GregorianCalendar())
        {
        }

        public DayInYear(int month, int day, Calendar calendar, bool skipLeapMonth=false)
        {
            this.Month = month;
            this.Day = day;
            this.Calendar = calendar;
            this.SkipLeapMonth = skipLeapMonth;
        }

        public int Day { get; private set; }
        public int Month { get; private set; }
        public Calendar Calendar { get; private set; }
        public bool SkipLeapMonth { get; private set; }

        public DateTime GetDayOnYear(int year)
        {
            var midDayOnGregoryanCalendar = new DateTime(year, 7, 1);
            int calendarYear = Calendar.GetYear(midDayOnGregoryanCalendar);
            int calendarMonth = Month;
            int leapMonth = Calendar.GetLeapMonth(calendarYear);
            if (SkipLeapMonth && leapMonth > 0 && leapMonth <= Month)
            {
                calendarMonth += 1;
            }
            var dayInstance = new DateTime(calendarYear, calendarMonth, Day, Calendar);

            //check if the instance day falls on previous year on Gregorian calendar.
            // the instance should fall between year and year + 1. 
            // TODO: This smells a bit. Ensure this is true for all types of calendars.
            if (dayInstance.Year < midDayOnGregoryanCalendar.Year)
            {
                dayInstance = Calendar.AddYears(dayInstance, 1);
                leapMonth = Calendar.GetLeapMonth(Calendar.GetYear(dayInstance));
                if (SkipLeapMonth && leapMonth>0 && leapMonth <= Calendar.GetMonth(dayInstance))
                {
                    dayInstance.AddMonths(1);
                }
            }

            return dayInstance;
        }

        public bool IsTheSameDay(DateTime day)
        {
            int calendarMonth = Month;
            int leapMonth = Calendar.GetLeapMonth(day.Year);
            if (SkipLeapMonth && leapMonth > 0 && leapMonth <= Month)
            {
                calendarMonth += 1;
            }
            var thisDayInYear = new DateTime(Calendar.GetYear(day), calendarMonth, Day, Calendar);
            return thisDayInYear == day.Date;
        }
    }
}