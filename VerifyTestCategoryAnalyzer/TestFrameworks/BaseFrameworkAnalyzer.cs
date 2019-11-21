using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace VerifyTestCategoryAnalyzer.TestFrameworks
{

    abstract class BaseFrameworkAnalyzer : IFrameworkAnalyzer
    {
        #region Properties
        
        private string testAttribute;
        private string testCategoryAttribute;

        public string TestAttribute { get => testAttribute; set => testAttribute = value.Replace("Attribute", ""); }
        public string TestCategoryAttribute { get => testCategoryAttribute; set => testCategoryAttribute = value.Replace("Attribute", ""); } 
        
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Tests";

        public virtual DiagnosticDescriptor Rule => new DiagnosticDescriptor(VerifyTestCategoryAnalyzer.DiagnosticId, Title, MessageFormat + $" ({this.TestCategoryAttribute}Attribute)", Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        #endregion Properties

        protected BaseFrameworkAnalyzer(string testAttribute, string testCategoryAttribute)
        {
            TestAttribute = testAttribute;
            TestCategoryAttribute = testCategoryAttribute;
        }

        #region IFrameworkAnalyzer members

        public SymbolVerificationResult Verify(ISymbol symbol)
        {
            var attributes = symbol.GetAttributes();
            if (TestAttributeExists(attributes, symbol))
            {
                AttributeData testCategoryAttribute = GetNearestTestCategoryAttribute(symbol, attributes);
                return VerifyTestCategoryAttribute(testCategoryAttribute, symbol) ?
                    SymbolVerificationResult.CategoryValid :
                    SymbolVerificationResult.CategoryInvalid;
            }
            else
            {
                return SymbolVerificationResult.IsNotTest;
            }
        }

        private AttributeData GetNearestTestCategoryAttribute(ISymbol symbol, ImmutableArray<AttributeData> attributes)
        {
            return GetAttribute(attributes, TestCategoryAttribute) ?? FindAttributeOnClass(symbol, TestCategoryAttribute);
        }

        public virtual SyntaxNodeOrTokenList GetTestCategoryAttributeArguments() => new SyntaxNodeOrTokenList(CreateLiteralArgumentFromString("UnitTest"));

        #endregion IFrameworkAnalyzer members

        #region Analyzer Processing

        protected virtual bool VerifyTestCategoryAttribute(AttributeData attribute, ISymbol symbolOfAttribute) => attribute != null;

        protected virtual bool TestAttributeExists(ImmutableArray<AttributeData> attributes, ISymbol symbolOfAttribute) => GetAttribute(attributes, TestAttribute) != null;

        protected static AttributeArgumentSyntax CreateLiteralArgumentFromString(string argumentLiteral)
            => SyntaxFactory.AttributeArgument(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(argumentLiteral)));

        protected AttributeData GetAttribute(ImmutableArray<AttributeData> attributes, string name) => attributes.FirstOrDefault(attr => CheckAttributeDerivedFrom(attr, name));

        private bool CheckAttributeDerivedFrom(AttributeData attribute, string name)
        {
            var attributeClass = attribute.AttributeClass;
            bool isDerived = false;
            while (attributeClass != null && !isDerived)
            {
                isDerived = attributeClass.Name == name || attributeClass.Name == $"{name}Attribute";
                attributeClass = attributeClass.BaseType;
            }

            return isDerived;
        }

        protected AttributeData FindAttributeOnClass(ISymbol symbolOfAttribute, string testCategoryAttribute) 
        {
            var classAttributes = symbolOfAttribute.ContainingType?.GetAttributes();
            if (classAttributes == null) return null;
            else return GetAttribute(classAttributes.Value, testCategoryAttribute);
        }

        #endregion Analyzer Processing
    }
}
