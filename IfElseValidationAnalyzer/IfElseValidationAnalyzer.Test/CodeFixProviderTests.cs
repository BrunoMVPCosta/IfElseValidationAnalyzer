using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;

namespace IfElseValidationAnalyzer.Test
{

    [TestClass]
    public class CodeFixProviderTests : CodeFixVerifier
    {
        [TestMethod]
        public void IfGuardClauseWithElse_UsingReturn_GetsTheRightFix()
        {
            var oldCode = @"
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
            if (args.Count == 0)
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

            var expectedOutput = @"
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
            if (args.Count == 0)
            {
                return;
            }
            Console.WriteLine(args.Count);
        }
    }
}";

            VerifyCSharpFix(oldCode, expectedOutput);
        }

        [TestMethod]
        public void IfGuardClauseWithElse_UsingReturnAndMultipleStatements_GetsTheRightFix()
        {
            var oldCode = @"
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
            if (args.Count == 0)
            {
                return;
            }
            else
            {
                Console.WriteLine(args.Count);
            }
            bool isValid = CheckIfItIsValid(args.Count);
        }
    }
}";

            var expectedOutput = @"
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
            if (args.Count == 0)
            {
                return;
            }
            Console.WriteLine(args.Count);
            bool isValid = CheckIfItIsValid(args.Count);
        }
    }
}";

            VerifyCSharpFix(oldCode, expectedOutput);
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
