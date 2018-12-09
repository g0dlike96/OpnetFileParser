using System;
using System.Globalization;

namespace OpnetFileParser.ValueObjects
{
    public sealed class OpnetTimeSpan : ValueObject<OpnetTimeSpan>
    {
        public string OpnetStartDateTimeString { get; }
        public string OpnetEndDateTimeString { get; }
        public DateTime OpnetStartDateTime { get; }
        public DateTime OpnetEndDateTime { get; }
        private CultureInfo UsCultureInfo { get; }

        public OpnetTimeSpan(string startTimeString, string endTimeString)
            :this(new CultureInfo("en-US", false))
        {
            this.OpnetStartDateTimeString = FromNfdumpToOpnetDateString(startTimeString);
            this.OpnetEndDateTimeString = FromNfdumpToOpnetDateString(endTimeString);
            this.OpnetStartDateTime = FromNfdumpDateStringToOpnetDate(startTimeString);
            this.OpnetEndDateTime = FromNfdumpDateStringToOpnetDate(endTimeString);
        }

        private OpnetTimeSpan(CultureInfo cultureInfo)
        {
            UsCultureInfo = cultureInfo;
        }

        public int GetTimeInterval()
            => (this.OpnetEndDateTime - this.OpnetStartDateTime).Seconds;

        public OpnetTimeSpan GetSmallerStartDateTime(OpnetTimeSpan other)
            => this.OpnetStartDateTime < other.OpnetStartDateTime ? this : other;

        public OpnetTimeSpan GetHigherEndDateTime(OpnetTimeSpan other)
            => this.OpnetEndDateTime > other.OpnetEndDateTime ? this : other;

        public override string ToString()
        {
            return $"Flow start: {OpnetStartDateTime}, Flow end: {OpnetEndDateTime}";
        }

        protected override bool EqualsCore(OpnetTimeSpan other)
            => this.OpnetEndDateTimeString == other.OpnetEndDateTimeString
               && this.OpnetStartDateTimeString == other.OpnetStartDateTimeString
               && this.OpnetStartDateTime == other.OpnetStartDateTime
               && this.OpnetEndDateTime == other.OpnetEndDateTime;

        protected override int GetHashCodeCore()
        {
            unchecked
            {
               return (this.OpnetStartDateTimeString.GetHashCode() * 397)
                    ^ (this.OpnetEndDateTimeString.GetHashCode() * 397)
                    ^ (this.OpnetStartDateTime.GetHashCode() * 397)
                    ^ (this.OpnetEndDateTime.GetHashCode());
            }
        }              

        private string FromNfdumpToOpnetDateString(string nfdumpDateTime)
        {
            var parsedDateTime = FromNfdumpDateStringToOpnetDate(nfdumpDateTime).ToString(this.UsCultureInfo);

            return parsedDateTime.Substring(0, parsedDateTime.Length - 3);
        }  

        private DateTime FromNfdumpDateStringToOpnetDate(string opnetDateTimestring)
            => DateTime.ParseExact
              (
                 opnetDateTimestring,
                 "yyyy-MM-dd HH:mm:ss",
                 this.UsCultureInfo
              );
    }
}
