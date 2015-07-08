using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brute
{
    public class Test
    {
        public string Name { get; private set; }
        public string SourceFile { get; set; }
        public int LineNumber { get; set; }

        public Test(string name)
        {
            this.Name = name;
        }
    }
}
