
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using Cafeine.Services.FilenameParser;
using DBreeze.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Windows.Storage;

namespace Cafeine.Tests
{
    [TestClass]
    public class FilenameParserTest
    {
        private List<TestCase> testcases;
        private TestContext testContextInstance;
        public TestContext TestContext {
            get => testContextInstance; 
            set => testContextInstance = value;
        }

        [TestInitialize]
        public async Task PopulateDatabase()
        {
            Uri path = new Uri("ms-appx:///data.json");
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(path);
            testcases = JsonConvert.DeserializeObject<List<TestCase>>(File.ReadAllText(file.Path));
        }


        [TestMethod]
        public void UniqueArrayContainsNullTest()
        {
            bool IsSuccess = true;
            foreach(var testcase in testcases)
            {
                var parser = new CafeineFilenameParser(testcase.FileName);
                parser.TryCreateUniqueArrays(out string[] keys,true);

                if(keys.Any(string.IsNullOrWhiteSpace) || keys.Length == 0 )
                {
                    IsSuccess = false;
                    TestContext.WriteLine("\n-------------------------------");
                    TestContext.WriteLine($"Filename : {testcase.FileName}");
                    TestContext.WriteLine($"Result :");
                    for(int i = 0; i<keys.Length;i++) TestContext.WriteLine(">"+keys[i]+"<");
                }
            }
            Assert.IsTrue(IsSuccess);
        }

        [TestMethod]
        public void UniqueArraysPerformance()
        {
            var non_simd = new Stopwatch();
            non_simd.Start();
            foreach (var test_nonSIMD in testcases)
            {
                var parser = new CafeineFilenameParser(test_nonSIMD.FileName);
                parser.TryCreateUniqueArrays(out string[] keys, false);
            }
            non_simd.Stop();

            // SIMD 
            var simd = new Stopwatch();
            simd.Start();
            foreach (var test_SIMD in testcases)
            {
                var parser = new CafeineFilenameParser(test_SIMD.FileName);
                parser.TryCreateUniqueArrays(out string[] keys, true);
            }
            simd.Stop();

            TestContext.WriteLine("\n-------------------------------");
            TestContext.WriteLine($"SIMD      : {simd.ElapsedTicks} ticks");
            TestContext.WriteLine($"Non-SIMD  : {non_simd.ElapsedTicks} ticks");
            TestContext.WriteLine("\n-------------------------------");

            Assert.IsTrue(true);
        }

        [TestMethod]
        public void UniqueArraysEqualityTest()
        {
            foreach (var testcase in testcases)
            {
                var parser = new CafeineFilenameParser(testcase.FileName);
                parser.TryCreateUniqueArrays(out string[] keys_simd,true);
                parser.TryCreateUniqueArrays(out string[] keys_nonsimd,false);

                if (!keys_simd.SequenceEqual(keys_nonsimd)) {
                    TestContext.WriteLine("\n-------------------------------");
                    TestContext.WriteLine($"Filename\t:{testcase.FileName}");
                    TestContext.WriteLine($"SIMD\t\t: {string.Join('|',keys_simd)}");
                    TestContext.WriteLine($"Non-SIMD\t: {string.Join('|',keys_nonsimd)}");
                    TestContext.WriteLine($"Count\t\t: SIMD: {keys_simd.Length}, LegaSIMD: {keys_nonsimd.Length}");
                    TestContext.WriteLine("-------------------------------");
                }
            }

            Assert.IsTrue(true);
        }
    }
    public class TestCase
    {
        [JsonProperty("file_name")]
        public string FileName { get; set; }

        [JsonProperty("ignore")]
        public bool Ignore { get; set; }

        [JsonProperty("results")]
        public Dictionary<string, object> Results { get; set; }
    }
}
