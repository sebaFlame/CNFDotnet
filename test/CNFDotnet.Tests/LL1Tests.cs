using System.Collections.Generic;

using Xunit;
using Xunit.Abstractions;

using CNFDotnet.Analysis.Grammar;
using CNFDotnet.Analysis.Parsing;
using CNFDotnet.Analysis.Parsing.LL;

namespace CNFDotnet.Tests
{
    public class LL1Tests : BaseParsingTests
    {
        public static IEnumerable<object[]> LL1Grammars =>
            new object[][]
            {
               new object[]
               {
                   @"A -> B | x C | y A
                       B -> C B
                       C -> r",
                    new LL1GenericParsingTable
                    (
                        ("A", "x", "1"),
                        ("A", "y", "2"),
                        ("A", "r", "0"),
                        ("B", "r", "3"),
                        ("C", "r", "4")
                    )
               },
               new object[]
                {
                    @"A -> y B | x | B C
                        B -> z B | u
                        C -> s",
                    new LL1GenericParsingTable
                    (
                        ("A", "y", "0"),
                        ("A", "x", "1"),
                        ("A", "z", "2"),
                        ("A", "u", "2"),
                        ("B", "z", "3"),
                        ("B", "u", "4"),
                        ("C", "s", "5")
                    )
                },
                new object[]
                {
                    @"S -> ( Ses ) | (* *)
                        Ses -> S SL
                        SL -> ; SL | S",
                    new LL1GenericParsingTable
                    (
                        ("S", "(", "0"),
                        ("S", "(*", "1"),
                        ("Ses", "(", "2"),
                        ("Ses", "(*", "2"),
                        ("SL", "(", "4"),
                        ("SL", "(*", "4"),
                        ("SL", ";", "3")
                    )
                },
                new object[]
                {
                    @"A -> B c | d n A B fo
                        B -> r | ε",
                    new LL1GenericParsingTable
                    (
                        ("A", "c", "0"),
                        ("A", "d", "1"),
                        ("A", "r", "0"),
                        ("B", "c", "3"),
                        ("B", "fo", "3"),
                        ("B", "r", "2")
                    )
                },
                new object[]
                {
                    @"S -> for ( ExprOpt ; ExprOpt ; ExprOpt ) S | expr ;
                    ExprOpt -> expr | ε",
                    new LL1GenericParsingTable
                    (
                        ("S", "for", "0"),
                        ("S", "expr", "1"),
                        ("ExprOpt", ";", "3"),
                        ("ExprOpt", ")", "3"),
                        ("ExprOpt", "expr", "2")
                    )
                },
                new object[]
                {
                    @"Decl -> DeclSpecifiers Declarator
                        DeclSpecifiers -> StorageClassSpecifier DeclSpecifiersOpt | TypeSpecifier DeclSpecifiersOpt | TypeQualifier DeclSpecifiersOpt
                        DeclSpecifiersOpt -> DeclSpecifiers | ε
                        StorageClassSpecifier -> typedef | static
                        TypeSpecifier -> void | short | int
                        TypeQualifier -> const | volatile
                        Declarator -> PointerOpt DirectDeclarator
                        DirectDeclarator -> id
                        PointerOpt -> * TypeQualifierList PointerOpt | ε
                        TypeQualifierList -> TypeQualifier TypeQualifierList | ε",
                    new LL1GenericParsingTable
                    (
                        ("Decl", "typedef", "0"),
                        ("Decl", "static", "0"),
                        ("Decl", "void", "0"),
                        ("Decl", "short", "0"),
                        ("Decl", "int", "0"),
                        ("Decl", "const", "0"),
                        ("Decl", "volatile", "0"),
                        ("DeclSpecifiers", "typedef", "1"),
                        ("DeclSpecifiers", "static", "1"),
                        ("DeclSpecifiers", "void", "2"),
                        ("DeclSpecifiers", "short", "2"),
                        ("DeclSpecifiers", "int", "2"),
                        ("DeclSpecifiers", "const", "3"),
                        ("DeclSpecifiers", "volatile", "3"),
                        ("DeclSpecifiersOpt", "typedef", "4"),
                        ("DeclSpecifiersOpt", "static", "4"),
                        ("DeclSpecifiersOpt", "void", "4"),
                        ("DeclSpecifiersOpt", "short", "4"),
                        ("DeclSpecifiersOpt", "int", "4"),
                        ("DeclSpecifiersOpt", "const", "4"),
                        ("DeclSpecifiersOpt", "volatile", "4"),
                        ("DeclSpecifiersOpt", "id", "5"),
                        ("DeclSpecifiersOpt", "*", "5"),
                        ("StorageClassSpecifier", "typedef", "6"),
                        ("StorageClassSpecifier", "static", "7"),
                        ("TypeSpecifier", "void", "8"),
                        ("TypeSpecifier", "short", "9"),
                        ("TypeSpecifier", "int", "10"),
                        ("TypeQualifier", "const", "11"),
                        ("TypeQualifier", "volatile", "12"),
                        ("Declarator", "id", "13"),
                        ("Declarator", "*", "13"),
                        ("DirectDeclarator", "id", "14"),
                        ("PointerOpt", "id", "16"),
                        ("PointerOpt", "*", "15"),
                        ("TypeQualifierList", "const", "17"),
                        ("TypeQualifierList", "volatile", "17"),
                        ("TypeQualifierList", "id", "18"),
                        ("TypeQualifierList", "*", "18")
                    )
                },
                new object[]
                {
                    @"S -> id S'
                        S' -> V assign E | ε
                        V -> ε
                        E -> id V | num",
                    new LL1GenericParsingTable
                    (
                        ("S", "id", "0"),
                        ("S'", "assign", "1"),
                        ("S'", "$", "2"),
                        ("V", "assign", "3"),
                        ("V", "$", "3"),
                        ("E", "id", "4"),
                        ("E", "num", "5")
                    )
                },
                new object[]
                {
                    @"L -> V + L | num
                        V -> Var ( Var + V ) | ε
                        Var -> ε",
                    new LL1GenericParsingTable
                    (
                        ("L", "+", "0"),
                        ("L", "num", "1"),
                        ("L", "(", "0"),
                        ("V", "+", "3"),
                        ("V", "(", "2"),
                        ("V", ")", "3"),
                        ("Var", "+", "4"),
                        ("Var", "(", "4")
                    )
                },
                new object[]
                {
                    @"P -> M * | ε
                        M -> Q StarM | ε
                        StarM -> (* M *) | ( Q * )
                        Q -> o | ε",
                    new LL1GenericParsingTable
                    (
                        ("P", "*", "0"),
                        ("P", "(*", "0"),
                        ("P", "(", "0"),
                        ("P", "o", "0"),
                        ("P", "$", "1"),
                        ("M", "*", "3"),
                        ("M", "(*", "2"),
                        ("M", "*)", "3"),
                        ("M", "(", "2"),
                        ("M", "o", "2"),
                        ("StarM", "(*", "4"),
                        ("StarM", "(", "5"),
                        ("Q", "*", "7"),
                        ("Q", "(*", "7"),
                        ("Q", "(", "7"),
                        ("Q", "o", "6")
                    )
                },
                new object[]
                {
                    @"S -> a A | b B
                        A -> C a | D b
                        B -> C b | D a
                        C -> E
                        D -> E
                        E -> ε",
                    new LL1GenericParsingTable
                    (
                        ("S", "a", "0"),
                        ("S", "b", "1"),
                        ("A", "a", "2"),
                        ("A", "b", "3"),
                        ("B", "a", "5"),
                        ("B", "b", "4"),
                        ("C", "a", "6"),
                        ("C", "b", "6"),
                        ("D", "a", "7"),
                        ("D", "b", "7"),
                        ("E", "a", "8"),
                        ("E", "b", "8")
                    )
                },
                new object[]
                {
                    @"S -> ( X | E sq) | F )
                        X -> E ) | F sq)
                        E -> A
                        F -> A
                        A -> ε",
                    new LL1GenericParsingTable
                    (
                        ("S", "(", "0"),
                        ("S", "sq)", "1"),
                        ("S", ")", "2"),
                        ("X", "sq)", "4"),
                        ("X", ")", "3"),
                        ("E", "sq)", "5"),
                        ("E", ")", "5"),
                        ("F", "sq)", "6"),
                        ("F", ")", "6"),
                        ("A", "sq)", "7"),
                        ("A", ")", "7")
                    )
                },
                new object[]
                {
                    @"E -> id + D | ( E * R ) | ε
                        D -> V * E | L ! E
                        R -> V ! E | L * E
                        V -> Z | num
                        L -> Z | ( E )
                        Z -> ε",
                    new LL1GenericParsingTable
                    (
                        ("E", "id", "0"),
                        ("E", "(", "1"),
                        ("E", "*", "2"),
                        ("E", ")", "2"),
                        ("E", "$", "2"),
                        ("D", "(", "4"),
                        ("D", "*", "3"),
                        ("D", "!", "4"),
                        ("D", "num", "3"),
                        ("R", "(", "6"),
                        ("R", "*", "6"),
                        ("R", "!", "5"),
                        ("R", "num", "5"),
                        ("V", "*", "7"),
                        ("V", "!", "7"),
                        ("V", "num", "8"),
                        ("L", "(", "10"),
                        ("L", "*", "9"),
                        ("L", "!", "9"),
                        ("Z", "*", "11"),
                        ("Z", "!", "11")
                    )
                },
           };

        public LL1Tests(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        { }

#pragma warning disable CA1707
        [Theory]
        [MemberData(nameof(LL1Tests.LL1Grammars))]
        public void LL1_Valid_Grammar
            (string grammar, LL1GenericParsingTable verification)
        {
            CNFGrammar cnfGrammar = this.GenerateGrammar(grammar);

            IParsing<LL1Action> ll1Parsing
                = BaseParsingTests.CreateLL1Parsing(cnfGrammar);

            ll1Parsing.Classify();

            IParsingTable<LL1Action> parsingTable =
                ll1Parsing.CreateParsingTable();
            Assert.NotNull(parsingTable);

            this.TestOutputHelper.WriteLine("Verifying parsing table");
            verification.Verify(cnfGrammar, parsingTable);
        }
#pragma warning restore CA1707
    }
}
