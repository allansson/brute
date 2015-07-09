using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System.Reflection;

namespace Brute
{
    [FileExtension(".exe")]
    [FileExtension(".dll")]
    [FileExtension(".appx")]
    [DefaultExecutorUri(ExecutorUriString)]
    public class TestGeneratorAdapter : ITestDiscoverer, ITestExecutor
    {
        public const string ExecutorUriString = "testexecutor://brute/generated-test";
        public static readonly Uri ExecutorUri = new Uri(ExecutorUriString);

        private ITestGeneratorDiscoverer discoverer;

        public TestGeneratorAdapter()
            : this(new AssemblyReflectionTestGeneratorDiscoverer())
        {

        }

        internal TestGeneratorAdapter(ITestGeneratorDiscoverer discoverer)
        {
            this.discoverer = discoverer;
        }

        public void DiscoverTests(IEnumerable<string> sources, IDiscoveryContext discoveryContext, IMessageLogger logger, ITestCaseDiscoverySink discoverySink)
        {
            foreach (TestCase testCase in discoverer.Discover(sources, logger))
            {
                discoverySink.SendTestCase(testCase);
            }
        }

        private static bool TypeIsImplementationOfTestGeneratorInterface(Type type)
        {
            return !type.IsInterface && typeof(ITestGenerator).IsAssignableFrom(type);
        }

        public void RunTests(IEnumerable<string> sources, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            IEnumerable<TestCase> testCases = discoverer.Discover(sources, frameworkHandle);

            RunTests(testCases, runContext, frameworkHandle);
        }

        public void RunTests(IEnumerable<TestCase> tests, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            foreach(TestCase testCase in tests)
            {
                TestResult result = new TestResult(testCase);
                TestContext context = testCase.LocalExtensionData as TestContext;

                try
                {
                    context.Generator.Run(context.Test);

                    result.Outcome = TestOutcome.Passed;
                }
                catch (Exception e)
                {
                    result.Outcome = TestOutcome.Failed;
                    result.ErrorMessage = e.Message;
                    result.ErrorStackTrace = e.StackTrace;
                }

                frameworkHandle.RecordResult(result);
            }   
        }

        public void Cancel()
        {
            
        }
    }
}
