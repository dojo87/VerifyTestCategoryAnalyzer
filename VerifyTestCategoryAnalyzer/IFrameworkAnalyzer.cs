using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace VerifyTestCategoryAnalyzer
{
    public interface IFrameworkAnalyzer
    { 
        DiagnosticDescriptor Rule { get; }
        string TestAttribute { get; }
        string TestCategoryAttribute { get; }
        SymbolVerificationResult Verify(ISymbol symbol);
        SyntaxNodeOrTokenList GetTestCategoryAttributeArguments();
    }

    public enum SymbolVerificationResult
    {
        IsNotTest,
        CategoryValid,
        CategoryInvalid
    }
}
