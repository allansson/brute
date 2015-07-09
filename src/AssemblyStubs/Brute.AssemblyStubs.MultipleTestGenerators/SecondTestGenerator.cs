using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brute.AssemblyStubs.MultipleTestGenerators
{
    public class SecondTestGenerator : ITestGenerator
    {
        public const string TestCaseName = "From Second Test Generator";

        public IEnumerable<Test> Generate()
        {
            yield return new Test(TestCaseName);
        }

        public void Run(Test test)
        {
            throw new NotImplementedException();
        }
    }
}
