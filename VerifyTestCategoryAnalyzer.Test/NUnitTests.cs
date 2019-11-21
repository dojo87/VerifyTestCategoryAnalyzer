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
    public class NUnitTests : CodeFixVerifier
    {

        [TestMethod]
        public void ForClassHavingOneTestMethodWithCategory_AnalyzerReturnsOneWarning()
        { 
            var test = @"
                using System;
                using System.Collections.Generic;
                using System.Linq;
                using System.Text;
                using System.Threading.Tasks;
                using NUnit;

                namespace ConsoleApplication1
                {
                    [TestFixture]
                    class SomeTests
                    {   
                        [Test]
                        public void TestWithoutCategory(){}

                        [Test]
                        [Category(""UnitTest"")]
                        public void TestWithCategory(){}
                    }
                }";

            var expected = new DiagnosticResult
            {
                Id = "VerifyTestCategoryAnalyzer",
                Message = String.Format("Test Method '{0}' has no category (CategoryAttribute)", "TestWithoutCategory"),
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 15, 37)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void ForClassHavingAllTestsWithCategories_AnalyzerReturnsNoWarning()
        {
            var test = @"
                using System;
                using System.Collections.Generic;
                using System.Linq;
                using System.Text;
                using System.Threading.Tasks;
                using NUnit;

                namespace ConsoleApplication1
                {
                    [TestFixture]
                    class SomeTests
                    {   
                        [Test]
                        [Category(""SystemTest"")]
                        public void TestWithCategory2(){}

                        [Test]
                        [Category(""UnitTest"")]
                        public void TestWithCategory(){}
                    }
                }";

         
            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        public void ForMultipleTestsWithoutCategory_CodeFixAppliesTestCategoryUnitTestForAll()
        {
            var test = @"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit;

namespace ConsoleApplication1
{
    [TestFixture]
    class SomeTests
    {   
        [Test]
        public void TestWithoutCategory(){}

        [Test]
        public void TestWithoutCategory2(){}

        [Test]
        [Category(""UnitTest"")]
        public void TestWithCategory(){}
    }
}";


            var fixtest = @"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit;

namespace ConsoleApplication1
{
    [TestFixture]
    class SomeTests
    {   
        [Test]
        [Category(""UnitTest"")]
        public void TestWithoutCategory(){}

        [Test]
        [Category(""UnitTest"")]
        public void TestWithoutCategory2(){}

        [Test]
        [Category(""UnitTest"")]
        public void TestWithCategory(){}
    }
}";

            VerifyCSharpFix(test, fixtest, allowNewCompilerDiagnostics: true);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        public void ForCategoryOnClass_TestMethodsShowNoWarnings()
        {
            var test = @"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit;

namespace ConsoleApplication1
{
    [Category(""UnitTest"")]
    class SomeTests
    {   
        [Test]
        public void TestWithoutCategory(){}

        [Test]
        public void TestWithoutCategory2(){}

        [Test]
        public void TestWithCategory(){}
    }
}";
            VerifyCSharpDiagnostic(test);
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
