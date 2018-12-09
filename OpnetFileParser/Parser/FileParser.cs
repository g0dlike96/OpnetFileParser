using System.IO;
using System.Text;

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
            this.StringBuilder.Clear();
        }

        public abstract void Parse();
        protected abstract void AppendHeaders(string timeOrigin, string timeEnd);
        protected abstract void AppendTimeWindowHeader(string timeOrigin, string timeEnd);
    }
}