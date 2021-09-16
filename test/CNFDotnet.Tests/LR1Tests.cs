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
    public class LR1Tests : BaseParsingTests
    {
        public static IEnumerable<object[]> LR1Grammars =>
            new object[][]
            {
                new object[]
                {
                    @"E -> d D | D | F
                        F -> e C | C
                        D -> d e B b | e A c
                        C -> e d B c | d A b
                        B -> a
                        A -> a"
                },
                new object[]
                {
                    @"S -> S R | ε
                        R -> s StructVar | u NVar
                        StructVar -> Var ; | Subvar :
                        NVar -> Var : | Subvar ;
                        Var -> V
                        Subvar -> V
                        V -> id"
                },
                new object[]
                {
                    @"S -> S ; C | ( C ) | * D *
                        C -> A x | B y
                        D -> A y | B x
                        A -> E | id
                        B -> E | num
                        E -> ! | ( S )"
                },
                new object[]
                {
                    @"S -> a a A | a b B
                        A -> C a | D b
                        B -> C b | D a
                        C -> E
                        D -> E
                        E -> ε"
                },
                new object[]
                {
                    @"Value -> number V | number
                        V -> f Real | i Int
                        Real -> IOpt dot | BOpt +
                        Int -> IOpt + | BOpt dot
                        BOpt -> Opt
                        IOpt -> Opt
                        Opt -> ε"
                },
                new object[]
                {
                    @"S -> ' Q | P
                        Q -> T W | E ;
                        P -> T ; | E W
                        T -> U
                        E -> U
                        U -> '
                        W -> * W | 8 W | ε"
                }
            };

        public LR1Tests(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        { }

        [Theory]
        [MemberData(nameof(LR1Tests.LR1Grammars))]
        public void LR1_Valid_Grammar(string grammar)
        {
            CNFGrammar cnfGrammar = this.GenerateGrammar(grammar);

            IParsing<IAction> lr1Parsing = this.CreateLR1Parsing(cnfGrammar);
            lr1Parsing.Classify();
            Assert.NotNull(lr1Parsing.CreateParsingTable());
        }
    }
}
