using System.Collections.Generic;

using Xunit;
using Xunit.Abstractions;

using CNFDotnet.Analysis.Grammar;
using CNFDotnet.Analysis.Parsing;
using CNFDotnet.Analysis.Parsing.LR;
using CNFDotnet.Analysis.Parsing.LR.SLR1;

namespace CNFDotnet.Tests
{
    public class SLR1Tests : BaseParsingTests
    {
        public static IEnumerable<object[]> SLR1Grammars =>
            new object[][]
            {
                new object[]
                {
                    @"A -> A b | c",
                    new LR1GenericParsingTable<LR0KernelItem>
                    (
                        ("0", "A", "s1"),
                        ("0", "c", "s2"),
                        ("1", "b", "s3"),
                        ("1", "$", "a"),
                        ("2", "$", "r1"),
                        ("2", "b", "r1"),
                        ("3", "$", "r0"),
                        ("3", "b", "r0")
                    )
                },
                new object[]
                {
                    @"EXP -> EXP add TERM | TERM
                        TERM -> id | id INDEX | let STMTS in EXP end
                        STMTS -> STMTS STMT | ε
                        STMT -> LEXP assign EXP semi
                        LEXP -> LEXP INDEX | id
                        INDEX -> lpar EXP rpar",
                    new LR1GenericParsingTable<LR0KernelItem>
                    (
                        ("0", "EXP", "s1"),
                        ("0", "TERM", "s2"),
                        ("0", "id", "s3"),
                        ("0", "let", "s4"),
                        ("1", "add", "s5"),
                        ("1", "$", "a"),
                        ("2", "$", "r1"),
                        ("2", "add", "r1"),
                        ("2", "end", "r1"),
                        ("2", "semi", "r1"),
                        ("2", "rpar", "r1"),
                        ("3", "INDEX", "s6"),
                        ("3", "lpar", "s7"),
                        ("3", "$", "r2"),
                        ("3", "add", "r2"),
                        ("3", "end", "r2"),
                        ("3", "semi", "r2"),
                        ("3", "rpar", "r2"),
                        ("4", "STMTS", "s8"),
                        ("4", "in", "r6"),
                        ("4", "id", "r6"),
                        ("5", "TERM", "s9"),
                        ("5", "id", "s3"),
                        ("5", "let", "s4"),
                        ("6", "$", "r3"),
                        ("6", "add", "r3"),
                        ("6", "end", "r3"),
                        ("6", "semi", "r3"),
                        ("6", "rpar", "r3"),
                        ("7", "EXP", "s10"),
                        ("7", "TERM", "s2"),
                        ("7", "id", "s3"),
                        ("7", "let", "s4"),
                        ("8", "in", "s11"),
                        ("8", "STMT", "s12"),
                        ("8", "LEXP", "s13"),
                        ("8", "id", "s14"),
                        ("9", "$", "r0"),
                        ("9", "add", "r0"),
                        ("9", "end", "r0"),
                        ("9", "semi", "r0"),
                        ("9", "rpar", "r0"),
                        ("10", "rpar", "s15"),
                        ("10", "add", "s5"),
                        ("11", "EXP", "s16"),
                        ("11", "TERM", "s2"),
                        ("11", "id", "s3"),
                        ("11", "let", "s4"),
                        ("12", "in", "r5"),
                        ("12", "id", "r5"),
                        ("13", "assign", "s17"),
                        ("13", "INDEX", "s18"),
                        ("13", "lpar", "s7"),
                        ("14", "assign", "r9"),
                        ("14", "lpar", "r9"),
                        ("15", "assign", "r10"),
                        ("15", "lpar", "r10"),
                        ("15", "$", "r10"),
                        ("15", "add", "r10"),
                        ("15", "end", "r10"),
                        ("15", "semi", "r10"),
                        ("15", "rpar", "r10"),
                        ("16", "end", "s19"),
                        ("16", "add", "s5"),
                        ("17", "EXP", "s20"),
                        ("17", "TERM", "s2"),
                        ("17", "id", "s3"),
                        ("17", "let", "s4"),
                        ("18", "assign", "r8"),
                        ("18", "lpar", "r8"),
                        ("19", "$", "r4"),
                        ("19", "add", "r4"),
                        ("19", "end", "r4"),
                        ("19", "semi", "r4"),
                        ("19", "rpar", "r4"),
                        ("20", "semi", "s21"),
                        ("20", "add", "s5"),
                        ("21", "in", "r7"),
                        ("21", "id", "r7")
                    )
                },
                new object[]
                {
                    @"S -> E
                        E -> B + T | B * T | T
                        B -> C
                        C -> D
                        D -> E
                        T -> X
                        X -> ε",
                    new LR1GenericParsingTable<LR0KernelItem>
                    (
                        ("0", "S", "s1"),
                        ("0", "E", "s2"),
                        ("0", "B", "s3"),
                        ("0", "T", "s4"),
                        ("0", "C", "s5"),
                        ("0", "X", "s6"),
                        ("0", "D", "s7"),
                        ("0", "$", "r8"),
                        ("0", "+", "r8"),
                        ("0", "*", "r8"),
                        ("1", "$", "a"),
                        ("2", "$", "r0"),
                        ("2", "+", "r6"),
                        ("2", "*", "r6"),
                        ("3", "+", "s8"),
                        ("3", "*", "s9"),
                        ("4", "$", "r3"),
                        ("4", "+", "r3"),
                        ("4", "*", "r3"),
                        ("5", "+", "r4"),
                        ("5", "*", "r4"),
                        ("6", "$", "r7"),
                        ("6", "+", "r7"),
                        ("6", "*", "r7"),
                        ("7", "+", "r5"),
                        ("7", "*", "r5"),
                        ("8", "T", "s10"),
                        ("8", "X", "s6"),
                        ("8", "$", "r8"),
                        ("8", "+", "r8"),
                        ("8", "*", "r8"),
                        ("9", "T", "s11"),
                        ("9", "X", "s6"),
                        ("9", "$", "r8"),
                        ("9", "+", "r8"),
                        ("9", "*", "r8"),
                        ("10", "$", "r1"),
                        ("10", "+", "r1"),
                        ("10", "*", "r1"),
                        ("11", "$", "r2"),
                        ("11", "+", "r2"),
                        ("11", "*", "r2")
                    )
                },
                new object[]
                {
                    @"A -> a B | a A C | ε
                        B -> d B | e
                        C -> c A",
                    new LR1GenericParsingTable<LR0KernelItem>
                    (
                        ("0", "A", "s1"),
                        ("0", "a", "s2"),
                        ("0", "$", "r2"),
                        ("0", "c", "r2"),
                        ("1", "$", "a"),
                        ("2", "B", "s3"),
                        ("2", "A", "s4"),
                        ("2", "d", "s5"),
                        ("2", "e", "s6"),
                        ("2", "a", "s2"),
                        ("2", "$", "r2"),
                        ("2", "c", "r2"),
                        ("3", "$", "r0"),
                        ("3", "c", "r0"),
                        ("4", "C", "s7"),
                        ("4", "c", "s8"),
                        ("5", "B", "s9"),
                        ("5", "d", "s5"),
                        ("5", "e", "s6"),
                        ("6", "$", "r4"),
                        ("6", "c", "r4"),
                        ("7", "$", "r1"),
                        ("7", "c", "r1"),
                        ("8", "A", "s10"),
                        ("8", "a", "s2"),
                        ("8", "$", "r2"),
                        ("8", "c", "r2"),
                        ("9", "$", "r3"),
                        ("9", "c", "r3"),
                        ("10", "$", "r5"),
                        ("10", "c", "r5")
                    )
                },
                new object[]
                {
                    @"TERM -> id | id INDEX | let LEXP
                        LEXP -> INDEX LEXP | id
                        INDEX -> lpar TERM rpar",
                    new LR1GenericParsingTable<LR0KernelItem>
                    (
                        ("0", "TERM", "s1"),
                        ("0", "id", "s2"),
                        ("0", "let", "s3"),
                        ("1", "$", "a"),
                        ("2", "INDEX", "s4"),
                        ("2", "lpar", "s5"),
                        ("2", "$", "r0"),
                        ("2", "rpar", "r0"),
                        ("3", "LEXP", "s6"),
                        ("3", "INDEX", "s7"),
                        ("3", "id", "s8"),
                        ("3", "lpar", "s5"),
                        ("4", "$", "r1"),
                        ("4", "rpar", "r1"),
                        ("5", "TERM", "s9"),
                        ("5", "id", "s2"),
                        ("5", "let", "s3"),
                        ("6", "$", "r2"),
                        ("6", "rpar", "r2"),
                        ("7", "LEXP", "s10"),
                        ("7", "INDEX", "s7"),
                        ("7", "id", "s8"),
                        ("7", "lpar", "s5"),
                        ("8", "$", "r4"),
                        ("8", "rpar", "r4"),
                        ("9", "rpar", "s11"),
                        ("10", "$", "r3"),
                        ("10", "rpar", "r3"),
                        ("11", "id", "r5"),
                        ("11", "lpar", "r5"),
                        ("11", "$", "r5"),
                        ("11", "rpar", "r5")
                    )
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
                        Macro -> id",
                    new LR1GenericParsingTable<LR0KernelItem>
                    (
                        ("0", "Line", "s1"),
                        ("0", "Op", "s2"),
                        ("0", "Label", "s3"),
                        ("0", "Inst", "s4"),
                        ("0", "id", "s5"),
                        ("0", "pneumonic", "s6"),
                        ("0", "Macro", "s7"),
                        ("1", "$", "a"),
                        ("2", "$", "r0"),
                        ("3", "$", "r1"),
                        ("4", "Operands", "s8"),
                        ("4", "Src", "s9"),
                        ("4", "Dest", "s10"),
                        ("4", "num", "s11"),
                        ("4", "id", "s12"),
                        ("4", "reg", "s13"),
                        ("4", "(", "s14"),
                        ("5", ":", "s15"),
                        ("5", "num", "r12"),
                        ("5", "id", "r12"),
                        ("5", "reg", "r12"),
                        ("5", "(", "r12"),
                        ("6", "num", "r10"),
                        ("6", "id", "r10"),
                        ("6", "reg", "r10"),
                        ("6", "(", "r10"),
                        ("7", "num", "r11"),
                        ("7", "id", "r11"),
                        ("7", "reg", "r11"),
                        ("7", "(", "r11"),
                        ("8", "$", "r2"),
                        ("9", "Src", "s16"),
                        ("9", "Dest", "s10"),
                        ("9", "num", "s11"),
                        ("9", "id", "s12"),
                        ("9", "reg", "s13"),
                        ("9", "(", "s14"),
                        ("10", "num", "r8"),
                        ("10", "id", "r8"),
                        ("10", "reg", "r8"),
                        ("10", "(", "r8"),
                        ("10", ")", "r8"),
                        ("11", "num", "r9"),
                        ("11", "id", "r9"),
                        ("11", "reg", "r9"),
                        ("11", "(", "r9"),
                        ("11", ")", "r9"),
                        ("12", "num", "r5"),
                        ("12", "id", "r5"),
                        ("12", "reg", "r5"),
                        ("12", "(", "r5"),
                        ("12", ")", "r5"),
                        ("12", "$", "r5"),
                        ("13", "num", "r6"),
                        ("13", "id", "r6"),
                        ("13", "reg", "r6"),
                        ("13", "(", "r6"),
                        ("13", ")", "r6"),
                        ("13", "$", "r6"),
                        ("14", "Src", "s17"),
                        ("14", "Dest", "s10"),
                        ("14", "num", "s11"),
                        ("14", "id", "s12"),
                        ("14", "reg", "s13"),
                        ("14", "(", "s14"),
                        ("15", "$", "r3"),
                        ("16", "Dest", "s18"),
                        ("16", "id", "s12"),
                        ("16", "reg", "s13"),
                        ("16", "(", "s14"),
                        ("17", ")", "s19"),
                        ("18", "$", "r4"),
                        ("19", "num", "r7"),
                        ("19", "id", "r7"),
                        ("19", "reg", "r7"),
                        ("19", "(", "r7"),
                        ("19", ")", "r7"),
                        ("19", "$", "r7")
                    )
                }
            };

        public SLR1Tests(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        { }

#pragma warning disable CA1707
        [Theory]
        [MemberData(nameof(SLR1Tests.SLR1Grammars))]
        public void SLR1_Valid_Grammar
            (string grammar, LR1GenericParsingTable<LR0KernelItem> verification)
        {
            CNFGrammar cnfGrammar = this.GenerateGrammar(grammar);

            IParsing<SLR1Action> slr1Parsing
                = BaseParsingTests.CreateSLR1Parsing(cnfGrammar);
            slr1Parsing.Classify();

            IParsingTable<SLR1Action> slr1ParsingTable
                = slr1Parsing.CreateParsingTable();
            Assert.NotNull(slr1ParsingTable);

            this.TestOutputHelper.WriteLine("Verifying parsing table");
            verification.Verify(cnfGrammar, slr1ParsingTable);
        }
#pragma warning restore CA1707
    }
}
