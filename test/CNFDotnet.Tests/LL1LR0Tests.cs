using System;
using System.Collections.Generic;

using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

using CNFDotnet.Analysis.Grammar;
using CNFDotnet.Analysis.Parsing;
using CNFDotnet.Analysis.Parsing.LL;

namespace CNFDotnet.Tests
{
    public class LL1LR0Tests : BaseParsingTests
    {
        public static IEnumerable<object[]> LL1LR0Grammars =>
            new object[][]
            {
                new object[]
                {
                    @"A -> B | x C | y A
                        B -> C B
                        C -> r"
                },
                new object[]
                {
                    @"A -> y B | x | B C
                        B -> z B | u
                        C -> s"
                },
                new object[]
                {
                    @"S -> ( Ses ) | (* *)
                        Ses -> S SL
                        SL -> ; SL | S"
                }
            };

        public LL1LR0Tests (ITestOutputHelper testOutputHelper)
            : base (testOutputHelper)
        { }

        [Theory]
        [MemberDataAttribute(nameof(LL1LR0Tests.LL1LR0Grammars))]
        public void LL1_And_LR0_Valid_Grammar (string grammar)
        {
            CNFGrammar cnfGrammar = this.GenerateGrammar(grammar);

            IParsing<IAction> ll1Parsing = this.CreateLL1Parsing(cnfGrammar);
            ll1Parsing.Classify();
            Assert.NotNull(ll1Parsing.CreateParsingTable());

            IParsing<IAction> lr0Parsing = this.CreateLR0Parsing(cnfGrammar);
            lr0Parsing.Classify();
            Assert.NotNull(lr0Parsing.CreateParsingTable());
        }
    }
}
