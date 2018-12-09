namespace OpnetFileParser.ValueObjects
{
    public class SummaryAvgValues : ValueObject<SummaryAvgValues>
    {
        public string AverageBytesPerSecond { get; }
        public string AveragePacketsPerSecond { get; }

        public SummaryAvgValues(string averageBytesPerSecond, string averagePacketsPerSecond)
        {
            this.AverageBytesPerSecond = averageBytesPerSecond;
            this.AveragePacketsPerSecond = averagePacketsPerSecond;
        }

        protected override bool EqualsCore(SummaryAvgValues other)
            => this.AverageBytesPerSecond == other.AverageBytesPerSecond
               && this.AveragePacketsPerSecond == other.AveragePacketsPerSecond;

        protected override int GetHashCodeCore()
            => (this.AverageBytesPerSecond.GetHashCode() * 397) 
               ^ this.AveragePacketsPerSecond.GetHashCode();
    }
}