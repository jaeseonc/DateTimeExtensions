using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using DateTimeExtensions.WorkingDays;
using DateTimeExtensions.WorkingDays.CultureStrategies;

namespace DateTimeExtensions.Tests
{
    [TestFixture]
    public class KRHolidaysTests
    {
        [Test]
        public void can_get_strateryies()
        {
            var dateTimeCulture = new WorkingDayCultureInfo("ko-KR");
            var holidaysStrategy = dateTimeCulture.LocateHolidayStrategy(dateTimeCulture.Name);
            Assert.AreEqual(typeof(KO_KRHolidayStrategy), holidaysStrategy.GetType());
            var workingDaysStrategy = dateTimeCulture.LocateWorkingDayOfWeekStrategy(dateTimeCulture.Name);
            Assert.AreEqual(typeof(DefaultWorkingDayOfWeekStrategy), workingDaysStrategy.GetType());
        }

        [Test]
        public void holiday_days_span()
        {
            var dateTimeCulture = new WorkingDayCultureInfo("ko-KR");
            var holiday = KO_KRHolidayStrategy.Seolnal;
            var startDay = holiday.GetInstance(2015);

            //Seolnal 2015
            DateTime day = new DateTime(2015, 2, 18);
            do
            {
                Assert.IsFalse(dateTimeCulture.IsWorkingDay(day), day.ToString() + " shouldn't be a working day");
                day = day.AddDays(1);
            } while (day <= new DateTime(2015, 2, 20));

            //Seolnal 2016 (with substitute holiday)
            day = new DateTime(2016, 2, 7);
            do
            {
                Assert.IsFalse(dateTimeCulture.IsWorkingDay(day), day.ToString() + " shouldn't be a working day");
                day = day.AddDays(1);
            } while (day <= new DateTime(2015, 2, 10));

            //First non-holiday after Chuseok 2009 (substitute holiday should not apply yet)
            day = new DateTime(2009, 10, 5);
            Assert.IsTrue(dateTimeCulture.IsWorkingDay(day), day.ToString() + " should be a working day");

            //Chuseok 2015 (with substitute holiday for overlapping with a sunday)
            day = new DateTime(2015, 9, 26);
            do
            {
                Assert.IsFalse(dateTimeCulture.IsWorkingDay(day), day.ToString() + " shouldn't be a working day");
                day = day.AddDays(1);
            } while (day <= new DateTime(2015, 9, 29));

            //Chuseok 2028 (with substitute holiday for overlapping over Gaecheonjeol)
            day = new DateTime(2028, 10, 5);
            Assert.IsFalse(dateTimeCulture.IsWorkingDay(day), day.ToString() + " shouldn't be a working day");

            //Chuseok 2036 (with substitute holiday for overlapping over a sunday and Gaecheonjeol, should add only a single day)
            day = new DateTime(2036, 10, 6);
            Assert.IsFalse(dateTimeCulture.IsWorkingDay(day), day.ToString() + " shouldn't be a working day");
            day = new DateTime(2036, 10, 7);
            Assert.IsTrue(dateTimeCulture.IsWorkingDay(day), day.ToString() + " should be a working day");
        }

        [Test]
        public void can_identify_Seolnal()
        {
            var holiday = KO_KRHolidayStrategy.Seolnal;
            var dateOnGregorian = new DateTime(2015, 2, 19);
            TestHoliday(holiday, dateOnGregorian);
        }

        [Test]
        public void can_identify_SeokgaTansilil()
        {
            var holiday = KO_KRHolidayStrategy.SeokgaTansinil;
            var dateOnGregorian = new DateTime(2015, 5, 25);
            TestHoliday(holiday, dateOnGregorian);
        }

        [Test]
        public void can_identify_Chuseok()
        {
            var holiday = KO_KRHolidayStrategy.Chuseok;
            var dateOnGregorian = new DateTime(2015, 9, 27);
            TestHoliday(holiday, dateOnGregorian);
        }

        [Test]
        public void can_identify_LeapMonth()
        {
            var holiday = KO_KRHolidayStrategy.Chuseok;
            var dateOnGregorian = new DateTime(2009, 10, 3);
            TestHoliday(holiday, dateOnGregorian);
        }

        private void TestHoliday(Holiday holiday, DateTime dateOnGregorian)
        {
            var holidayInstance = holiday.GetInstance(dateOnGregorian.Year);
            Assert.IsTrue(holidayInstance.HasValue);
            Assert.AreEqual(dateOnGregorian, holidayInstance.Value);
            Assert.IsTrue(holiday.IsInstanceOf(dateOnGregorian));
        }
        
        [Test]
        public void get_year_holidays_of_korea()
        {
            var koreanlWorkingDayCultureInfo = new WorkingDayCultureInfo("ko-KR");
            var today = new DateTime(2015, 2, 5);
            var holidays = today.AllYearHolidays(koreanlWorkingDayCultureInfo);

            foreach (DateTime holidayDate in holidays.Keys)
            {
                var holiday = holidays[holidayDate];
                Assert.IsTrue(holidayDate.IsWorkingDay(koreanlWorkingDayCultureInfo) == false,
                    "holiday {0} shouln't be working day in Korea", holiday.Name);
            }
        }
    }
}