using Brute.AssemblyStubs.MultipleTestGenerators;
using Brute.AssemblyStubs.SingleTestGenerator;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Brute.Tests
{
    public class TestGeneratorAdapterTests
    {
        private ITestGeneratorDiscoverer testGeneratorDiscoverer;
        private IDiscoveryContext discoveryContext;
        private ITestCaseDiscoverySink discoverySink;
        private IMessageLogger logger;
        private IFrameworkHandle frameworkHandle;
        private IRunContext runContext;

        public TestGeneratorAdapterTests()
        {
            testGeneratorDiscoverer = Substitute.For<ITestGeneratorDiscoverer>();
            discoveryContext = Substitute.For<IDiscoveryContext>();
            discoverySink = Substitute.For<ITestCaseDiscoverySink>();
            logger = Substitute.For<IMessageLogger>();
            frameworkHandle = Substitute.For<IFrameworkHandle>();
            runContext = Substitute.For<IRunContext>();
        }

        private TestGeneratorAdapter CreateTestGeneratorAdapter()
        {
            return new TestGeneratorAdapter(testGeneratorDiscoverer);
        }
        
        [Fact]
        public void WhenTestGeneratorDiscovererDiscoversNoTests_ShouldNotRegisterAnyTestCases()
        {
            TestGeneratorAdapter adapter = CreateTestGeneratorAdapter();

            testGeneratorDiscoverer.Discover(Arg.Any<IEnumerable<string>>(), Arg.Any<IMessageLogger>())
                .Returns(new TestCase[] { });

            adapter.DiscoverTests(new string[] { "Source1" }, discoveryContext, logger, discoverySink);

            discoverySink.Received(0).SendTestCase(Arg.Any<TestCase>());
        }

        [Fact]
        public void WhenTestGeneratorDiscovererDiscoversATest_ShouldRegisterTestCase()
        {
            TestGeneratorAdapter adapter = CreateTestGeneratorAdapter();
            TestCase testCase = new TestCase("#1", TestGeneratorAdapter.ExecutorUri, "Source1");

            testGeneratorDiscoverer.Discover(Arg.Any<IEnumerable<string>>(), Arg.Any<IMessageLogger>())
                .Returns(new TestCase[] { testCase });

            adapter.DiscoverTests(new string[] { "Source1" }, discoveryContext, logger, discoverySink);

            discoverySink.Received(1).SendTestCase(testCase);
        }

        [Fact]
        public void WhenTestGeneratorDiscovererDiscoversMultipleTests_ShouldRegisterAllTestCases()
        {
            TestGeneratorAdapter adapter = CreateTestGeneratorAdapter();

            TestCase testCase1 = new TestCase("#1", TestGeneratorAdapter.ExecutorUri, "Source1");
            TestCase testCase2 = new TestCase("#1", TestGeneratorAdapter.ExecutorUri, "Source1");
            TestCase testCase3 = new TestCase("#1", TestGeneratorAdapter.ExecutorUri, "Source1");

            testGeneratorDiscoverer.Discover(Arg.Any<IEnumerable<string>>(), Arg.Any<IMessageLogger>())
                .Returns(new TestCase[] { testCase1, testCase2, testCase3 });

            adapter.DiscoverTests(new string[] { "Source1" }, discoveryContext, logger, discoverySink);

            discoverySink.Received(1).SendTestCase(testCase1);
            discoverySink.Received(1).SendTestCase(testCase2);
            discoverySink.Received(1).SendTestCase(testCase3);
        }

        [Fact]
        public void WhenRunningTestsProvidingAssemblySources_ShouldDiscoverAndRunTestsInAssembly()
        {
            TestGeneratorAdapter adapter = CreateTestGeneratorAdapter();

            TestCase testCase1 = new TestCase("#1", TestGeneratorAdapter.ExecutorUri, "Source1");
            TestCase testCase2 = new TestCase("#1", TestGeneratorAdapter.ExecutorUri, "Source1");
            TestCase testCase3 = new TestCase("#1", TestGeneratorAdapter.ExecutorUri, "Source1");

            testGeneratorDiscoverer.Discover(Arg.Any<IEnumerable<string>>(), Arg.Any<IMessageLogger>())
                .Returns(new TestCase[] { testCase1, testCase2, testCase3 });

            adapter.RunTests(new string[] { "Some Source" }, runContext, frameworkHandle);

            frameworkHandle.Received(3).RecordResult(Arg.Is<TestResult>(result => result.TestCase.DisplayName == "#1"));
        }
    }
}
