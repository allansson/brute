using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brute.AssemblyStubs.MultipleTestGenerators
{
    public class FirstTestGenerator : ITestGenerator
    {
        public const string TestCaseName = "From First Test Generator";

        public IEnumerable<Test> Generate()
        {
            yield return new Test(TestCaseName);
        }
    }
}
