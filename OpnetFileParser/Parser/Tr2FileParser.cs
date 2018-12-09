using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OpnetFileParser.Enums;
using OpnetFileParser.ValueObjects;

using static OpnetFileParser.Enums.DatagramFieldsIndexes;

namespace OpnetFileParser.Parser
{
    public sealed class Tr2FileParser : FileParser
    {
        public Tr2FileParser(StreamWriter outputFileWriter, StreamReader inputFileReader)
        :base(outputFileWriter, inputFileReader)
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
            var headers = Headers.HeadersForTr2FileFormat;

            base.AppendTimeWindowHeader(timeOrigin, timeEnd);
            headers.ForEach(header => StringBuilder.AppendLine(header));
        }

        protected override string ParseLine(ICollection<string> fields)
        {
            var timeInterval = new OpnetTimeSpan
                (startTimeString: fields.ElementAt(StartTimeIndex), endTimeString: fields.ElementAt(EndTimeIndex));

            var startTime = timeInterval.OpnetStartDateTimeString;
            var endTime = timeInterval.OpnetEndDateTimeString;
            var sourceAddres = fields.ElementAt(SourceAddressIndex);
            var destinationAddress = fields.ElementAt(DestinationAddressIndex);
            var sourcePort = fields.ElementAt(SourcePortIndex);
            var destinationPort = fields.ElementAt(DestinationPortIndex);
            var protocol = fields.ElementAt(ProtocolIndex);
            var packets = fields.ElementAt(PacketsCountIndex);
            var bytes = fields.ElementAt(BytesCountIndex);

            var packetsPerSecond = base.CalculateValuePerSecond(timeInterval, packets);
            var bitsPerSecond = base.CalculateValuePerSecond(timeInterval, base.ConvertBytesToBits(bytes));

            return string.Join(",", sourceAddres, destinationAddress, protocol, sourcePort,
                 destinationPort, startTime, endTime, bitsPerSecond, packetsPerSecond);
        }

        //todo: decide what to do with it
        private static SummaryAvgValues TakeAverageTransmissionSpeed(string fileText)
        {
            var summaryString = string.Empty;

            using (var reader = new StringReader(fileText))
            {
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Contains("Summary"))
                    {
                        summaryString = reader.ReadToEnd();
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(summaryString))
            {
                throw new ArgumentException($"Passed argument {nameof(fileText)} doesn't have summary section!");
            }

            var summaryProperties = summaryString.Split(',');

            return new SummaryAvgValues
                (
                    averageBytesPerSecond: summaryProperties.ElementAt(8), 
                    averagePacketsPerSecond: summaryProperties.ElementAt(9)
                );
        }
    }
}