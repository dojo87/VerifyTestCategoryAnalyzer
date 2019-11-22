using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace VerifyTestCategoryAnalyzer
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(VerifyTestCategoryAnalyzerCodeFixProvider)), Shared]
    public class VerifyTestCategoryAnalyzerCodeFixProvider : CodeFixProvider
    {
        private const string title = "Add Test Category";

        private IEnumerable<IFrameworkAnalyzer> FrameworkAnalyzers = FrameworkAnalyzerFactory.Instance.GetAnalyzers();

        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(VerifyTestCategoryAnalyzer.DiagnosticId); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.FirstOrDefault(d => d.Id == VerifyTestCategoryAnalyzer.DiagnosticId);
            if (diagnostic != null)
            {
                var diagnosticSpan = diagnostic.Location.SourceSpan;

                MethodDeclarationSyntax declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().First();

                context.RegisterCodeFix(
                    CodeAction.Create(
                        title: title,
                        createChangedSolution: c => CreateTestCategoryAttribute(context.Document, diagnostic, declaration, c),
                        equivalenceKey: title),
                    diagnostic);
            }
        }

        private async Task<Solution> CreateTestCategoryAttribute(Document document, Diagnostic diagnostic, MethodDeclarationSyntax methodDeclaration, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken);

            var frameworkAnalyzerUsed = FrameworkAnalyzers.FirstOrDefault(analyzer => analyzer.GetType().FullName == diagnostic.Properties[nameof(IFrameworkAnalyzer)]);

            string testCategoryAttribute = frameworkAnalyzerUsed.TestCategoryAttribute;

            SyntaxNodeOrTokenList attributeArguments = frameworkAnalyzerUsed.GetTestCategoryAttributeArguments();

            var currentIndent = methodDeclaration.GetLeadingTrivia().Where(t => t.IsKind(SyntaxKind.WhitespaceTrivia));
            
            var attributes = methodDeclaration.AttributeLists.Add(
                SyntaxFactory.AttributeList(
                    SyntaxFactory.SingletonSeparatedList(
                        SyntaxFactory.Attribute(
                            SyntaxFactory.IdentifierName(testCategoryAttribute))
                                .WithArgumentList(SyntaxFactory
                                    .AttributeArgumentList(SyntaxFactory
                                        .SeparatedList<AttributeArgumentSyntax>(attributeArguments)))

                )).NormalizeWhitespace()
                  .WithLeadingTrivia(currentIndent)
                  .WithTrailingTrivia(SyntaxFactory.Whitespace(Environment.NewLine)));

            document = document.WithSyntaxRoot(
                root.ReplaceNode(
                    methodDeclaration,
                    methodDeclaration.WithAttributeLists(attributes)
                ));

            return document.Project.Solution;
        }

        private static AttributeArgumentSyntax CreateLiteralArgumentFromString(string argumentLiteral)
        {
            return SyntaxFactory.AttributeArgument(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(argumentLiteral)));
        }
    }
}
