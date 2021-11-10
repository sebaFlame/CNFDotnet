using System.Collections.Generic;

using Xunit;
using Xunit.Abstractions;

using CNFDotnet.Analysis.Grammar;
using CNFDotnet.Analysis.Parsing;
using CNFDotnet.Analysis.Parsing.LR.LR0;

namespace CNFDotnet.Tests
{
    public class LR0Tests : BaseParsingTests
    {
        public static IEnumerable<object[]> LR0Grammars =>
            new object[][]
            {
               new object[]
               {
                   @"A -> B | x C | y A
                       B -> C B
                       C -> r",
                    new LR0GenericParsingTable
                    (
                        ("0", "A", "s1"),
                        ("0", "B", "s2"),
                        ("0", "x", "s3"),
                        ("0", "y", "s4"),
                        ("0", "C", "s5"),
                        ("0", "r", "s6"),
                        ("1", "", "a"),
                        ("2", "", "r0"),
                        ("3", "C", "s7"),
                        ("3", "r", "s6"),
                        ("4", "A", "s8"),
                        ("4", "B", "s2"),
                        ("4", "x", "s3"),
                        ("4", "y", "s4"),
                        ("4", "C", "s5"),
                        ("4", "r", "s6"),
                        ("5", "B", "s9"),
                        ("5", "C", "s5"),
                        ("5", "r", "s6"),
                        ("6", "", "r4"),
                        ("7", "", "r1"),
                        ("8", "", "r2"),
                        ("9", "", "r3")
                    )
               },
               new object[]
                {
                    @"A -> y B | x | B C
                        B -> z B | u
                        C -> s",
                    new LR0GenericParsingTable
                    (
                        ("0", "A", "s1"),
                        ("0", "y", "s2"),
                        ("0", "x", "s3"),
                        ("0", "B", "s4"),
                        ("0", "z", "s5"),
                        ("0", "u", "s6"),
                        ("1", "", "a"),
                        ("2", "B", "s7"),
                        ("2", "z", "s5"),
                        ("2", "u", "s6"),
                        ("3", "", "r1"),
                        ("4", "C", "s8"),
                        ("4", "s", "s9"),
                        ("5", "B", "s10"),
                        ("5", "z", "s5"),
                        ("5", "u", "s6"),
                        ("6", "", "r4"),
                        ("7", "", "r0"),
                        ("8", "", "r2"),
                        ("9", "", "r5"),
                        ("10", "", "r3")
                    )
                },
                new object[]
                {
                    @"S -> ( Ses ) | (* *)
                        Ses -> S SL
                        SL -> ; SL | S",
                    new LR0GenericParsingTable
                    (
                        ("0", "S", "s1"),
                        ("0", "(", "s2"),
                        ("0", "(*", "s3"),
                        ("1", "", "a"),
                        ("2", "Ses", "s4"),
                        ("2", "S", "s5"),
                        ("2", "(", "s2"),
                        ("2", "(*", "s3"),
                        ("3", "*)", "s6"),
                        ("4", ")", "s7"),
                        ("5", "SL", "s8"),
                        ("5", ";", "s9"),
                        ("5", "S", "s10"),
                        ("5", "(", "s2"),
                        ("5", "(*", "s3"),
                        ("6", "", "r1"),
                        ("7", "", "r0"),
                        ("8", "", "r2"),
                        ("9", "SL", "s11"),
                        ("9", ";", "s9"),
                        ("9", "S", "s10"),
                        ("9", "(", "s2"),
                        ("9", "(*", "s3"),
                        ("10", "", "r4"),
                        ("11", "", "r3")
                    )
                },
                new object[]
                {
                    @"S -> Block | ( )
                        Block -> ( stmt )",
                    new LR0GenericParsingTable
                    (
                        ("0", "S", "s1"),
                        ("0", "Block", "s2"),
                        ("0", "(", "s3"),
                        ("1", "", "a"),
                        ("2", "", "r0"),
                        ("3", ")", "s4"),
                        ("3", "stmt", "s5"),
                        ("4", "", "r1"),
                        ("5", ")", "s6"),
                        ("6", "", "r2")
                    )
                },
                new object[]
                {
                    @"S -> Assign | Inc
                        Assign -> Lv equals Rv
                        Inc -> Lv ++ | Lv //
                        Rv -> Lv | num
                        Lv -> id",
                    new LR0GenericParsingTable
                    (
                        ("0", "S", "s1"),
                        ("0", "Assign", "s2"),
                        ("0", "Inc", "s3"),
                        ("0", "Lv", "s4"),
                        ("0", "id", "s5"),
                        ("1", "", "a"),
                        ("2", "", "r0"),
                        ("3", "", "r1"),
                        ("4", "equals", "s6"),
                        ("4", "++", "s7"),
                        ("4", "//", "s8"),
                        ("5", "", "r7"),
                        ("6", "Rv", "s9"),
                        ("6", "Lv", "s10"),
                        ("6", "num", "s11"),
                        ("6", "id", "s5"),
                        ("7", "", "r3"),
                        ("8", "", "r4"),
                        ("9", "", "r2"),
                        ("10", "", "r5"),
                        ("11", "", "r6")
                    )
                },
                new object[]
                {
                    @"Emoticon -> Happy | Sad
                        Happy -> : )
                        Sad -> : (",
                    new LR0GenericParsingTable
                    (
                        ("0", "Emoticon", "s1"),
                        ("0", "Happy", "s2"),
                        ("0", "Sad", "s3"),
                        ("0", ":", "s4"),
                        ("1", "", "a"),
                        ("2", "", "r0"),
                        ("3", "", "r1"),
                        ("4", ")", "s5"),
                        ("4", "(", "s6"),
                        ("5", "", "r2"),
                        ("6", "", "r3")
                    )
                },
               new object[]
                {
                    @"A -> C | B
                        C -> a C a | b
                        B -> a B | c",
                    new LR0GenericParsingTable
                    (
                        ("0", "A", "s1"),
                        ("0", "C", "s2"),
                        ("0", "B", "s3"),
                        ("0", "a", "s4"),
                        ("0", "b", "s5"),
                        ("0", "c", "s6"),
                        ("1", "", "a"),
                        ("2", "", "r0"),
                        ("3", "", "r1"),
                        ("4", "C", "s7"),
                        ("4", "B", "s8"),
                        ("4", "a", "s4"),
                        ("4", "b", "s5"),
                        ("4", "c", "s6"),
                        ("5", "", "r3"),
                        ("6", "", "r5"),
                        ("7", "a", "s9"),
                        ("8", "", "r4"),
                        ("9", "", "r2")
                    )
                },
                new object[]
                {
                    @"S -> A | B
                        A -> x A | a
                        B -> x B | b",
                    new LR0GenericParsingTable
                    (
                        ("0", "S", "s1"),
                        ("0", "A", "s2"),
                        ("0", "B", "s3"),
                        ("0", "x", "s4"),
                        ("0", "a", "s5"),
                        ("0", "b", "s6"),
                        ("1", "", "a"),
                        ("2", "", "r0"),
                        ("3", "", "r1"),
                        ("4", "A", "s7"),
                        ("4", "B", "s8"),
                        ("4", "x", "s4"),
                        ("4", "a", "s5"),
                        ("4", "b", "s6"),
                        ("5", "", "r3"),
                        ("6", "", "r5"),
                        ("7", "", "r2"),
                        ("8", "", "r4")
                    )
                },
                new object[]
                {
                    @"S -> Parens | StarParens
                        Parens -> ( Parens ) | ( )
                    StarParens -> ( StarParens *) | ( *)",
                    new LR0GenericParsingTable
                    (
                        ("0", "S", "s1"),
                        ("0", "Parens", "s2"),
                        ("0", "StarParens", "s3"),
                        ("0", "(", "s4"),
                        ("1", "", "a"),
                        ("2", "", "r0"),
                        ("3", "", "r1"),
                        ("4", "Parens", "s5"),
                        ("4", ")", "s6"),
                        ("4", "StarParens", "s7"),
                        ("4", "*)", "s8"),
                        ("4", "(", "s4"),
                        ("5", ")", "s9"),
                        ("6", "", "r3"),
                        ("7", "*)", "s10"),
                        ("8", "", "r5"),
                        ("9", "", "r2"),
                        ("10", "", "r4")
                    )
                },
                new object[]
                {
                    @"S -> T | A
                        A -> B
                        B -> C
                        D -> d | d D | ε",
                    new LR0GenericParsingTable
                    (
                        ("0", "S", "s1"),
                        ("0", "T", "s2"),
                        ("0", "A", "s3"),
                        ("0", "B", "s4"),
                        ("0", "C", "s5"),
                        ("1", "", "a"),
                        ("2", "", "r0"),
                        ("3", "", "r1"),
                        ("4", "", "r2"),
                        ("5", "", "r3")
                    )
                }

            };

        public LR0Tests(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        { }

#pragma warning disable CA1707
        [Theory]
        [MemberData(nameof(LR0Tests.LR0Grammars))]
        public void LR0_Valid_Grammar
            (string grammar, LR0GenericParsingTable verification)
        {
            CNFGrammar cnfGrammar = this.GenerateGrammar(grammar);

            IParsing<LR0Action> lr0Parsing
                = BaseParsingTests.CreateLR0Parsing(cnfGrammar);
            lr0Parsing.Classify();

            IParsingTable<LR0Action> lr0ParsingTable
                = lr0Parsing.CreateParsingTable();
            Assert.NotNull(lr0ParsingTable);

            this.TestOutputHelper.WriteLine("Verifying parsing table");
            verification.Verify(cnfGrammar, lr0ParsingTable);
        }
#pragma warning restore CA1707
    }
}
