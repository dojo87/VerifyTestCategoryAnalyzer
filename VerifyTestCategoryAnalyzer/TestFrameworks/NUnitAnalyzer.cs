using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;

namespace VerifyTestCategoryAnalyzer.TestFrameworks
{
    class NUnitAnalyzer : BaseFrameworkAnalyzer
    {
        public NUnitAnalyzer() : base("Test", "Category") { }
    }
}
