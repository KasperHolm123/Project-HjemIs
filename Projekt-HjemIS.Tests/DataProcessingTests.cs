using Microsoft.VisualStudio.TestTools.UnitTesting;
using Projekt_HjemIS.Services;
using System;

namespace Projekt_HjemIS.Tests
{
    [TestClass]
    public class DataProcessingTests
    {
        [TestMethod]
        public void ParseRecord_ReturnsCorrectlyParsedArray()
        {
            // Arrange
            var dataProcessor = new DataProcessorService();
            var testData = "00401010004002 028 L2018092921101654København V         ";
            var recordType = testData.Substring(0, 3);
            var expected = new string[]
            {
                "004",
                "0101",
                "0004",
                "002 ",
                "028 ",
                "L",
                "201809292110",
                "1654",
                "København V         "
            };

            // Act
            var actual = dataProcessor.ParseRecord(testData, recordType);

            // Assert
            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
