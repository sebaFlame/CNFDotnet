using System.Collections.Generic;

using Xunit;
using Xunit.Abstractions;

using CNFDotnet.Analysis.Grammar;
using CNFDotnet.Analysis.Parsing;
using CNFDotnet.Analysis.Parsing.LL;
using CNFDotnet.Analysis.Parsing.LR.LR0;
using CNFDotnet.Analysis.Parsing.LR.SLR1;
using CNFDotnet.Analysis.Parsing.LR.LR1;
using CNFDotnet.Analysis.Parsing.LR.LALR1;

namespace CNFDotnet.Tests
{
    public abstract class BaseParsingTests
    {
        protected ITestOutputHelper TestOutputHelper { get; private set; }

        protected BaseParsingTests(ITestOutputHelper testOutputHelper)
        {
            this.TestOutputHelper = testOutputHelper;
        }

        protected CNFGrammar GenerateGrammar(string grammar)
        {
            Token token;
            CNFGrammar cnfGrammar;
            List<Token> tokens = new List<Token>();

            this.TestOutputHelper.WriteLine("Initializing lexer from string");
            BaseLexer lexer = new StringLexer(grammar);

            this.TestOutputHelper.WriteLine("Tokenizing grammar");
            while((token = lexer.Next()).TokenType != TokenType.EOF)
            {
                tokens.Add(token);
            }
            tokens.Add(token);

            this.TestOutputHelper.WriteLine("Creating CNF grammar from tokens");

            cnfGrammar = new CNFGrammar(tokens);

            this.TestOutputHelper.WriteLine("Validating grammar");

            //Assert.Empty(cnfGrammar.ComputeUnreachable());
            //Assert.Empty(cnfGrammar.ComputeUnrealizable());
            Assert.Empty(cnfGrammar.ComputeFirstCycle());

            return cnfGrammar;
        }

        protected static IParsing<LL1Action> CreateLL1Parsing
            (CNFGrammar cnfGrammar)
            => new LL1Parsing(cnfGrammar);

        protected static IParsing<LR0Action> CreateLR0Parsing
            (CNFGrammar cnfGrammar)
            => new LR0Parsing(cnfGrammar);

        protected static IParsing<SLR1Action> CreateSLR1Parsing
            (CNFGrammar cnfGrammar)
            => new SLR1Parsing(cnfGrammar);

        protected static IParsing<LR1Action> CreateLR1Parsing
            (CNFGrammar cnfGrammar)
            => new LR1Parsing(cnfGrammar);

        protected static IParsing<LALR1Action> CreateLALR1Parsing
            (CNFGrammar cnfGrammar)
            => new LALR1Parsing(cnfGrammar);
    }
}