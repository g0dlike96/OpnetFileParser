using OpnetFileParser.ValueObjects;

namespace OpnetFileParser
{
    public static class Tools
    {
        public static OpnetTimeSpan CheckIfGreater(OpnetTimeSpan current, OpnetTimeSpan currentMax)
            => current.GetHigherEndDateTime(currentMax);

        public static OpnetTimeSpan CheckIfSmaller(OpnetTimeSpan current, OpnetTimeSpan currentMin)
            => current.GetSmallerStartDateTime(currentMin);

    }
}