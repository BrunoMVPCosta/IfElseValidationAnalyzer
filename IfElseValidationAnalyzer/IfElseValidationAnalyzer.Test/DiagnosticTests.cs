using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TestHelper;
using IfElseValidationAnalyzer;

namespace IfElseValidationAnalyzer.Test
{
    [TestClass]
    public class UnitTest : CodeFixVerifier
    {
        //Diagnostic and CodeFix both triggered and checked for
        [TestMethod]
        public void IfGuardClauseWithElse_UsingReturn_GetWarning()
        {
            var test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        public class Bar
        {   
            private void Bar(string[] args)
            {
                if(args.Count == 0)
                {
                    return;
                }
                else
                {
                    Console.WriteLine(args.Count);
                }
            }
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = "IfElseValidationAnalyzer",
                Message = "Guard clause don't need the else statement",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 15, 17)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void IfGuardClauseWithElse_UsingThrow_GetWarning()
        {
            var test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        public class Bar
        {   
            public double Divide(int number, int divisor)
            {
	            if(divisor == 0)
	            {
		            throw new ArgumentOutOfRangeException(""divisor""
		            , ""Divide by zero is not allowed"");
                    }
	            else
	            {
		            return number / divisor;
	            }
            }
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = "IfElseValidationAnalyzer",
                Message = "Guard clause don't need the else statement",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 15, 14)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void IfGuardClauseWithoutElse_UsingThrow_IsValid()
        {
            var test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        public class Bar
        {   
            public double Divide(int number, int divisor)
            {
	            if(divisor == 0)
	            {
		            throw new ArgumentOutOfRangeException(""divisor""
		            , ""Divide by zero is not allowed"");
                }

	            return number / divisor;
            }
        }
    }";
            VerifyCSharpDiagnostic(test);
        }


        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new IfElseValidationAnalyzerCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new IfElseValidationAnalyzerAnalyzer();
        }
    }
}