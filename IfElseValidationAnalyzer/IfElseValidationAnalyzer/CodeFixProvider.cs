using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Formatting;

namespace IfElseValidationAnalyzer
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(IfElseValidationAnalyzerCodeFixProvider)), Shared]
    public class IfElseValidationAnalyzerCodeFixProvider : CodeFixProvider
    {
        private const string title = "Do not use if-else stataments where you don't need it";

        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(IfElseValidationAnalyzerAnalyzer.DiagnosticId); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            // TODO: Replace the following code with your own analysis, generating a CodeAction for each fix to suggest
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            // Find the if statement identified by the diagnostic.
            var ifStatement = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<IfStatementSyntax>().First();

            // Register a code action that will invoke the fix.
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: title,
                    createChangedDocument: c => RemoveElseInGuardValidation(context.Document, ifStatement, c),
                    equivalenceKey: title),
                diagnostic);
        }

        private async Task<Document> RemoveElseInGuardValidation(Document document, IfStatementSyntax ifStatement, CancellationToken cancellationToken)
        {
            //We need to get the parent because we need to replace the entire block
            var blockSyntax = ifStatement.Parent as BlockSyntax;
            var blockElseStatement = ifStatement.Else.Statement as BlockSyntax;
            
            //Build the new if statement without the else condition
            var newIfStatement = SyntaxFactory.IfStatement(
                condition: ifStatement.Condition,
                statement: ifStatement.Statement);

            //Create an aux block
            var auxBlock = blockSyntax.RemoveNode(ifStatement, SyntaxRemoveOptions.KeepNoTrivia);

            //Create the new block with the if and the statements that were inside of the else block
            var newBlockSyntax = SyntaxFactory.Block()
                .AddStatements(newIfStatement)
                .AddStatements(blockElseStatement.Statements.ToArray())
                .AddStatements(auxBlock.Statements.ToArray());

            //Replace it in the document
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var newRoot = root.ReplaceNode(blockSyntax, newBlockSyntax)
                .WithAdditionalAnnotations(Formatter.Annotation);
            
            return document.WithSyntaxRoot(newRoot);
        }
    }
}