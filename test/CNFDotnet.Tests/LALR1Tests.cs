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
    public class LALR1Tests : BaseParsingTests
    {
        public static IEnumerable<object[]> LALR1Grammars =>
            new object[][]
            {
                new object[]
                {
                    @"S -> id | V assign E
                        V -> id
                        E -> V | num"
                },
                new object[]
                {
                    @"S' -> S
                        S -> L assign R | R
                        L -> * R | id
                        R -> L"
                },
                new object[]
                {
                    @"S -> A x B x
                        S -> B y A y
                        A -> w
                        B -> w"
                },
                new object[]
                {
                    @"S -> S ; T | T
                        T -> id | V assign E
                        V -> id
                        E -> V | num"
                },
                new object[]
                {
                    @"L -> V ( args ) | L equals Var ( )
                        V -> Var + V | id
                        Var -> id"
                },
                new object[]
                {
                    @"E -> O : OL | O
                        O -> id | OL l
                        OL -> id | ( O : OL )"
                },
                new object[]
                {
                    @"S -> a g d | a A c | b A d | b g c
                        A -> B
                        B -> g"
                }
            };

        public LALR1Tests(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        { }

        [Theory]
        [MemberData(nameof(LALR1Tests.LALR1Grammars))]
        public void LALR1_Valid_Grammar(string grammar)
        {
            CNFGrammar cnfGrammar = this.GenerateGrammar(grammar);

            IParsing<IAction> larl1Parsing = this.CreateLALR1Parsing(cnfGrammar);
            larl1Parsing.Classify();
            Assert.NotNull(larl1Parsing.CreateParsingTable());
        }
    }
}
