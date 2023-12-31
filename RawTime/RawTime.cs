﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace RawTimeCore
{
    /// <summary>
    /// Manages time in a raw format independently from timezones or other parameters.
    /// </summary>
    public class RawTime : IEquatable<RawTime?>
    {
        #region Time properties

        /// <summary>
        /// Gets the year.
        /// </summary>
        public int Year { get; } = 1;

        /// <summary>
        /// Gets the month.
        /// </summary>
        public int Month { get; } = 1;

        /// <summary>
        /// Gets the day.
        /// </summary>
        public int Day { get; } = 1;

        /// <summary>
        /// Gets the hour in 24 military format.
        /// </summary>
        public int Hour { get; } = 0;

        /// <summary>
        /// Gets the minute.
        /// </summary>
        public int Minute { get; } = 0;

        /// <summary>
        /// Gets the second.
        /// </summary>
        public int Second { get; } = 0;

        /// <summary>
        /// Gets the millisecond.
        /// </summary>
        public int Millisecond { get; } = 0;

        #endregion Time properties

        #region Constructors

        /// <summary>
        /// Initializes a new instance.
        /// The time will be 01/Jan/0001 - 00:00:00.
        /// </summary>
        public RawTime() : this(1, 1, 1, 0, 0, 0, 0) { }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public RawTime(int year, int month, int day) : this(year, month, day, 0, 0, 0, 0) { }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public RawTime(int year, int month, int day, int hour, int minute) : this(year, month, day, hour, minute, 0, 0) { }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public RawTime(int year, int month, int day, int hour, int minute, int second) : this(year, month, day, hour, minute, second, 0) { }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public RawTime(int year, int month, int day, int hour, int minute, int second, int millisecond)
        {
            if (year < 1 || year > 9999) throw new ArgumentOutOfRangeException(nameof(year));
            if (month < 1 || month > 12) throw new ArgumentOutOfRangeException(nameof(month));
            if (day < 1 || day > DateTime.DaysInMonth(year, month)) throw new ArgumentOutOfRangeException(nameof(day));
            if (hour < 0 || hour > 23) throw new ArgumentOutOfRangeException(nameof(hour));
            if (minute < 0 || minute > 59) throw new ArgumentOutOfRangeException(nameof(minute));
            if (second < 0 || second > 59) throw new ArgumentOutOfRangeException(nameof(second));
            if (millisecond < 0 || millisecond > 999) throw new ArgumentOutOfRangeException(nameof(millisecond));

            Year = year;
            Month = month;
            Day = day;
            Hour = hour;
            Minute = minute;
            Second = second;
            Millisecond = millisecond;
        }

        #endregion Constructors

        #region Converting from and to long

        /// <summary>
        /// Gets a new instance converting a raw long truncated to the day.
        /// </summary>
        public static RawTime FromRawLongDay(long raw) => FromRawLong(raw * 1000000000);

        /// <summary>
        /// Gets a new instance converting a raw long truncated to the minute.
        /// </summary>
        public static RawTime FromRawLongMinute(long raw) => FromRawLong(raw * 100000);

        /// <summary>
        /// Gets a new instance converting a raw long truncated to the second.
        /// </summary>
        public static RawTime FromRawLongSecond(long raw) => FromRawLong(raw * 1000);

        /// <summary>
        /// Gets a new instance converting a raw long.
        /// </summary>
        public static RawTime FromRawLong(long raw)
        {
            int millisecond = (int)(raw % 1000);
            raw /= 1000;
            int second = (int)(raw % 100);
            raw /= 100;
            int minute = (int)(raw % 100);
            raw /= 100;
            int hour = (int)(raw % 100);
            raw /= 100;
            int day = (int)(raw % 100);
            raw /= 100;
            int month = (int)(raw % 100);
            raw /= 100;
            int year = (int)raw;

            return new RawTime(year, month, day, hour, minute, second, millisecond);
        }

        /// <summary>
        /// <para>Encodes this instance into a long, copying the values positionally.</para>
        /// <para>Format: yyyyMMddHHmmssfff</para>
        /// </summary>
        public long EncodeToRawLong() => EncodeToRawLongSecond() * 1000 + Millisecond;

        /// <summary>
        /// <para>Encodes this instance into a long, copying the values positionally, truncated to the second.</para>
        /// <para>Format: yyyyMMddHHmmss</para>
        /// </summary>
        public long EncodeToRawLongSecond() => EncodeToRawLongMinute() * 100 + Second;

        /// <summary>
        /// <para>Encodes this instance into a long, copying the values positionally, truncated to the minute.</para>
        /// <para>Format: yyyyMMddHHmm</para>
        /// </summary>
        public long EncodeToRawLongMinute()
        {
            long encodedTime = EncodeToRawLongDay();
            encodedTime *= 100;
            encodedTime += Hour;
            encodedTime *= 100;
            encodedTime += Minute;
            return encodedTime;
        }

        /// <summary>
        /// <para>Encodes this instance into a long, copying the values positionally, truncated to the day.</para>
        /// <para>Format: yyyyMMdd</para>
        /// </summary>
        public long EncodeToRawLongDay()
        {
            long encodedTime = Year;
            encodedTime *= 100;
            encodedTime += Month;
            encodedTime *= 100;
            encodedTime += Day;
            return encodedTime;
        }

        #endregion Converting from and to long

        #region Converting from and to DateTime

        /// <summary>
        /// Gets a new instance converting from a <see cref="DateTime"/>.
        /// </summary>
        public static RawTime FromDateTime(DateTime time) => new(time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second, time.Millisecond);

        /// <summary>
        /// Converts this instance to a <see cref="DateTime"/>.
        /// </summary>
        public DateTime ToDateTime() => new(Year, Month, Day, Hour, Minute, Second, Millisecond);

        #endregion Converting from and to DateTime

        #region Truncations and rounding

        /// <summary>
        /// Gets a new instance truncated to the day.
        /// </summary>
        public RawTime Date() => new(Year, Month, Day, 0, 0, 0, 0);

        /// <summary>
        /// Gets a new instance with only hour, minute, second, and millisecond. (Date will be 1/Jan/1)
        /// </summary>
        public RawTime TimeOfDay() => new(1, 1, 1, Hour, Minute, Second, Millisecond);

        /// <summary>
        /// Gets a new instance with only hour, minute, and second. (Date will be 1/Jan/1)
        /// </summary>
        public RawTime TimeOfDaySeconds() => new(1, 1, 1, Hour, Minute, Second, 0);

        /// <summary>
        /// Gets a new instance with only hour and minute. (Date will be 1/Jan/1)
        /// </summary>
        public RawTime TimeOfDayMinutes() => new(1, 1, 1, Hour, Minute, 0, 0);

        #region Rounding

        /// <summary>
        /// Tipo di arrotondamento.
        /// </summary>
        public enum RoundingType
        {
            /// <summary>
            /// Mathematical rounding.
            /// </summary>
            Math,

            /// <summary>
            /// Floor rounding.
            /// </summary>
            Floor,

            /// <summary>
            /// Ceiling rounding.
            /// </summary>
            Ceiling,
        };

        /// <summary>
        /// Gets a new instance rounded at the minute using the specified interval in minutes and rounding type.
        /// The interval will be clamped between 1 and 60 minutes.
        /// </summary>
        public RawTime RoundMinute(int roundingInterval, RoundingType roundingType)
        {
            roundingInterval = Math.Min(Math.Max(roundingInterval, 1), 60);
            int minute = Minute;
            int roundedMinute = Minute;

            if (minute % roundingInterval != 0)
            {
                switch (roundingType)
                {
                    case RoundingType.Math:
                        roundedMinute = minute + roundingInterval / 2;
                        roundedMinute -= (roundedMinute % roundingInterval);
                        break;
                    case RoundingType.Floor:
                        roundedMinute = minute - (minute % roundingInterval);
                        break;
                    case RoundingType.Ceiling:
                        roundedMinute = minute + (roundingInterval - (minute % roundingInterval));
                        break;
                    default:
                        roundedMinute = minute;
                        break;
                }
            }
            
            return AddMinutes(roundedMinute - minute);
        }

        #endregion

        #endregion Truncations

        #region Operations

        /// <summary>
        /// Gets a new instance adding the specified amount of years to this instance.
        /// </summary>
        public RawTime AddYears(int years) => FromDateTime(ToDateTime().AddYears(years));

        /// <summary>
        /// Gets a new instance adding the specified amount of months to this instance.
        /// </summary>
        public RawTime AddMonths(int months) => FromDateTime(ToDateTime().AddMonths(months));

        /// <summary>
        /// Gets a new instance adding the specified amount of days to this instance.
        /// </summary>
        public RawTime AddDays(int days) => FromDateTime(ToDateTime().AddDays(days));

        /// <summary>
        /// Gets a new instance adding the specified amount of hours to this instance.
        /// </summary>
        public RawTime AddHours(int hours) => FromDateTime(ToDateTime().AddHours(hours));

        /// <summary>
        /// Gets a new instance adding the specified amount of minutes to this instance.
        /// </summary>
        public RawTime AddMinutes(int minutes) => FromDateTime(ToDateTime().AddMinutes(minutes));

        /// <summary>
        /// Gets a new instance adding the specified amount of seconds to this instance.
        /// </summary>
        public RawTime AddSeconds(int seconds) => FromDateTime(ToDateTime().AddSeconds(seconds));

        /// <summary>
        /// Gets a new instance adding the specified amount of milliseconds to this instance.
        /// </summary>
        public RawTime AddMilliseconds(int milliseconds) => FromDateTime(ToDateTime().AddMilliseconds(milliseconds));

        /// <summary>
        /// Gets the difference between two instances as a <see cref="TimeSpan"/>.
        /// It does the operation left - right, and returns the difference.
        /// </summary>
        public static TimeSpan Difference(RawTime left, RawTime right) => left.ToDateTime() - right.ToDateTime();

        /// <summary>
        /// Gets the difference between two instances as a <see cref="TimeSpan"/>.
        /// </summary>
        public static TimeSpan operator -(RawTime left, RawTime right) => Difference(left, right);

        /// <summary>
        /// Returns the mimimum between two instances.
        /// </summary>
        public static RawTime Min(RawTime left, RawTime right) => left <= right ? left : right;

        /// <summary>
        /// Returns the maximum between two instances.
        /// </summary>
        public static RawTime Max(RawTime left, RawTime right) => left >= right ? left : right;

        #endregion Operations

        #region Utils

        /// <summary>
        /// Gets a new instance from the current time on this computer.
        /// </summary>
        public static RawTime GetCurrentTime() => FromDateTime(DateTime.Now);

        /// <summary>
        /// Gets the day of week of this instance.
        /// </summary>
        public DayOfWeek DayOfWeek() => ToDateTime().DayOfWeek;

        /// <summary>
        /// Gets the day of year of this instance.
        /// </summary>
        public int DayOfYear() => ToDateTime().DayOfYear;

        /// <summary>
        /// Gets the second of the day of this instance.
        /// </summary>
        public long SecondOfDay() => (MinuteOfDay() * 60) + Second;

        /// <summary>
        /// Gets the minute of the day of this instance.
        /// </summary>
        public long MinuteOfDay() => (Hour * 60) + Minute;

        #endregion Utils

        #region ToString and Parse

#if NET7_0_OR_GREATER
        /// <summary>
        /// Converts this instance to a string with the specified format.
        /// </summary>
        public string ToString([StringSyntax(StringSyntaxAttribute.DateTimeFormat)] string? format) => ToDateTime().ToString(format);
#else
        /// <summary>
        /// Converts this instance to a string with the specified format.
        /// </summary>
        public string ToString(string? format) => ToDateTime().ToString(format);
#endif

        /// <summary>
        /// Converts this instance to a string with the default format.
        /// </summary>
        public override string ToString() => ToString(null);

        /// <summary>
        /// Converts the specified string to a new <see cref="RawTime"/> instance, using the specified format provider.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="FormatException"/>
        public static RawTime Parse(string time, IFormatProvider? provider = null) => FromDateTime(DateTime.Parse(time, provider));

        #endregion ToString and Parse

        #region Comparisons

        /// <inheritdoc/>
        public override bool Equals(object? obj) => Equals(obj as RawTime);

        /// <inheritdoc/>
        public bool Equals(RawTime? other) => other is not null && FastEncodeToRawLong() == other.FastEncodeToRawLong();

        /// <inheritdoc/>
        public override int GetHashCode() => HashCode.Combine(Year, Month, Day, Hour, Minute, Second, Millisecond);

        /// <summary>
        /// Indicates if the left operand is less than the right.
        /// </summary>
        public static bool operator <(RawTime left, RawTime right) => left.FastEncodeToRawLong() < right.FastEncodeToRawLong();

        /// <summary>
        /// Indicates if the left operand is greater than the right.
        /// </summary>
        public static bool operator >(RawTime left, RawTime right) => left.FastEncodeToRawLong() > right.FastEncodeToRawLong();

        /// <summary>
        /// Indicates if the left operand is less than or equal to the right.
        /// </summary>
        public static bool operator <=(RawTime left, RawTime right) => left.FastEncodeToRawLong() <= right.FastEncodeToRawLong();

        /// <summary>
        /// Indicates if the left operand is greater than or equal to the right.
        /// </summary>
        public static bool operator >=(RawTime left, RawTime right) => left.FastEncodeToRawLong() >= right.FastEncodeToRawLong();

        /// <summary>
        /// Indicates if the left operand is equal to the right.
        /// </summary>
        public static bool operator ==(RawTime? left, RawTime? right) => EqualityComparer<RawTime>.Default.Equals(left, right);

        /// <summary>
        /// Indicates if the left operand is different to the right.
        /// </summary>
        public static bool operator !=(RawTime? left, RawTime? right) => !(left == right);

        #region Fast conversion to long

        /// <summary>
        /// Converts to a long in a fast way with bit operators.
        /// It generates a long that is useful for comparisons but is different from the long generated with <see cref="EncodeToRawLong"/>.
        /// </summary>
        private long FastEncodeToRawLong()
        {
            long encoded = Year;
            encoded <<= 4;
            encoded |= (uint)Month;
            encoded <<= 6;
            encoded |= (uint)Day;
            encoded <<= 5;
            encoded |= (uint)Hour;
            encoded <<= 6;
            encoded |= (uint)Minute;
            encoded <<= 6;
            encoded |= (uint)Second;
            encoded <<= 10;
            encoded |= (uint)Millisecond;
            return encoded;
        }

        #endregion

        #endregion
    }
}
