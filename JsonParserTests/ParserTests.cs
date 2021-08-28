using Microsoft.VisualStudio.TestTools.UnitTesting;
using JsonParser;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace JsonParser.Tests
{
    [TestClass()]
    public class ParserTests
    {
        [TestMethod()]
        public void ProcessTest()
        {
            var jsonStr = File.ReadAllText("f:/json.txt");
            var json = JsonParser.Parser.Process(jsonStr);
        }
    }
}