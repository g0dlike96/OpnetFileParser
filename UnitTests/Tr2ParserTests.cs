using System.IO;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpnetFileParser.Parser;

namespace UnitTests
{
    [TestClass]
    [TestCategory("Testing parser for Tr2 file format")]
    public class Tr2ParserTests
    {
        private const string FilePath = @"D:\Pulpit\Inzynierka\File_Parser\testParse.txt";

        // todo: useless test, replace with real parse, headers are only placeholder
        [TestMethod]
        public void InitializeHeadersSimpleTest_WhenCalled_JustAddsHeadersFromEnum()
        {
            TestInitialize();


            var streamWriter = new StreamWriter(FilePath);
            var tr2FileParser = new Tr2FileParser(streamWriter, null);

            try
            {
                tr2FileParser.Parse();
            }
            finally
            {
                tr2FileParser.Cleanup();
            }

            var streamLength = File.Open(FilePath, FileMode.Open).Length;

            Assert.IsTrue(streamLength > 0);
        }

        private static void TestInitialize()
        {
            File.Delete(FilePath);
            Thread.Sleep(1000);
        }
    }
}