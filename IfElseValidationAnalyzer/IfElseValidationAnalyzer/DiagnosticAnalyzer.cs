using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace IfElseValidationAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class IfElseValidationAnalyzerAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "IfElseValidationAnalyzer";

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "CSharp.Readability";

        private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        private static readonly Action<SyntaxNodeAnalysisContext> IfStatementAction = HandleIfStatement;


        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(IfStatementAction, SyntaxKind.IfStatement);
        }

        private static void HandleIfStatement(SyntaxNodeAnalysisContext context)
        {
            var ifStatement = context.Node as IfStatementSyntax;

            if (ifStatement == null) return;
            if (ifStatement.Else == null) return;

            var childNodes = ifStatement.Statement.ChildNodes();
            if (childNodes.Count() != 1) return;

            var node = childNodes.First();
            if (node is ReturnStatementSyntax || node is ThrowStatementSyntax)
            {
                var diagnostic = Diagnostic.Create(Rule, ifStatement.GetLocation());

                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
