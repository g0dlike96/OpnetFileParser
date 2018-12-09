using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using OpnetFileParser.ValueObjects;

using static OpnetFileParser.Enums.DatagramFieldsIndexes;

namespace OpnetFileParser.Parser
{
    public abstract class CsvFileParser
    {
        protected StreamWriter OutputFileWriter { get; }
        protected StreamReader InputFileReader { get; }
        protected StringBuilder StringBuilder { get; }

        protected CsvFileParser(StreamWriter outputFileWriter, StreamReader inputFileReader)
        {
            this.OutputFileWriter = outputFileWriter;
            this.InputFileReader = inputFileReader;
            this.StringBuilder = new StringBuilder();
        }

        public void Cleanup()
        {
            this.OutputFileWriter.Close();
            this.OutputFileWriter.Dispose();
            this.InputFileReader.Close();
            this.InputFileReader.Dispose();
            this.StringBuilder.Clear();
        }

        //todo: decide what to do with it
        protected SummaryAvgValues TakeAverageTransmissionSpeed(string fileText)
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

        protected OpnetTimeSpan GetBorderTime
        (
            string fileText,
            Func<OpnetTimeSpan, OpnetTimeSpan, OpnetTimeSpan> condition,
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
                        (
                            startTimeString: fields.ElementAt(StartTimeIndex),
                            endTimeString: fields.ElementAt(EndTimeIndex)
                        );

                        resultDateTime = condition.Invoke(currentDateTime, resultDateTime);
                    }

                    ++counter;
                }
            }

            return resultDateTime;
        }

        protected void ParsingLoop(string fileToParseString)
        {
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
                        this.StringBuilder.AppendLine(parsedLine);

                        Console.WriteLine($"Processing line: {lineCount}");
                    }
                    ++lineCount;
                }
            }
        }

        protected virtual void AppendTimeWindowHeader(string timeOrigin, string timeEnd)
        {
            StringBuilder.AppendLine($"time_origin: {timeOrigin}");
            StringBuilder.AppendLine($"time_end: {timeEnd}");
            StringBuilder.AppendLine();
        }

        public abstract void Parse();

        protected abstract string ParseLine(ICollection<string> fields);
        protected abstract void AppendHeaders(string timeOrigin, string timeEnd);
    }
}