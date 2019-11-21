using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TestHelper;
using VerifyTestCategoryAnalyzer;

namespace VerifyTestCategoryAnalyzer.Test
{
    [TestClass]
    public class CommonTests : CodeFixVerifier
    {
        [TestMethod]
        public void TestCategoryFix_AppliesCorrectCategoryAttributeForEachFramework()
        {
            var test = @"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XUnit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit;

namespace ConsoleApplication1
{
    [TestFixture]
    [TestClass]
    class SomeTests
    {   
        [TestMethod]
        public void MSTestWithoutCategory(){}

        [Test]
        public void NUnitWithoutCategory(){}

        [Fact]
        public void XUnitWithoutCategory(){}
    }
}";


            var fixtest = @"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XUnit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit;

namespace ConsoleApplication1
{
    [TestFixture]
    [TestClass]
    class SomeTests
    {   
        [TestMethod]
        [TestCategory(""UnitTest"")]
        public void MSTestWithoutCategory(){}

        [Test]
        [Category(""UnitTest"")]
        public void NUnitWithoutCategory(){}

        [Fact]
        [Trait(""Category"", ""UnitTest"")]
        public void XUnitWithoutCategory(){}
    }
}";

            VerifyCSharpFix(test, fixtest, allowNewCompilerDiagnostics: true);
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new VerifyTestCategoryAnalyzerCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new VerifyTestCategoryAnalyzer();
        }
    }
}
