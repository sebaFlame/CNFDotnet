using System.Collections.Generic;

using Xunit;
using Xunit.Abstractions;

using CNFDotnet.Analysis.Grammar;
using CNFDotnet.Analysis.Parsing;
using CNFDotnet.Analysis.Parsing.LR.LR1;

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
                        A -> a",
                    new LR1GenericParsingTable<LR1KernelItem>
                    (
                        ("0", "E", "s1"),
                        ("0", "d", "s2"),
                        ("0", "D", "s3"),
                        ("0", "F", "s4"),
                        ("0", "e", "s5"),
                        ("0", "C", "s6"),
                        ("1", "$", "a"),
                        ("2", "D", "s7"),
                        ("2", "e", "s8"),
                        ("2", "A", "s9"),
                        ("2", "d", "s10"),
                        ("2", "a", "s11"),
                        ("3", "$", "r1"),
                        ("4", "$", "r2"),
                        ("5", "A", "s12"),
                        ("5", "C", "s13"),
                        ("5", "d", "s14"),
                        ("5", "a", "s15"),
                        ("5", "e", "s16"),
                        ("6", "$", "r4"),
                        ("7", "$", "r0"),
                        ("8", "B", "s17"),
                        ("8", "A", "s12"),
                        ("8", "a", "s18"),
                        ("9", "b", "s19"),
                        ("10", "e", "s20"),
                        ("11", "b", "r10"),
                        ("12", "c", "s21"),
                        ("13", "$", "r3"),
                        ("14", "B", "s22"),
                        ("14", "A", "s9"),
                        ("14", "a", "s23"),
                        ("15", "c", "r10"),
                        ("16", "d", "s24"),
                        ("17", "b", "s25"),
                        ("18", "b", "r9"),
                        ("18", "c", "r10"),
                        ("19", "$", "r8"),
                        ("20", "B", "s17"),
                        ("20", "a", "s26"),
                        ("21", "$", "r6"),
                        ("22", "c", "s27"),
                        ("23", "c", "r9"),
                        ("23", "b", "r10"),
                        ("24", "B", "s22"),
                        ("24", "a", "s28"),
                        ("25", "$", "r5"),
                        ("26", "b", "r9"),
                        ("27", "$", "r7"),
                        ("28", "c", "r9")
                    )
                },
                new object[]
                {
                    @"S -> S R | ε
                        R -> s StructVar | u NVar
                        StructVar -> Var ; | Subvar :
                        NVar -> Var : | Subvar ;
                        Var -> V
                        Subvar -> V
                        V -> id",
                    new LR1GenericParsingTable<LR1KernelItem>
                    (
                        ("0", "S", "s1"),
                        ("0", "$", "r1"),
                        ("0", "s", "r1"),
                        ("0", "u", "r1"),
                        ("1", "R", "s2"),
                        ("1", "s", "s3"),
                        ("1", "u", "s4"),
                        ("1", "$", "a"),
                        ("2", "$", "r0"),
                        ("2", "s", "r0"),
                        ("2", "u", "r0"),
                        ("3", "StructVar", "s5"),
                        ("3", "Var", "s6"),
                        ("3", "Subvar", "s7"),
                        ("3", "V", "s8"),
                        ("3", "id", "s9"),
                        ("4", "NVar", "s10"),
                        ("4", "Var", "s11"),
                        ("4", "Subvar", "s12"),
                        ("4", "V", "s13"),
                        ("4", "id", "s9"),
                        ("5", "$", "r2"),
                        ("5", "s", "r2"),
                        ("5", "u", "r2"),
                        ("6", ";", "s14"),
                        ("7", ":", "s15"),
                        ("8", ";", "r8"),
                        ("8", ":", "r9"),
                        ("9", ";", "r10"),
                        ("9", ":", "r10"),
                        ("10", "$", "r3"),
                        ("10", "s", "r3"),
                        ("10", "u", "r3"),
                        ("11", ":", "s16"),
                        ("12", ";", "s17"),
                        ("13", ":", "r8"),
                        ("13", ";", "r9"),
                        ("14", "$", "r4"),
                        ("14", "s", "r4"),
                        ("14", "u", "r4"),
                        ("15", "$", "r5"),
                        ("15", "s", "r5"),
                        ("15", "u", "r5"),
                        ("16", "$", "r6"),
                        ("16", "s", "r6"),
                        ("16", "u", "r6"),
                        ("17", "$", "r7"),
                        ("17", "s", "r7"),
                        ("17", "u", "r7")
                    )
                },
                new object[]
                {
                    @"S -> S ; C | ( C ) | * D *
                        C -> A x | B y
                        D -> A y | B x
                        A -> E | id
                        B -> E | num
                        E -> ! | ( S )",
                    new LR1GenericParsingTable<LR1KernelItem>
                    (
                        ("0", "S", "s1"),
                        ("0", "(", "s2"),
                        ("0", "*", "s3"),
                        ("1", ";", "s4"),
                        ("1", "$", "a"),
                        ("2", "C", "s5"),
                        ("2", "A", "s6"),
                        ("2", "B", "s7"),
                        ("2", "E", "s8"),
                        ("2", "id", "s9"),
                        ("2", "num", "s10"),
                        ("2", "!", "s11"),
                        ("2", "(", "s12"),
                        ("3", "D", "s13"),
                        ("3", "A", "s14"),
                        ("3", "B", "s15"),
                        ("3", "E", "s16"),
                        ("3", "id", "s17"),
                        ("3", "num", "s18"),
                        ("3", "!", "s11"),
                        ("3", "(", "s12"),
                        ("4", "C", "s19"),
                        ("4", "A", "s20"),
                        ("4", "B", "s21"),
                        ("4", "E", "s8"),
                        ("4", "id", "s9"),
                        ("4", "num", "s10"),
                        ("4", "!", "s11"),
                        ("4", "(", "s12"),
                        ("5", ")", "s22"),
                        ("6", "x", "s23"),
                        ("7", "y", "s24"),
                        ("8", "x", "r7"),
                        ("8", "y", "r9"),
                        ("9", "x", "r8"),
                        ("10", "y", "r10"),
                        ("11", "x", "r11"),
                        ("11", "y", "r11"),
                        ("12", "S", "s25"),
                        ("12", "(", "s26"),
                        ("12", "*", "s27"),
                        ("13", "*", "s28"),
                        ("14", "y", "s29"),
                        ("15", "x", "s30"),
                        ("16", "y", "r7"),
                        ("16", "x", "r9"),
                        ("17", "y", "r8"),
                        ("18", "x", "r10"),
                        ("19", "$", "r0"),
                        ("19", ";", "r0"),
                        ("20", "x", "s31"),
                        ("21", "y", "s32"),
                        ("22", "$", "r1"),
                        ("22", ";", "r1"),
                        ("23", ")", "r3"),
                        ("24", ")", "r4"),
                        ("25", ")", "s33"),
                        ("25", ";", "s34"),
                        ("26", "C", "s35"),
                        ("26", "A", "s6"),
                        ("26", "B", "s7"),
                        ("26", "E", "s8"),
                        ("26", "id", "s9"),
                        ("26", "num", "s10"),
                        ("26", "!", "s11"),
                        ("26", "(", "s12"),
                        ("27", "D", "s36"),
                        ("27", "A", "s14"),
                        ("27", "B", "s15"),
                        ("27", "E", "s16"),
                        ("27", "id", "s17"),
                        ("27", "num", "s18"),
                        ("27", "!", "s11"),
                        ("27", "(", "s12"),
                        ("28", "$", "r2"),
                        ("28", ";", "r2"),
                        ("29", "*", "r5"),
                        ("30", "*", "r6"),
                        ("31", "$", "r3"),
                        ("31", ";", "r3"),
                        ("32", "$", "r4"),
                        ("32", ";", "r4"),
                        ("33", "x", "r12"),
                        ("33", "y", "r12"),
                        ("34", "C", "s37"),
                        ("34", "A", "s38"),
                        ("34", "B", "s39"),
                        ("34", "E", "s8"),
                        ("34", "id", "s9"),
                        ("34", "num", "s10"),
                        ("34", "!", "s11"),
                        ("34", "(", "s12"),
                        ("35", ")", "s40"),
                        ("36", "*", "s41"),
                        ("37", ")", "r0"),
                        ("37", ";", "r0"),
                        ("38", "x", "s42"),
                        ("39", "y", "s43"),
                        ("40", ")", "r1"),
                        ("40", ";", "r1"),
                        ("41", ")", "r2"),
                        ("41", ";", "r2"),
                        ("42", ")", "r3"),
                        ("42", ";", "r3"),
                        ("43", ")", "r4"),
                        ("43", ";", "r4")
                    )
                },
                new object[]
                {
                    @"S -> a a A | a b B
                        A -> C a | D b
                        B -> C b | D a
                        C -> E
                        D -> E
                        E -> ε",
                    new LR1GenericParsingTable<LR1KernelItem>
                    (
                        ("0", "S", "s1"),
                        ("0", "a", "s2"),
                        ("1", "$", "a"),
                        ("2", "a", "s3"),
                        ("2", "b", "s4"),
                        ("3", "A", "s5"),
                        ("3", "C", "s6"),
                        ("3", "D", "s7"),
                        ("3", "E", "s8"),
                        ("3", "a", "r8"),
                        ("3", "b", "r8"),
                        ("4", "B", "s9"),
                        ("4", "C", "s10"),
                        ("4", "D", "s11"),
                        ("4", "E", "s12"),
                        ("4", "b", "r8"),
                        ("4", "a", "r8"),
                        ("5", "$", "r0"),
                        ("6", "a", "s13"),
                        ("7", "b", "s14"),
                        ("8", "a", "r6"),
                        ("8", "b", "r7"),
                        ("9", "$", "r1"),
                        ("10", "b", "s15"),
                        ("11", "a", "s16"),
                        ("12", "b", "r6"),
                        ("12", "a", "r7"),
                        ("13", "$", "r2"),
                        ("14", "$", "r3"),
                        ("15", "$", "r4"),
                        ("16", "$", "r5")
                    )
                },
                new object[]
                {
                    @"Value -> number V | number
                        V -> f Real | i Int
                        Real -> IOpt dot | BOpt +
                        Int -> IOpt + | BOpt dot
                        BOpt -> Opt
                        IOpt -> Opt
                        Opt -> ε",
                    new LR1GenericParsingTable<LR1KernelItem>
                    (
                        ("0", "Value", "s1"),
                        ("0", "number", "s2"),
                        ("1", "$", "a"),
                        ("2", "V", "s3"),
                        ("2", "f", "s4"),
                        ("2", "i", "s5"),
                        ("2", "$", "r1"),
                        ("3", "$", "r0"),
                        ("4", "Real", "s6"),
                        ("4", "IOpt", "s7"),
                        ("4", "BOpt", "s8"),
                        ("4", "Opt", "s9"),
                        ("4", "dot", "r10"),
                        ("4", "+", "r10"),
                        ("5", "Int", "s10"),
                        ("5", "IOpt", "s11"),
                        ("5", "BOpt", "s12"),
                        ("5", "Opt", "s13"),
                        ("5", "+", "r10"),
                        ("5", "dot", "r10"),
                        ("6", "$", "r2"),
                        ("7", "dot", "s14"),
                        ("8", "+", "s15"),
                        ("9", "dot", "r9"),
                        ("9", "+", "r8"),
                        ("10", "$", "r3"),
                        ("11", "+", "s16"),
                        ("12", "dot", "s17"),
                        ("13", "+", "r9"),
                        ("13", "dot", "r8"),
                        ("14", "$", "r4"),
                        ("15", "$", "r5"),
                        ("16", "$", "r6"),
                        ("17", "$", "r7")
                    )
                },
                //TODO: rewrite parsing table to ignore order (follow each item)
//                new object[]
//                {
//                    @"S -> ' Q | P
//                        Q -> T W | E ;
//                        P -> T ; | E W
//                        T -> U
//                        E -> U
//                        U -> '
//                        W -> * W | 8 W | ε",
//                    new LR1GenericParsingTable<LR1KernelItem>
//                    (
//                        @"[{""S"":{""shift"":1},""'"":{""shift"":2},""P"":{""shift"":3},""T"":{""shift"":4},""E"":{""shift"":5},""U"":{""shift"":6}},{""Grammar.END"":{""reduce"":[-1]}},{""8"":{""reduce"":[8]},""Q"":{""shift"":7},""T"":{""shift"":8},""E"":{""shift"":9},""U"":{""shift"":10},""'"":{""shift"":11},"";"":{""reduce"":[8]},""*"":{""reduce"":[8]},""Grammar.END"":{""reduce"":[8]}},{""Grammar.END"":{""reduce"":[1]}},{"";"":{""shift"":12}},{""8"":{""shift"":13},""W"":{""shift"":14},""*"":{""shift"":15},""Grammar.END"":{""reduce"":[11]}},{""8"":{""reduce"":[7]},"";"":{""reduce"":[6]},""*"":{""reduce"":[7]},""Grammar.END"":{""reduce"":[7]}},{""Grammar.END"":{""reduce"":[0]}},{""8"":{""shift"":13},""W"":{""shift"":16},""*"":{""shift"":15},""Grammar.END"":{""reduce"":[11]}},{"";"":{""shift"":17}},{""8"":{""reduce"":[6]},""*"":{""reduce"":[6]},""Grammar.END"":{""reduce"":[6]},"";"":{""reduce"":[7]}},{""8"":{""reduce"":[8]},""*"":{""reduce"":[8]},""Grammar.END"":{""reduce"":[8]},"";"":{""reduce"":[8]}},{""Grammar.END"":{""reduce"":[4]}},{""8"":{""shift"":13},""W"":{""shift"":18},""*"":{""shift"":15},""Grammar.END"":{""reduce"":[11]}},{""Grammar.END"":{""reduce"":[5]}},{""8"":{""shift"":13},""W"":{""shift"":19},""*"":{""shift"":15},""Grammar.END"":{""reduce"":[11]}},{""Grammar.END"":{""reduce"":[2]}},{""Grammar.END"":{""reduce"":[3]}},{""Grammar.END"":{""reduce"":[10]}},{""Grammar.END"":{""reduce"":[9]}}]"
//                    )
//                }
            };

        public LR1Tests(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        { }

#pragma warning disable CA1707
        [Theory]
        [MemberData(nameof(LR1Tests.LR1Grammars))]
        public void LR1_Valid_Grammar
            (string grammar, LR1GenericParsingTable<LR1KernelItem> verification)
        {
            CNFGrammar cnfGrammar = this.GenerateGrammar(grammar);

            IParsing<LR1Action> lr1Parsing
                = BaseParsingTests.CreateLR1Parsing(cnfGrammar);
            lr1Parsing.Classify();

            IParsingTable<LR1Action> lr1ParsingTable
                = lr1Parsing.CreateParsingTable();
            Assert.NotNull(lr1ParsingTable);

            this.TestOutputHelper.WriteLine("Verifying parsing table");
            verification.Verify(cnfGrammar, lr1ParsingTable);

            //this.TestOutputHelper.WriteLine(verification.ToString());
        }
#pragma warning restore CA1707
    }
}
