using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace VerifyTestCategoryAnalyzer.TestFrameworks
{
    class MSTestAnalyzer : BaseFrameworkAnalyzer
    {
        public MSTestAnalyzer() : base("TestMethod", "TestCategory") { }
    }
}
