namespace OpnetFileParser.ValueObjects
{
    public class SummaryAvgValues : ValueObject<SummaryAvgValues>
    {
        public string AverageBitsPerSecond { get; }
        public string AveragePacketsPerSecond { get; }

        public SummaryAvgValues(string averageBytesPerSecond, string averagePacketsPerSecond)
        {
            this.AverageBitsPerSecond = (int.Parse(averageBytesPerSecond)*8).ToString();
            this.AveragePacketsPerSecond = averagePacketsPerSecond;
        }

        protected override bool EqualsCore(SummaryAvgValues other)
            => this.AverageBitsPerSecond == other.AverageBitsPerSecond
               && this.AveragePacketsPerSecond == other.AveragePacketsPerSecond;

        protected override int GetHashCodeCore()
            => (this.AverageBitsPerSecond.GetHashCode() * 397) 
               ^ this.AveragePacketsPerSecond.GetHashCode();
    }
}