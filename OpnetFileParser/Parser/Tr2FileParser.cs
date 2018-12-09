using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OpnetFileParser.Enums;
using OpnetFileParser.ValueObjects;

using static OpnetFileParser.Enums.DatagramFields;

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

            var timeOrigin = GetTime(fileToParseString, Tools.CheckIfSmaller, maxDateTime)
                .OpnetStartDateTimeString;
            var timeEnd = GetTime(fileToParseString, Tools.CheckIfGreater, minDateTime)
                .OpnetEndDateTimeString;

            AppendHeaders(timeOrigin, timeEnd);            

            using (var reader = new StringReader(fileToParseString))
            {
                string line;
                var lineCount = 0;

                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Contains("Summary"))
                    {
                        break;
                    }

                    if (lineCount > 1)
                    {
                        var fields = line.Split(',').ToList();

                        var parsedLine = ParseLine(fields);
                        StringBuilder.AppendLine(parsedLine);

                        Console.WriteLine($"Processing line: {lineCount}");
                    }
                    ++lineCount;
                }
            }

            this.OutputFileWriter.WriteLine(this.StringBuilder.ToString());
        }

        protected override void AppendHeaders(string timeOrigin, string timeEnd)
        {
            var headers = Headers.HeadersForTr2FileFormat;

            AppendTimeWindowHeader(timeOrigin, timeEnd);
            headers.ForEach(h => StringBuilder.AppendLine(h));
        }

        protected override void AppendTimeWindowHeader(string timeOrigin, string timeEnd)
        {
            StringBuilder.AppendLine($"time_origin: {timeOrigin}");
            StringBuilder.AppendLine($"time_end: {timeEnd}");
            StringBuilder.AppendLine();
        }

        private static OpnetTimeSpan GetTime
            (
                string fileText,
                Func<OpnetTimeSpan, OpnetTimeSpan, OpnetTimeSpan>condition, 
                OpnetTimeSpan dateSearchinitializer
            )
        {
            var resultDateTime = dateSearchinitializer;

            using (var reader = new StringReader(fileText))
            {
                string line;
                var counter = 0;

                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Contains("Summary"))
                    {
                        break;
                    }

                    if (counter > 1)
                    {
                        var fields = line.Split(',');
                        var currentDateTime = new OpnetTimeSpan
                            (startTimeString:fields.ElementAt(0), endTimeString: fields.ElementAt(1));

                        resultDateTime = condition.Invoke(currentDateTime, resultDateTime);
                    }

                    ++counter;
                }
            }

            return resultDateTime;
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

        private static string ParseLine(ICollection<string> fields)
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

            var packetsPerSecond = CalculateValuePerSecond(timeInterval, packets);
            var bytesPerSecond = CalculateValuePerSecond(timeInterval, bytes);

            return string.Join(",", sourceAddres, destinationAddress, protocol,
                sourcePort, destinationPort, startTime, endTime, bytesPerSecond, packetsPerSecond);
        }

        private static string CalculateValuePerSecond(OpnetTimeSpan timeInterval, string value)
        {
            var duration = timeInterval.GetTimeInterval();

            try
            {
                return (int.Parse(value) / duration).ToString();
            }
            catch (ArithmeticException)
            {
                Console.WriteLine("Blad w wyliczaniu statystyk: " +
                                  $"{nameof(timeInterval)}: {timeInterval}, {nameof(value)}: {value}");
                throw;
            }
        }
    }
}