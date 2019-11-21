using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VerifyTestCategoryAnalyzer.TestFrameworks;

namespace VerifyTestCategoryAnalyzer
{
    class FrameworkAnalyzerFactory
    {
        public static FrameworkAnalyzerFactory Instance { get; set; } = new FrameworkAnalyzerFactory();

        public virtual IEnumerable<IFrameworkAnalyzer> GetAnalyzers()
        {
            return new IFrameworkAnalyzer[] { new MSTestAnalyzer(), new NUnitAnalyzer(), new XUnitAnalyzer() };
        }
    }
}
