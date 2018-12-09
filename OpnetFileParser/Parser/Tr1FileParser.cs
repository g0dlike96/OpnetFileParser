using System.IO;
using OpnetFileParser.Enums;

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

        }

        protected override void AppendHeaders(string timeOrigin, string timeEnd)
        {
            var headers = Headers.HeadersForTr1FileFormat;

            headers.ForEach(h => StringBuilder.AppendLine(h));
        }

        protected override void AppendTimeWindowHeader(string timeOrigin, string timeEnd)
        {
            throw new System.NotImplementedException();
        }
    }
}
