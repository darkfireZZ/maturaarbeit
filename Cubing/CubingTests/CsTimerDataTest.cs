using Cubing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;

namespace CubingTests
{
    [TestClass]
    public class CsTimerDataTest
    {
        [TestMethod]
        public void ReadWriteTest()
        {
            var collection = new List<CsTimerData>();
            string testCasePath = "../../../csTimerDataTestCase.csv";
            string resultPath = "../../../csTimerDataTestResult.csv";
            CsTimerData.ReadCsTimerData(testCasePath, data => collection.Add(data));
            CsTimerData.WriteCsTimerData(resultPath, collection);

            using StreamReader
                testCaseReader = new StreamReader(new FileStream(testCasePath, FileMode.Open)),
                resultReader = new StreamReader(new FileStream(resultPath, FileMode.Open));
            string expectedLine, actualLine;
            int line = 0;
            while ((expectedLine = testCaseReader.ReadLine()) != null && (actualLine = resultReader.ReadLine()) != null)
                Assert.AreEqual(expectedLine, actualLine, "problem on line number " + line++ + " (0-indexed)");
        }
    }
}