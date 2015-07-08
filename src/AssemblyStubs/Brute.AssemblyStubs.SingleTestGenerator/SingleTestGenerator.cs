using Brute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brute.AssemblyStubs.SingleTestGenerator
{
    public class SingleTestGenerator : ITestGenerator
    {
        public const string TestCaseName = "My Test Case #1";
        public const string SourceFile = @"..\SourceFile.json";
        public const int LineNumber = 124;

        public IEnumerable<Test> Generate()
        {
            yield return new Test(TestCaseName)
            {
                LineNumber = LineNumber,
                SourceFile = SourceFile,
            };
        }
    }
}
