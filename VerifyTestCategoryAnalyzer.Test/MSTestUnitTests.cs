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
    public class MSTestUnitTests : CodeFixVerifier
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
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                namespace ConsoleApplication1
                {
                    [TestClass]
                    class SomeTests
                    {   
                        [TestMethod]
                        public void TestWithoutCategory(){}

                        [TestMethod]
                        [TestCategory(""UnitTest"")]
                        public void TestWithCategory(){}
                    }
                }";

            var expected = new DiagnosticResult
            {
                Id = "VerifyTestCategoryAnalyzer",
                Message = String.Format("Test Method '{0}' has no category (TestCategoryAttribute)", "TestWithoutCategory"),
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
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                namespace ConsoleApplication1
                {
                    [TestClass]
                    class SomeTests
                    {   
                        [TestMethod]
                        [TestCategory(""SystemTest"")]
                        public void TestWithCategory2(){}

                        [TestMethod]
                        [TestCategory(""UnitTest"")]
                        public void TestWithCategory(){}
                    }
                }";

         
            VerifyCSharpDiagnostic(test);
        }


        [TestMethod]
        [TestCategory("UnitTest")]
        public void ForClassFromFile_HavingOneTestWithoutCategory_FixAppliesTestCategoryUnityTest()
        {
            var test = System.IO.File.ReadAllText("TestClass.cs");
            var fixtest = System.IO.File.ReadAllText("TestClassFixed.cs");

            VerifyCSharpFix(test, fixtest, allowNewCompilerDiagnostics: true);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        public void ForTestWithoutCategory_CodeFixAppliesTestCategoryUnitTest()
        {  
            var test = @"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConsoleApplication1
{
    [TestClass]
    class SomeTests
    {   
        [TestMethod]
        public void TestWithoutCategory(){}

        [TestMethod]
        [TestCategory(""UnitTest"")]
        public void TestWithCategory(){}
    }
}";


            var fixtest = @"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConsoleApplication1
{
    [TestClass]
    class SomeTests
    {   
        [TestMethod]
        [TestCategory(""UnitTest"")]
        public void TestWithoutCategory(){}

        [TestMethod]
        [TestCategory(""UnitTest"")]
        public void TestWithCategory(){}
    }
}";

            VerifyCSharpFix(test, fixtest, allowNewCompilerDiagnostics: true);
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
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConsoleApplication1
{
    [TestClass]
    class SomeTests
    {   
        [TestMethod]
        public void TestWithoutCategory(){}

        [TestMethod]
        public void TestWithoutCategory2(){}

        [TestMethod]
        [TestCategory(""UnitTest"")]
        public void TestWithCategory(){}
    }
}";


            var fixtest = @"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConsoleApplication1
{
    [TestClass]
    class SomeTests
    {   
        [TestMethod]
        [TestCategory(""UnitTest"")]
        public void TestWithoutCategory(){}

        [TestMethod]
        [TestCategory(""UnitTest"")]
        public void TestWithoutCategory2(){}

        [TestMethod]
        [TestCategory(""UnitTest"")]
        public void TestWithCategory(){}
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
