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
        private IDiscoveryContext discoveryContext;
        private ITestCaseDiscoverySink discoverySink;
        private IMessageLogger logger;
        private IFrameworkHandle frameworkHandle;
        private IRunContext runContext;

        public TestGeneratorAdapterTests()
        {
            discoveryContext = Substitute.For<IDiscoveryContext>();
            discoverySink = Substitute.For<ITestCaseDiscoverySink>();
            logger = Substitute.For<IMessageLogger>();
            frameworkHandle = Substitute.For<IFrameworkHandle>();
            runContext = Substitute.For<IRunContext>();
        }

        [Fact]
        public void WhenAssemblyFileInSourcesContainsNoImplementationOfITestGeneratorInterface_ShouldNotRegisterAnyTestCases()
        {
            TestGeneratorAdapter adapter = new TestGeneratorAdapter();

            adapter.DiscoverTests(new string[] { "Brute.AssemblyStubs.NoTestGenerator.dll" }, discoveryContext, logger, discoverySink);

            discoverySink.Received(0).SendTestCase(Arg.Any<TestCase>());
        }

        [Fact]
        public void WhenAssemblyFileInSourcesContainsSingleImplementationOfITestGeneratorInterface_ShouldRegisterTestCasesForAllTestsGenerated()
        {
            TestGeneratorAdapter adapter = new TestGeneratorAdapter();

            adapter.DiscoverTests(new string[] { "Brute.AssemblyStubs.SingleTestGenerator.dll" }, discoveryContext, logger, discoverySink);

            discoverySink.Received(1).SendTestCase(Arg.Any<TestCase>());
        }

        [Fact]
        public void WhenAssemblyFileInSourcesContainsMultipleImplementationsOfITestGeneratorInterface_ShouldRegisterTestCasesForEveryImplementation()
        {
            TestGeneratorAdapter adapter = new TestGeneratorAdapter();

            adapter.DiscoverTests(new string[] { "Brute.AssemblyStubs.MultipleTestGenerators.dll" }, discoveryContext, logger, discoverySink);

            discoverySink.Received(1).SendTestCase(Arg.Is<TestCase>(t => t.DisplayName == FirstTestGenerator.TestCaseName));
            discoverySink.Received(1).SendTestCase(Arg.Is<TestCase>(t => t.DisplayName == SecondTestGenerator.TestCaseName));
        }

        [Fact]
        public void WhenMultipleSourcesAreGiven_ShouldLoadTestGeneratorsFromAllSources()
        {
            TestGeneratorAdapter adapter = new TestGeneratorAdapter();

            adapter.DiscoverTests(new string[] { "Brute.AssemblyStubs.SingleTestGenerator.dll", "Brute.AssemblyStubs.MultipleTestGenerators.dll" }, discoveryContext, logger, discoverySink);

            discoverySink.Received(1).SendTestCase(Arg.Is<TestCase>(t => t.DisplayName == SingleTestGenerator.TestCaseName));
            discoverySink.Received(1).SendTestCase(Arg.Is<TestCase>(t => t.DisplayName == FirstTestGenerator.TestCaseName));
            discoverySink.Received(1).SendTestCase(Arg.Is<TestCase>(t => t.DisplayName == SecondTestGenerator.TestCaseName));
        }

        [Fact]
        public void WhenTestCaseIsRegistered_ShouldBeRegisteredWithDisplayNameGivenInTest()
        {
            TestGeneratorAdapter adapter = new TestGeneratorAdapter();

            adapter.DiscoverTests(new string[] { "Brute.AssemblyStubs.SingleTestGenerator.dll" }, discoveryContext, logger, discoverySink);

            discoverySink.Received(1).SendTestCase(Arg.Is<TestCase>(tc => tc.DisplayName == SingleTestGenerator.TestCaseName));
        }

        [Fact]
        public void WhenTestCaseIsRegistered_ShouldBeRegisteredWithLineNumberGivenInTest()
        {
            TestGeneratorAdapter adapter = new TestGeneratorAdapter();

            adapter.DiscoverTests(new string[] { "Brute.AssemblyStubs.SingleTestGenerator.dll" }, discoveryContext, logger, discoverySink);

            discoverySink.Received(1).SendTestCase(Arg.Is<TestCase>(tc => tc.LineNumber == SingleTestGenerator.LineNumber));
        }

        [Fact]
        public void WhenTestCaseIsRegistered_ShouldBeRegisteredWithCodeFilePathSetToSourceFileReturnedInTest()
        {
            TestGeneratorAdapter adapter = new TestGeneratorAdapter();

            adapter.DiscoverTests(new string[] { "Brute.AssemblyStubs.SingleTestGenerator.dll" }, discoveryContext, logger, discoverySink);

            discoverySink.Received(1).SendTestCase(Arg.Is<TestCase>(tc => tc.CodeFilePath == SingleTestGenerator.SourceFile));
        }

        [Fact]
        public void WhenTestCaseIsRegistered_ShouldSetFullyQualifiedNameToFullNameOfTestGeneratorTypePlusTheTestCaseNameWithoutSpacesSeparatedByHashTag()
        {
            TestGeneratorAdapter adapter = new TestGeneratorAdapter();

            adapter.DiscoverTests(new string[] { "Brute.AssemblyStubs.SingleTestGenerator.dll" }, discoveryContext, logger, discoverySink);

            discoverySink.Received(1).SendTestCase(Arg.Is<TestCase>(tc => tc.FullyQualifiedName == String.Format("{0}#{1}", typeof(SingleTestGenerator).FullName, SingleTestGenerator.TestCaseName.Replace(" ", ""))));
        }

        [Fact]
        public void WhenSourceIsTheBruteLibrary_ShouldIgnoreITestGeneratorInterface()
        {
            TestGeneratorAdapter adapter = new TestGeneratorAdapter();

            adapter.DiscoverTests(new string[] { "Brute.TestAdapter.dll" }, discoveryContext, logger, discoverySink);

            discoverySink.Received(0).SendTestCase(Arg.Any<TestCase>());
        }

        [Fact]
        public void WhenRunningTestsProvidingAssemblySources_ShouldDiscoverAndRunTestsInAssembly()
        {
            TestGeneratorAdapter adapter = new TestGeneratorAdapter();

            adapter.RunTests(new string[] { "Brute.AssemblyStubs.SingleTestGenerator.dll", "Brute.AssemblyStubs.MultipleTestGenerators.dll" }, runContext, frameworkHandle);

            frameworkHandle.Received(1).RecordResult(Arg.Is<TestResult>(result => result.TestCase.DisplayName == SingleTestGenerator.TestCaseName));
            frameworkHandle.Received(1).RecordResult(Arg.Is<TestResult>(result => result.TestCase.DisplayName == FirstTestGenerator.TestCaseName));
            frameworkHandle.Received(1).RecordResult(Arg.Is<TestResult>(result => result.TestCase.DisplayName == SecondTestGenerator.TestCaseName));
        }
    }
}
