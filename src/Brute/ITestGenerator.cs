using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brute
{
    public interface ITestGenerator
    {
        IEnumerable<Test> Generate();
        void Run(Test test);
    }
}
