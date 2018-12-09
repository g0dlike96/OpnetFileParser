using System;
using OpnetFileParser.ValueObjects;

namespace OpnetFileParser
{
    public static class Tools
    {
        public static OpnetTimeSpan CheckIfGreater(OpnetTimeSpan current, OpnetTimeSpan currentMax)
            => current.GetHigherEndDateTime(currentMax);

        public static OpnetTimeSpan CheckIfSmaller(OpnetTimeSpan current, OpnetTimeSpan currentMin)
            => current.GetSmallerStartDateTime(currentMin);

        public static string ConvertBytesToBits(string bytes)
            => (int.Parse(bytes) * 8).ToString();

        public static string CalculateValuePerSecond(OpnetTimeSpan timeInterval, string value)
        {
            var duration = timeInterval.GetTimeInterval();

            try
            {
                return (int.Parse(value) / duration).ToString();
            }
            catch (ArithmeticException)
            {
                Console.WriteLine($"{Environment.NewLine}Blad w wyliczaniu statystyk: " +
                                  $"{nameof(timeInterval)}: {timeInterval}, {nameof(value)}: {value}");
                throw;
            }
        }
    }
}