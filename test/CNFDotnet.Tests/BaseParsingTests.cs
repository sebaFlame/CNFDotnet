using System;
using System.Collections.Generic;

using Xunit;
using Xunit.Abstractions;

using CNFDotnet.Analysis.Grammar;
using CNFDotnet.Analysis.Parsing;
using CNFDotnet.Analysis.Parsing.LL;
using CNFDotnet.Analysis.Parsing.LR.LR0;
using CNFDotnet.Analysis.Parsing.LR.SLR1;
using CNFDotnet.Analysis.Parsing.LR.LR1;

namespace CNFDotnet.Tests
{
    public abstract class BaseParsingTests
    {
        protected ITestOutputHelper TestOutputHelper { get; private set; }

        protected BaseParsingTests(ITestOutputHelper testOutputHelper)
        {
            this.TestOutputHelper = testOutputHelper;
        }

        protected CNFGrammar GenerateGrammar (string grammar)
        {
            Token token;
            CNFGrammar cnfGrammar;
            List<Token> tokens = new List<Token>();

            this.TestOutputHelper.WriteLine("Initializing lexer from string");
            BaseLexer lexer = new StringLexer(grammar);

            this.TestOutputHelper.WriteLine("Tokenizing grammar");
            while ((token = lexer.Next()).TokenType != TokenType.EOF)
            {
                tokens.Add(token);
            }
            tokens.Add(token);

            this.TestOutputHelper.WriteLine("Creating CNF grammar from tokens");

            cnfGrammar = new CNFGrammar(tokens);

            this.TestOutputHelper.WriteLine("Validating grammar");

            cnfGrammar.ComputeStartNonTerminal();
            Assert.NotNull(cnfGrammar.Start);

            cnfGrammar.ComputeNonTerminals();
            Assert.NotEmpty(cnfGrammar.NonTerminals);

            cnfGrammar.ComputeTerminals();
            Assert.NotEmpty(cnfGrammar.Terminals);

            cnfGrammar.ComputeUnreachable();
            cnfGrammar.ComputeUnrealizable();
            cnfGrammar.ComputeNullable();

            IReadOnlyList<Token> firstCycle = cnfGrammar.ComputeFirstCycle();
            if(firstCycle is not null)
            {
                Assert.Empty(firstCycle);
            }

            cnfGrammar.ComputeFirstSet();
            Assert.NotEmpty(cnfGrammar.FirstSet);

            cnfGrammar.ComputeFollowSet();
            Assert.NotEmpty(cnfGrammar.FollowSet);

            return cnfGrammar;
        }

        protected IParsing<IAction> CreateLL1Parsing (CNFGrammar cnfGrammar)
            => new LL1Parsing(cnfGrammar);

        protected IParsing<IAction> CreateLR0Parsing (CNFGrammar cnfGrammar)
            => new LR0Parsing(cnfGrammar);

        protected IParsing<IAction> CreateSLR1Parsing (CNFGrammar cnfGrammar)
            => new SLR1Parsing(cnfGrammar);

        protected IParsing<IAction> CreateLR1Parsing(CNFGrammar cnfGrammar)
            => new LR1Parsing(cnfGrammar);
    }
}