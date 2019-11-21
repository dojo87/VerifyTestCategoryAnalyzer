using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace VerifyTestCategoryAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class VerifyTestCategoryAnalyzer : DiagnosticAnalyzer
    {
        #region Settings

        public const string DiagnosticId = "VerifyTestCategoryAnalyzer";

        #endregion Settings

        private IEnumerable<IFrameworkAnalyzer> FrameworkAnalyzers = FrameworkAnalyzerFactory.Instance.GetAnalyzers();

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(FrameworkAnalyzers.Select(a => a.Rule).ToArray()); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.Method);
            context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);
        }

        protected void AnalyzeSymbol(SymbolAnalysisContext context)
        {
            ISymbol symbol = context.Symbol;
            foreach (IFrameworkAnalyzer analyzer in FrameworkAnalyzers)
            {
                if (analyzer.Verify(symbol) == SymbolVerificationResult.CategoryInvalid)
                {
                    ImmutableDictionary<string, string> properties = (new Dictionary<string, string>() 
                        { 
                            { nameof(IFrameworkAnalyzer), analyzer.GetType().FullName } 
                        })
                        .ToImmutableDictionary();
                    var diagnostic = Diagnostic.Create(analyzer.Rule, symbol.Locations[0], properties,  symbol.Name);
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
    }
}
