
using System;
using Cafeine.Services.FilenameParser;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cafeine.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var w = new CafeineFilenameParser("wwwe [we]");
            w.TryGetArrayIdentifier(out string[] keys);
            Assert.AreNotEqual(keys.Length, 0);
        }
    }
}
