using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using OpnetFileParser.ValueObjects;

using static OpnetFileParser.Enums.DatagramFieldsIndexes;

namespace OpnetFileParser.Parser
{
    public abstract class FileParser
    {
        protected StreamWriter OutputFileWriter { get; }
        protected StreamReader InputFileReader { get; }
        protected StringBuilder StringBuilder { get; }

        protected FileParser(StreamWriter outputFileWriter, StreamReader inputFileReader)
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

        protected string ConvertBytesToBits(string bytes)
            => (int.Parse(bytes) * 8).ToString();

        protected virtual OpnetTimeSpan GetTime
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

        protected virtual void ParsingLoop(string fileToParseString)
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

        protected virtual string CalculateValuePerSecond(OpnetTimeSpan timeInterval, string value)
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

        public abstract void Parse();

        protected abstract string ParseLine(ICollection<string> fields);
        protected abstract void AppendHeaders(string timeOrigin, string timeEnd);
    }
}