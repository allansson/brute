using Brute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brute.AssemblyStubs.MultipleTestsGenerator
{
    public class MultipleTestsGenerator : ITestGenerator
    {
        public const string TestCaseName1 = "First";
        public const string TestCaseName2 = "Second";
        public const string TestCaseName3 = "Third";
        public const string TestCaseName4 = "Fourth";

        public IEnumerable<Test> Generate()
        {
            yield return new Test(TestCaseName1);
            yield return new Test(TestCaseName2);
            yield return new Test(TestCaseName3);
            yield return new Test(TestCaseName4);
        }

        public void Run(Test test)
        {
            throw new NotImplementedException();
        }
    }
}
