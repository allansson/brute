using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System.Reflection;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace Brute
{
    internal class AssemblyReflectionTestGeneratorDiscoverer : ITestGeneratorDiscoverer
    {
        public IEnumerable<TestCase> Discover(IEnumerable<string> sources, IMessageLogger logger)
        {
            foreach (string source in sources)
            {
                Assembly assembly = Assembly.LoadFrom(source);

                IEnumerable<Type> testGeneratorTypes = assembly.GetTypes()
                    .Where(TypeIsImplementationOfTestGeneratorInterface);

                foreach (Type testGeneratorType in testGeneratorTypes)
                {
                    ITestGenerator testGenerator = Activator.CreateInstance(testGeneratorType) as ITestGenerator;

                    foreach (Test test in testGenerator.Generate())
                    {
                        yield return new TestCase(String.Format("{0}#{1}", testGeneratorType.FullName, test.Name.Replace(" ", "")), TestGeneratorAdapter.ExecutorUri, source)
                        {
                            DisplayName = test.Name,
                            LineNumber = test.LineNumber,
                            CodeFilePath = test.SourceFile
                        };
                    }
                }
            }
        }

        private static bool TypeIsImplementationOfTestGeneratorInterface(Type type)
        {
            return !type.IsInterface && typeof(ITestGenerator).IsAssignableFrom(type);
        }
    }
}
