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
    public class LR0Tests : BaseParsingTests
    {
        public static IEnumerable<object[]> LR0Grammars =>
            new object[][]
            {
                new object[]
                {
                    @"A -> C | B
                        C -> a C a | b
                        B -> a B | c"
                },
                new object[]
                {
                    @"S -> A | B
                        A -> x A | a
                        B -> x B | b"
                },
                new object[]
                {
                    @"S -> Parens | StarParens
                        Parens -> ( Parens ) | ( )
                    StarParens -> ( StarParens *) | ( *)"
                },
                new object[]
                {
                    @"S -> T | A
                        A -> B
                        B -> C
                        D -> d | d D | ε"
                }
            };

        public LR0Tests(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        { }

        [Theory]
        [MemberDataAttribute(nameof(LR0Tests.LR0Grammars))]
        public void LR0_Valid_Grammar(string grammar)
        {
            CNFGrammar cnfGrammar = this.GenerateGrammar(grammar);

            IParsing<IAction> ll1Parsing = this.CreateLL1Parsing(cnfGrammar);
            Assert.Throws<LL1ClassificationException>(() => ll1Parsing.Classify());

            IParsing<IAction> lr0Parsing = this.CreateLR0Parsing(cnfGrammar);
            lr0Parsing.Classify();
            Assert.NotNull(lr0Parsing.CreateParsingTable());
        }
    }
}
