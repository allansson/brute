using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brute
{
    public class TestContext
    {
        public ITestGenerator Generator { get; set; }
        public Test Test { get; set; }

        public TestContext(ITestGenerator generator, Test test)
        {
            this.Generator = generator;
            this.Test = test;
        }
    }
}
