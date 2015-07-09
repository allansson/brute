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
        private ITestGenerator testGenerator;

        public TestGeneratorAdapterTests()
        {
            testGeneratorDiscoverer = Substitute.For<ITestGeneratorDiscoverer>();
            discoveryContext = Substitute.For<IDiscoveryContext>();
            discoverySink = Substitute.For<ITestCaseDiscoverySink>();
            logger = Substitute.For<IMessageLogger>();
            frameworkHandle = Substitute.For<IFrameworkHandle>();
            runContext = Substitute.For<IRunContext>();
            testGenerator = Substitute.For<ITestGenerator>();
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

            TestCase testCase1 = new TestCase("#1", TestGeneratorAdapter.ExecutorUri, "Source1")
            {
                LocalExtensionData = new TestContext(testGenerator, new Test("#1"))
            };

            TestCase testCase2 = new TestCase("#1", TestGeneratorAdapter.ExecutorUri, "Source1")
            {
                LocalExtensionData = new TestContext(testGenerator, new Test("#1"))
            }; 

            TestCase testCase3 = new TestCase("#1", TestGeneratorAdapter.ExecutorUri, "Source1")
            {
                LocalExtensionData = new TestContext(testGenerator, new Test("#1"))
            };

            testGeneratorDiscoverer.Discover(Arg.Any<IEnumerable<string>>(), Arg.Any<IMessageLogger>())
                .Returns(new TestCase[] { testCase1, testCase2, testCase3 });

            adapter.RunTests(new string[] { "Some Source" }, runContext, frameworkHandle);

            frameworkHandle.Received(3).RecordResult(Arg.Is<TestResult>(result => result.TestCase.DisplayName == "#1"));
        }

        [Fact]
        public void WhenTestGeneratorMethodReturnsWithoutFailure_ShouldRecordTestResultForTestCase()
        {
            TestGeneratorAdapter adapter = CreateTestGeneratorAdapter();

            TestCase testCase = new TestCase("#1", TestGeneratorAdapter.ExecutorUri, "Source1")
            {
                LocalExtensionData = new TestContext(testGenerator, new Test("#1"))
            };

            adapter.RunTests(new TestCase[] { testCase }, runContext, frameworkHandle);

            frameworkHandle.Received(1).RecordResult(Arg.Is<TestResult>(result => result.TestCase == testCase));
        }

        [Fact]
        public void WhenRunningTestCase_ShouldPassTestToRunMethodOfTestGenerator()
        {
            TestGeneratorAdapter adapter = CreateTestGeneratorAdapter();

            Test test = new Test("Some test");
            TestCase testCase = new TestCase("#1", TestGeneratorAdapter.ExecutorUri, "Source1")
            {
                LocalExtensionData = new TestContext(testGenerator, test)
            };

            adapter.RunTests(new TestCase[] { testCase }, runContext, frameworkHandle);

            testGenerator.Received(1).Run(test);
        }

        [Fact]
        public void WhenTestGeneratorMethodReturnsWithoutFailure_ShouldRecordTestResultAsPassed()
        {
            TestGeneratorAdapter adapter = CreateTestGeneratorAdapter();

            TestCase testCase = new TestCase("#1", TestGeneratorAdapter.ExecutorUri, "Source1")
            {
                LocalExtensionData = new TestContext(testGenerator, new Test("Some test"))
            };

            adapter.RunTests(new TestCase[] { testCase }, runContext, frameworkHandle);

            frameworkHandle.Received(1).RecordResult(Arg.Is<TestResult>(result => result.Outcome == TestOutcome.Passed));
        }

        [Fact]
        public void WhenTestGeneratorThrowsAnExcpetion_ShouldRecordTestResultAsFailed()
        {
            TestGeneratorAdapter adapter = CreateTestGeneratorAdapter();

            TestCase testCase = new TestCase("#1", TestGeneratorAdapter.ExecutorUri, "Source1")
            {
                LocalExtensionData = new TestContext(testGenerator, new Test("Some test"))
            };

            testGenerator.When(g => g.Run(Arg.Any<Test>())).Do(c => { throw new Exception(); });

            adapter.RunTests(new TestCase[] { testCase }, runContext, frameworkHandle);

            frameworkHandle.Received(1).RecordResult(Arg.Is<TestResult>(result => result.Outcome == TestOutcome.Failed));
        }

        [Fact]
        public void WhenTestGeneratorThrowsAnExcpetion_ShouldSetErrorMessageToMessageOfException()
        {
            TestGeneratorAdapter adapter = CreateTestGeneratorAdapter();

            TestCase testCase = new TestCase("#1", TestGeneratorAdapter.ExecutorUri, "Source1")
            {
                LocalExtensionData = new TestContext(testGenerator, new Test("Some test"))
            };

            testGenerator.When(g => g.Run(Arg.Any<Test>())).Do(c => { throw new Exception("Expected message"); });

            adapter.RunTests(new TestCase[] { testCase }, runContext, frameworkHandle);

            frameworkHandle.Received(1).RecordResult(Arg.Is<TestResult>(result => result.ErrorMessage == "Expected message"));
        }

        [Fact]
        public void WhenTestGeneratorThrowsAnExcpetion_ShouldSetErrorStackTraceOnTestResult()
        {
            TestGeneratorAdapter adapter = CreateTestGeneratorAdapter();

            TestCase testCase = new TestCase("#1", TestGeneratorAdapter.ExecutorUri, "Source1")
            {
                LocalExtensionData = new TestContext(testGenerator, new Test("Some test"))
            };

            testGenerator.When(g => g.Run(Arg.Any<Test>())).Do(c => { throw new Exception("Expected message"); });

            adapter.RunTests(new TestCase[] { testCase }, runContext, frameworkHandle);

            frameworkHandle.Received(1).RecordResult(Arg.Is<TestResult>(result => !String.IsNullOrEmpty(result.ErrorStackTrace)));
        }
    }
}
