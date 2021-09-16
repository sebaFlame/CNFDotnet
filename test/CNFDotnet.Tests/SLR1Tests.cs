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
    public class SLR1Tests : BaseParsingTests
    {
        public static IEnumerable<object[]> SLR1Grammars =>
            new object[][]
            {
                new object[]
                {
                    @"A -> A b | c"
                },
                new object[]
                {
                    @"EXP -> EXP add TERM | TERM
                        TERM -> id | id INDEX | let STMTS in EXP end
                        STMTS -> STMTS STMT | ε
                        STMT -> LEXP assign EXP semi
                        LEXP -> LEXP INDEX | id
                        INDEX -> lpar EXP rpar"
                },
                new object[]
                {
                    @"S -> E
                        E -> B + T | B * T | T
                        B -> C
                        C -> D
                        D -> E
                        T -> X
                        X -> ε"
                },
                new object[]
                {
                    @"A -> a B | a A C | ε
                        B -> d B | e
                        C -> c A"
                },
                new object[]
                {
                    @"TERM -> id | id INDEX | let LEXP
                        LEXP -> INDEX LEXP | id
                        INDEX -> lpar TERM rpar"
                },
                new object[]
                {
                    @"Line -> Op | Label
                        Op -> Inst Operands
                        Label -> id :
                        Operands -> Src Src Dest
                        Dest -> id | reg | ( Src )
                        Src -> Dest | num
                        Inst -> pneumonic | Macro
                        Macro -> id"
                }
            };

        public SLR1Tests(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        { }

        [Theory]
        [MemberData(nameof(SLR1Tests.SLR1Grammars))]
        public void SLR1_Valid_Grammar(string grammar)
        {
            CNFGrammar cnfGrammar = this.GenerateGrammar(grammar);

            IParsing<IAction> slr1Parsing = this.CreateSLR1Parsing(cnfGrammar);
            slr1Parsing.Classify();
            Assert.NotNull(slr1Parsing.CreateParsingTable());
        }
    }
}
