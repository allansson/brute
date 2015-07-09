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
    [ExtensionUri(ExecutorUriString)]
    public class TestGeneratorAdapter : ITestDiscoverer, ITestExecutor
    {
        public const string ExecutorUriString = "testexecutor://brute/generated-test";
        public static readonly Uri ExecutorUri = new Uri(ExecutorUriString);

        private bool cancelled;
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
            frameworkHandle.SendMessage(TestMessageLevel.Informational, "Running discovery...");

            IEnumerable<TestCase> testCases = discoverer.Discover(sources, frameworkHandle);

            frameworkHandle.SendMessage(TestMessageLevel.Informational, "Starting test run...");
            RunTests(testCases, runContext, frameworkHandle);
        }

        public void RunTests(IEnumerable<TestCase> tests, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            cancelled = false;

            foreach (TestCase testCase in tests)
            {
                if (cancelled)
                {
                    frameworkHandle.SendMessage(TestMessageLevel.Informational, "Test run was cancelled...");

                    break;
                }

                frameworkHandle.SendMessage(TestMessageLevel.Informational, String.Format("Running test case {0}...", testCase.DisplayName));

                TestResult result = new TestResult(testCase);
                TestContext context = testCase.LocalExtensionData as TestContext;

                frameworkHandle.SendMessage(TestMessageLevel.Informational, String.Format("TestContext instance is {0}...", context == null ? "null" : "not null"));

                try
                {
                    frameworkHandle.SendMessage(TestMessageLevel.Informational, "Calling test generator...");

                    context.Generator.Run(context.Test);

                    frameworkHandle.SendMessage(TestMessageLevel.Informational, "Test should have passed...");

                    result.Outcome = TestOutcome.Passed;
                }
                catch (Exception e)
                {

                    frameworkHandle.SendMessage(TestMessageLevel.Informational, "Test failed...");
                    frameworkHandle.SendMessage(TestMessageLevel.Informational, e.Message);

                    result.Outcome = TestOutcome.Failed;
                    result.ErrorMessage = e.Message;
                    result.ErrorStackTrace = e.StackTrace;
                }

                frameworkHandle.SendMessage(TestMessageLevel.Informational, "Recording result...");
                frameworkHandle.RecordResult(result);

                frameworkHandle.SendMessage(TestMessageLevel.Informational, "Result recorded...");
            }   
        }

        public void Cancel()
        {
            cancelled = true;
        }
    }
}
