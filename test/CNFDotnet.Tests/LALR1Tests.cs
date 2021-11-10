using System.Collections.Generic;

using Xunit;
using Xunit.Abstractions;

using CNFDotnet.Analysis.Grammar;
using CNFDotnet.Analysis.Parsing;
using CNFDotnet.Analysis.Parsing.LR.LALR1;

namespace CNFDotnet.Tests
{
    public class LALR1Tests : BaseParsingTests
    {
        public static IEnumerable<object[]> LALR1Grammars =>
            new object[][]
            {
                new object[]
                {
                    @"S -> id S'
                        S' -> V assign E | ε
                        V -> ε
                        E -> id V | num",
                    new LR1GenericParsingTable<LALR1KernelItem>
                    (
                        ("0", "S", "s1"),
                        ("0", "id", "s2"),
                        ("1", "$", "a"),
                        ("2", "S'", "s3"),
                        ("2", "V", "s4"),
                        ("2", "$", "r2"),
                        ("2", "assign", "r3"),
                        ("3", "$", "r0"),
                        ("4", "assign", "s5"),
                        ("5", "E", "s6"),
                        ("5", "id", "s7"),
                        ("5", "num", "s8"),
                        ("6", "$", "r1"),
                        ("7", "V", "s9"),
                        ("7", "$", "r3"),
                        ("8", "$", "r5"),
                        ("9", "$", "r4")
                    )
                },
                new object[]
                {
                    @"L -> V + L | num
                        V -> Var ( Var + V ) | ε
                        Var -> ε",
                    new LR1GenericParsingTable<LALR1KernelItem>
                    (
                        ("0", "L", "s1"),
                        ("0", "V", "s2"),
                        ("0", "num", "s3"),
                        ("0", "Var", "s4"),
                        ("0", "+", "r3"),
                        ("0", "(", "r4"),
                        ("1", "$", "a"),
                        ("2", "+", "s5"),
                        ("3", "$", "r1"),
                        ("4", "(", "s6"),
                        ("5", "L", "s7"),
                        ("5", "V", "s2"),
                        ("5", "num", "s3"),
                        ("5", "Var", "s4"),
                        ("5", "+", "r3"),
                        ("5", "(", "r4"),
                        ("6", "Var", "s8"),
                        ("6", "+", "r4"),
                        ("7", "$", "r0"),
                        ("8", "+", "s9"),
                        ("9", "V", "s10"),
                        ("9", "Var", "s4"),
                        ("9", ")", "r3"),
                        ("9", "(", "r4"),
                        ("10", ")", "s11"),
                        ("11", ")", "r2"),
                        ("11", "+", "r2")
                    )
                },
                new object[]
                {
                    @"P -> M * | ε
                        M -> Q StarM | ε
                        StarM -> (* M *) | ( Q * )
                        Q -> o | ε",
                    new LR1GenericParsingTable<LALR1KernelItem>
                    (
                        ("0", "P", "s1"),
                        ("0", "M", "s2"),
                        ("0", "Q", "s3"),
                        ("0", "o", "s4"),
                        ("0", "$", "r1"),
                        ("0", "*", "r3"),
                        ("0", "(*", "r7"),
                        ("0", "(", "r7"),
                        ("1", "$", "a"),
                        ("2", "*", "s5"),
                        ("3", "StarM", "s6"),
                        ("3", "(*", "s7"),
                        ("3", "(", "s8"),
                        ("4", "*", "r6"),
                        ("4", "(*", "r6"),
                        ("4", "(", "r6"),
                        ("5", "$", "r0"),
                        ("6", "*)", "r2"),
                        ("6", "*", "r2"),
                        ("7", "M", "s9"),
                        ("7", "Q", "s3"),
                        ("7", "o", "s4"),
                        ("7", "*)", "r3"),
                        ("7", "(*", "r7"),
                        ("7", "(", "r7"),
                        ("8", "Q", "s10"),
                        ("8", "o", "s4"),
                        ("8", "*", "r7"),
                        ("9", "*)", "s11"),
                        ("10", "*", "s12"),
                        ("11", "*)", "r4"),
                        ("11", "*", "r4"),
                        ("12", ")", "s13"),
                        ("13", "*)", "r5"),
                        ("13", "*", "r5")
                    )
                },
                new object[]
                {
                    @"S -> id | V assign E
                        V -> id
                        E -> V | num",
                    new LR1GenericParsingTable<LALR1KernelItem>
                    (
                        ("0", "S", "s1"),
                        ("0", "id", "s2"),
                        ("0", "V", "s3"),
                        ("1", "$", "a"),
                        ("2", "$", "r0"),
                        ("2", "assign", "r2"),
                        ("3", "assign", "s4"),
                        ("4", "E", "s5"),
                        ("4", "V", "s6"),
                        ("4", "num", "s7"),
                        ("4", "id", "s8"),
                        ("5", "$", "r1"),
                        ("6", "$", "r3"),
                        ("7", "$", "r4"),
                        ("8", "$", "r2")
                    )
                },
                new object[]
                {
                    @"S' -> S
                        S -> L assign R | R
                        L -> * R | id
                        R -> L",
                    new LR1GenericParsingTable<LALR1KernelItem>
                    (
                        ("0", "S'", "s1"),
                        ("0", "S", "s2"),
                        ("0", "L", "s3"),
                        ("0", "R", "s4"),
                        ("0", "*", "s5"),
                        ("0", "id", "s6"),
                        ("1", "$", "a"),
                        ("2", "$", "r0"),
                        ("3", "assign", "s7"),
                        ("3", "$", "r5"),
                        ("4", "$", "r2"),
                        ("5", "R", "s8"),
                        ("5", "L", "s9"),
                        ("5", "*", "s5"),
                        ("5", "id", "s6"),
                        ("6", "$", "r4"),
                        ("6", "assign", "r4"),
                        ("7", "R", "s10"),
                        ("7", "L", "s9"),
                        ("7", "*", "s5"),
                        ("7", "id", "s6"),
                        ("8", "$", "r3"),
                        ("8", "assign", "r3"),
                        ("9", "$", "r5"),
                        ("9", "assign", "r5"),
                        ("10", "$", "r1")
                    )
                },
                new object[]
                {
                    @"S -> A x B x
                        S -> B y A y
                        A -> w
                        B -> w",
                    new LR1GenericParsingTable<LALR1KernelItem>
                    (
                        ("0", "S", "s1"),
                        ("0", "A", "s2"),
                        ("0", "B", "s3"),
                        ("0", "w", "s4"),
                        ("1", "$", "a"),
                        ("2", "x", "s5"),
                        ("3", "y", "s6"),
                        ("4", "x", "r2"),
                        ("4", "y", "r3"),
                        ("5", "B", "s7"),
                        ("5", "w", "s8"),
                        ("6", "A", "s9"),
                        ("6", "w", "s10"),
                        ("7", "x", "s11"),
                        ("8", "x", "r3"),
                        ("9", "y", "s12"),
                        ("10", "y", "r2"),
                        ("11", "$", "r0"),
                        ("12", "$", "r1")
                    )
                },
                new object[]
                {
                    @"S -> S ; T | T
                        T -> id | V assign E
                        V -> id
                        E -> V | num",
                    new LR1GenericParsingTable<LALR1KernelItem>
                    (
                        ("0", "S", "s1"),
                        ("0", "T", "s2"),
                        ("0", "id", "s3"),
                        ("0", "V", "s4"),
                        ("1", ";", "s5"),
                        ("1", "$", "a"),
                        ("2", "$", "r1"),
                        ("2", ";", "r1"),
                        ("3", "$", "r2"),
                        ("3", ";", "r2"),
                        ("3", "assign", "r4"),
                        ("4", "assign", "s6"),
                        ("5", "T", "s7"),
                        ("5", "id", "s3"),
                        ("5", "V", "s4"),
                        ("6", "E", "s8"),
                        ("6", "V", "s9"),
                        ("6", "num", "s10"),
                        ("6", "id", "s11"),
                        ("7", "$", "r0"),
                        ("7", ";", "r0"),
                        ("8", "$", "r3"),
                        ("8", ";", "r3"),
                        ("9", "$", "r5"),
                        ("9", ";", "r5"),
                        ("10", "$", "r6"),
                        ("10", ";", "r6"),
                        ("11", "$", "r4"),
                        ("11", ";", "r4")
                    )
                },
                new object[]
                {
                    @"L -> V ( args ) | L equals Var ( )
                        V -> Var + V | id
                        Var -> id",
                    new LR1GenericParsingTable<LALR1KernelItem>
                    (
                        ("0", "L", "s1"),
                        ("0", "V", "s2"),
                        ("0", "Var", "s3"),
                        ("0", "id", "s4"),
                        ("1", "equals", "s5"),
                        ("1", "$", "a"),
                        ("2", "(", "s6"),
                        ("3", "+", "s7"),
                        ("4", "(", "r3"),
                        ("4", "+", "r4"),
                        ("5", "Var", "s8"),
                        ("5", "id", "s9"),
                        ("6", "args", "s10"),
                        ("7", "V", "s11"),
                        ("7", "Var", "s3"),
                        ("7", "id", "s4"),
                        ("8", "(", "s12"),
                        ("9", "(", "r4"),
                        ("10", ")", "s13"),
                        ("11", "(", "r2"),
                        ("12", ")", "s14"),
                        ("13", "$", "r0"),
                        ("13", "equals", "r0"),
                        ("14", "$", "r1"),
                        ("14", "equals", "r1")
                    )
                },
                new object[]
                {
                    @"E -> O : OL | O
                        O -> id | OL l
                        OL -> id | ( O : OL )",
                    new LR1GenericParsingTable<LALR1KernelItem>
                    (
                        ("0", "E", "s1"),
                        ("0", "O", "s2"),
                        ("0", "id", "s3"),
                        ("0", "OL", "s4"),
                        ("0", "(", "s5"),
                        ("1", "$", "a"),
                        ("2", ":", "s6"),
                        ("2", "$", "r1"),
                        ("3", ":", "r2"),
                        ("3", "$", "r2"),
                        ("3", "l", "r4"),
                        ("4", "l", "s7"),
                        ("5", "O", "s8"),
                        ("5", "id", "s3"),
                        ("5", "OL", "s4"),
                        ("5", "(", "s5"),
                        ("6", "OL", "s9"),
                        ("6", "id", "s10"),
                        ("6", "(", "s5"),
                        ("7", ":", "r3"),
                        ("7", "$", "r3"),
                        ("8", ":", "s11"),
                        ("9", "$", "r0"),
                        ("10", ")", "r4"),
                        ("10", "$", "r4"),
                        ("11", "OL", "s12"),
                        ("11", "id", "s10"),
                        ("11", "(", "s5"),
                        ("12", ")", "s13"),
                        ("13", ")", "r5"),
                        ("13", "$", "r5"),
                        ("13", "l", "r5")
                    )
                },
                new object[]
                {
                    @"S -> a g d | a A c | b A d | b g c
                        A -> B
                        B -> g",
                    new LR1GenericParsingTable<LALR1KernelItem>
                    (
                        ("0", "S", "s1"),
                        ("0", "a", "s2"),
                        ("0", "b", "s3"),
                        ("1", "$", "a"),
                        ("2", "g", "s4"),
                        ("2", "A", "s5"),
                        ("2", "B", "s6"),
                        ("3", "A", "s7"),
                        ("3", "g", "s8"),
                        ("3", "B", "s6"),
                        ("4", "d", "s9"),
                        ("4", "c", "r5"),
                        ("5", "c", "s10"),
                        ("6", "d", "r4"),
                        ("6", "c", "r4"),
                        ("7", "d", "s11"),
                        ("8", "c", "s12"),
                        ("8", "d", "r5"),
                        ("9", "$", "r0"),
                        ("10", "$", "r1"),
                        ("11", "$", "r2"),
                        ("12", "$", "r3")
                    )
                }
            };

        public LALR1Tests(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        { }

#pragma warning disable CA1707
        [Theory]
        [MemberData(nameof(LALR1Tests.LALR1Grammars))]
        public void LALR1_Valid_Grammar
        (
            string grammar,
            LR1GenericParsingTable<LALR1KernelItem> verification
        )
        {
            CNFGrammar cnfGrammar = this.GenerateGrammar(grammar);

            IParsing<LALR1Action> larl1Parsing
                = BaseParsingTests.CreateLALR1Parsing(cnfGrammar);
            larl1Parsing.Classify();

            IParsingTable<LALR1Action> lr1ParsingTable
                = larl1Parsing.CreateParsingTable();
            Assert.NotNull(lr1ParsingTable);

            this.TestOutputHelper.WriteLine("Verifying parsing table");
            verification.Verify(cnfGrammar, lr1ParsingTable);
        }
#pragma warning restore CA1707
    }
}
