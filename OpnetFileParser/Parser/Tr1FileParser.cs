using System.Collections.Generic;
using System.IO;
using System.Linq;
using OpnetFileParser.Enums;
using OpnetFileParser.ValueObjects;

using static OpnetFileParser.Enums.DatagramFieldsIndexes;

namespace OpnetFileParser.Parser
{
    public sealed class Tr1FileParser : FileParser
    {
        public Tr1FileParser(StreamWriter outputFileWriter, StreamReader inputFileReader)
            : base(outputFileWriter, inputFileReader)
        {
        }

        public override void Parse()
        {
            var fileToParseString = InputFileReader.ReadToEnd();
            var maxDateTime = new OpnetTimeSpan
                (startTimeString: "9999-12-30 00:00:00", endTimeString: "1000-12-30 00:00:00");
            var minDateTime = new OpnetTimeSpan
                (startTimeString: "1000-12-30 00:00:00", endTimeString: "1000-12-30 00:00:00");

            var timeOrigin = base.GetTime(fileToParseString, Tools.CheckIfSmaller, maxDateTime)
                .OpnetStartDateTimeString;
            var timeEnd = base.GetTime(fileToParseString, Tools.CheckIfGreater, minDateTime)
                .OpnetEndDateTimeString;

            AppendHeaders(timeOrigin, timeEnd);

            base.ParsingLoop(fileToParseString);

            this.OutputFileWriter.WriteLine(this.StringBuilder.ToString());
        }

        protected override void AppendHeaders(string timeOrigin, string timeEnd)
        {
            var headers = Headers.HeadersForTr1FileFormat;

            base.AppendTimeWindowHeader(timeOrigin, timeEnd);
            headers.ForEach(h => StringBuilder.AppendLine(h));
        }

        protected override string ParseLine(ICollection<string> fields)
        {
            var timeInterval = new OpnetTimeSpan
            (
                fields.ElementAt(StartTimeIndex),
                fields.ElementAt(EndTimeIndex)
            );

            var sourceAddress = fields.ElementAt(SourceAddressIndex);
            var destinationAddress = fields.ElementAt(DestinationAddressIndex);
            var timeWindow = GetTimeWindowString(timeInterval);
            var packets = fields.ElementAt(PacketsCountIndex);
            var bytes = fields.ElementAt(BytesCountIndex);

            var packetsPerSecond = base.CalculateValuePerSecond(timeInterval, packets);
            var bitsPerSecond = base.CalculateValuePerSecond(timeInterval, base.ConvertBytesToBits(bytes));

            return string.Join(",", sourceAddress, destinationAddress, timeWindow, packetsPerSecond, bitsPerSecond);
        }

        private static string GetTimeWindowString(OpnetTimeSpan timeInterval)
        {
            var start = timeInterval.OpnetStartDateTimeString;
            var end = timeInterval.OpnetEndDateTimeString;

            return string.Concat("[", start, "-", end, "]");
        }
    }
}
