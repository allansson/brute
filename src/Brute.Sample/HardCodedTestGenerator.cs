using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brute.Sample
{
    public class HardCodedTestGenerator : ITestGenerator
    {
        public IEnumerable<Test> Generate()
        {
            for (int i = 0; i < 10; i++)
            {
                yield return new Test("Test #" + i);
            }
        }
    }
}
