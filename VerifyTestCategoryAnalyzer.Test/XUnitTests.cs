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
    public class XUnitTests : CodeFixVerifier
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
                
                namespace ConsoleApplication1
                {
                    // Required to read attribute constructor arguments. In normal projects, XUnit references will provide the attribute declaration.
                    class TraitAttribute: Attribute {
                        public TraitAttribute(string name, string value) {}
                    }
                    class SomeTests
                    {   
                        [Fact]
                        public void TestWithoutCategory(){}

                        [Fact]
                        [Trait(""Category"",""UnitTest"")]
                        public void TestWithCategory(){}
                    }
                }";

            var expected = new DiagnosticResult
            {
                Id = "VerifyTestCategoryAnalyzer",
                Message = String.Format("Test Method '{0}' has no category (TraitAttribute)", "TestWithoutCategory"),
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 17, 37)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void ForTestDecoratedWithITraitAttributeImplementation_ShowNoWarnings()
        {
            var test = @"
                using System;
                using System.Collections.Generic;
                using System.Linq;
                using System.Text;
                using System.Threading.Tasks;
                
                namespace ConsoleApplication1
                {
                    // Required to read attribute constructor arguments. In normal projects, XUnit references will provide the attribute declaration.
                    interface ITraitAttribute {
                    }
                    class CategoryAttribute : Attribute, ITraitAttribute
                    {
                        public CategoryAttribute(string category) { }
                    }
                    class SomeTests
                    {   
                        [Fact]
                        [Category(""UnitTest"")]
                        public void TestWithCategory1(){}

                        [Fact]
                        [Trait(""Category"",""UnitTest"")]
                        public void TestWithCategory2(){}
                    }
                }";

            
            VerifyCSharpDiagnostic(test);
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
                using XUnit;

                namespace ConsoleApplication1
                {
                    // Required to read attribute constructor arguments. In normal projects, XUnit references will provide the attribute declaration.
                    class TraitAttribute: Attribute {
                        public TraitAttribute(string name, string value) {}
                    }
                    class SomeTests
                    {   
                        [Fact]
                        [Trait(""Category"",""SystemTest"")]
                        public void TestWithCategory2(){}

                        [Fact]
                        [Trait(""Category"",""UnitTest"")]
                        public void TestWithCategory(){}
                    }
                }";

         
            VerifyCSharpDiagnostic(test);
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
using XUnit;

namespace ConsoleApplication1
{
    // Required to read attribute constructor arguments. In normal projects, XUnit references will provide the attribute declaration.
    class TraitAttribute: Attribute {
        public TraitAttribute(string name, string value) {}
    }
    class SomeTests
    {   
        [Fact]
        public void TestWithoutCategory(){}

        [Fact]
        [Trait(""Category"", ""UnitTest"")]
        public void TestWithCategory(){}
    }
}";


            var fixtest = @"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XUnit;

namespace ConsoleApplication1
{
    // Required to read attribute constructor arguments. In normal projects, XUnit references will provide the attribute declaration.
    class TraitAttribute: Attribute {
        public TraitAttribute(string name, string value) {}
    }
    class SomeTests
    {   
        [Fact]
        [Trait(""Category"", ""UnitTest"")]
        public void TestWithoutCategory(){}

        [Fact]
        [Trait(""Category"", ""UnitTest"")]
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
    // Required to read attribute constructor arguments. In normal projects, XUnit references will provide the attribute declaration.
    class TraitAttribute: Attribute {
        public TraitAttribute(string name, string value) {}
    }
    class SomeTests
    {   
        [Fact]
        public void TestWithoutCategory(){}

        [Fact]
        public void TestWithoutCategory2(){}

        [Fact]
        [Trait(""Category"", ""UnitTest"")]
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
    // Required to read attribute constructor arguments. In normal projects, XUnit references will provide the attribute declaration.
    class TraitAttribute: Attribute {
        public TraitAttribute(string name, string value) {}
    }
    class SomeTests
    {   
        [Fact]
        [Trait(""Category"", ""UnitTest"")]
        public void TestWithoutCategory(){}

        [Fact]
        [Trait(""Category"", ""UnitTest"")]
        public void TestWithoutCategory2(){}

        [Fact]
        [Trait(""Category"", ""UnitTest"")]
        public void TestWithCategory(){}
    }
}";

            VerifyCSharpFix(test, fixtest, allowNewCompilerDiagnostics: true);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        public void ForTraitOnClass_TestMethodsShowNoWarnings()
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
    // Required to read attribute constructor arguments. In normal projects, XUnit references will provide the attribute declaration.
    class TraitAttribute: Attribute {
        public TraitAttribute(string name, string value) {}
    }
    [Trait(""Category"", ""UnitTest"")]
    class SomeTests
    {   
        [Fact]
        public void TestWithoutCategory(){}

        [Fact]
        public void TestWithoutCategory2(){}

        [Fact]
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
