using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brute
{
    public interface ITestGeneratorDiscoverer
    {
        IEnumerable<TestCase> Discover(IEnumerable<string> sources, IMessageLogger logger);
    }
}
