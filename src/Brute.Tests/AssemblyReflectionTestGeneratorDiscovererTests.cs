using Brute.AssemblyStubs.MultipleTestGenerators;
using Brute.AssemblyStubs.SingleTestGenerator;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Brute
{
    public class AssemblyReflectionTestGeneratorDiscovererTests
    {
        private IMessageLogger logger;

        public AssemblyReflectionTestGeneratorDiscovererTests()
        {
            this.logger = Substitute.For<IMessageLogger>();
        }

        [Fact]
        public void WhenAssemblyFileInSourcesContainsNoImplementationOfITestGeneratorInterface_ShouldNotRegisterAnyTestCases()
        {
            AssemblyReflectionTestGeneratorDiscoverer discoverer = new AssemblyReflectionTestGeneratorDiscoverer();

            IEnumerable<TestCase> result = discoverer.Discover(new string[] { "Brute.AssemblyStubs.NoTestGenerator.dll" }, logger);

            Assert.Empty(result);
        }

        [Fact]
        public void WhenAssemblyFileInSourcesContainsSingleImplementationOfITestGeneratorInterface_ShouldRegisterTestCasesForAllTestsGenerated()
        {
            AssemblyReflectionTestGeneratorDiscoverer discoverer = new AssemblyReflectionTestGeneratorDiscoverer();

            IEnumerable<TestCase> result = discoverer.Discover(new string[] { "Brute.AssemblyStubs.SingleTestGenerator.dll" }, logger);

            Assert.Equal(1, result.Count());
        }

        [Fact]
        public void WhenAssemblyFileInSourcesContainsMultipleImplementationsOfITestGeneratorInterface_ShouldRegisterTestCasesForEveryImplementation()
        {
            AssemblyReflectionTestGeneratorDiscoverer discoverer = new AssemblyReflectionTestGeneratorDiscoverer();

            IEnumerable<TestCase> result = discoverer.Discover(new string[] { "Brute.AssemblyStubs.MultipleTestGenerators.dll" }, logger);

            Assert.True(result.Any(t => t.DisplayName == FirstTestGenerator.TestCaseName));
            Assert.True(result.Any(t => t.DisplayName == SecondTestGenerator.TestCaseName));
        }

        [Fact]
        public void WhenMultipleSourcesAreGiven_ShouldLoadTestGeneratorsFromAllSources()
        {
            AssemblyReflectionTestGeneratorDiscoverer discoverer = new AssemblyReflectionTestGeneratorDiscoverer();

            IEnumerable<TestCase> result = discoverer.Discover(new string[] { "Brute.AssemblyStubs.SingleTestGenerator.dll", "Brute.AssemblyStubs.MultipleTestGenerators.dll" }, logger);

            Assert.True(result.Any(t => t.DisplayName == SingleTestGenerator.TestCaseName));
            Assert.True(result.Any(t => t.DisplayName == FirstTestGenerator.TestCaseName));
            Assert.True(result.Any(t => t.DisplayName == SecondTestGenerator.TestCaseName));
        }

        [Fact]
        public void WhenTestCaseIsGenerated_ShouldBeRegisteredWithDisplayNameGivenInTest()
        {
            AssemblyReflectionTestGeneratorDiscoverer discoverer = new AssemblyReflectionTestGeneratorDiscoverer();

            IEnumerable<TestCase> result = discoverer.Discover(new string[] { "Brute.AssemblyStubs.SingleTestGenerator.dll" }, logger);

            Assert.True(result.Any(tc => tc.DisplayName == SingleTestGenerator.TestCaseName));
        }

        [Fact]
        public void WhenTestCaseIsGenerated_ShouldBeRegisteredWithLineNumberGivenInTest()
        {
            AssemblyReflectionTestGeneratorDiscoverer discoverer = new AssemblyReflectionTestGeneratorDiscoverer();

            IEnumerable<TestCase> result = discoverer.Discover(new string[] { "Brute.AssemblyStubs.SingleTestGenerator.dll" }, logger);

            Assert.True(result.Any(tc => tc.LineNumber == SingleTestGenerator.LineNumber));
        }

        [Fact]
        public void WhenTestCaseIsGenerated_ShouldBeRegisteredWithCodeFilePathSetToSourceFileReturnedInTest()
        {
            AssemblyReflectionTestGeneratorDiscoverer discoverer = new AssemblyReflectionTestGeneratorDiscoverer();

            IEnumerable<TestCase> result = discoverer.Discover(new string[] { "Brute.AssemblyStubs.SingleTestGenerator.dll" }, logger);

            Assert.True(result.Any(tc => tc.CodeFilePath == SingleTestGenerator.SourceFile));
        }

        [Fact]
        public void WhenTestCaseIsGenerated_ShouldSetFullyQualifiedNameToFullNameOfTestGeneratorTypePlusTheTestCaseNameWithoutSpacesSeparatedByHashTag()
        {
            AssemblyReflectionTestGeneratorDiscoverer discoverer = new AssemblyReflectionTestGeneratorDiscoverer();

            IEnumerable<TestCase> result = discoverer.Discover(new string[] { "Brute.AssemblyStubs.SingleTestGenerator.dll" }, logger);

            Assert.True(result.Any(tc => tc.FullyQualifiedName == String.Format("{0}#{1}", typeof(SingleTestGenerator).FullName, SingleTestGenerator.TestCaseName.Replace(" ", ""))));
        }

        [Fact]
        public void WhenSourceIsTheBruteLibrary_ShouldIgnoreITestGeneratorInterface()
        {
            AssemblyReflectionTestGeneratorDiscoverer discoverer = new AssemblyReflectionTestGeneratorDiscoverer();

            IEnumerable<TestCase> result = discoverer.Discover(new string[] { "Brute.TestAdapter.dll" }, logger);

            Assert.Equal(0, result.Count());
        }
    }
}
