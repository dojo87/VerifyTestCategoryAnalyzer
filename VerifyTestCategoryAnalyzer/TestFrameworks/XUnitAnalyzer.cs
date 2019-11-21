using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace VerifyTestCategoryAnalyzer.TestFrameworks
{
    class XUnitAnalyzer : BaseFrameworkAnalyzer
    {
        private const string TRAIT_INTERFACE = "ITraitAttribute";

        public XUnitAnalyzer(): base("Fact", "Trait") { }

        protected override bool VerifyTestCategoryAttribute(AttributeData attribute, ISymbol symbolOfAttribute)
        {
            bool hasAttributeWithTraitInterface = symbolOfAttribute.GetAttributes().Any(a => a.AttributeClass.Interfaces.Any(attrInterface => attrInterface.Name == TRAIT_INTERFACE));
            return base.VerifyTestCategoryAttribute(attribute, symbolOfAttribute) || hasAttributeWithTraitInterface;
        }

        public override SyntaxNodeOrTokenList GetTestCategoryAttributeArguments() 
            => new SyntaxNodeOrTokenList(
                CreateLiteralArgumentFromString("Category"), 
                SyntaxFactory.Token(SyntaxKind.CommaToken), 
                CreateLiteralArgumentFromString("UnitTest"));
    }
}
