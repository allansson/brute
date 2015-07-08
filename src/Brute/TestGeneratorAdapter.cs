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

        public void DiscoverTests(IEnumerable<string> sources, IDiscoveryContext discoveryContext, IMessageLogger logger, ITestCaseDiscoverySink discoverySink)
        {
            string source = sources.FirstOrDefault();

            Assembly assembly = Assembly.LoadFrom(source);

            Type testGeneratorType = assembly.GetTypes()
                .Where(t => typeof(ITestGenerator).IsAssignableFrom(t))
                .FirstOrDefault();

            if (testGeneratorType == null)
            {
                return;
            }

            ITestGenerator testGenerator = Activator.CreateInstance(testGeneratorType) as ITestGenerator;

            Test test = testGenerator.Generate().FirstOrDefault();

            discoverySink.SendTestCase(new TestCase(String.Format("{0}#{1}", testGeneratorType.FullName, test.Name.Replace(" ", "")), ExecutorUri, source)
            {
                DisplayName = test.Name,
                LineNumber = test.LineNumber,
                CodeFilePath = test.SourceFile
            });
        }

        public void RunTests(IEnumerable<string> sources, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            
        }

        public void RunTests(IEnumerable<TestCase> tests, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            
        }

        public void Cancel()
        {
            
        }
    }
}
