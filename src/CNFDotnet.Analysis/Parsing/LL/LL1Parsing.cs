using System.Collections.Generic;
using System.Linq;

using CNFDotnet.Analysis.Grammar;

namespace CNFDotnet.Analysis.Parsing.LL
{
    /* (strong) LL(1) (Left-to-right, producing a Left-most derivation) is a
     * form of deterministic top-down parsing with a single (1) look-ahead
     * token. Top-down meaning start parsing from the start token (a
     * non-terminal).
     *
     * Consider as a LL(1) grammar
     *
     * S -> a A | b B
     * A -> C a | D b
     * B -> C b | D a
     * C -> E
     * D -> E
     * E -> ε
     *
     * S being the start symbol, space as token seperator, $ representing
     * the end (EOF) and ε (epsilon) representing an empty (nullable) token.
     * Terminals are represented by non-capital letters and non-terminals by
     * capital letters.
     * This produces the following parsing table using FIRST and FOLLOW sets
     * Number the productions for easier resolving and remove the spaces so
     * it can fit.
     *
     *     |     a    |    b     | $
     *   S | S₁ -> aA | S₂ -> bB |
     *   A | A₁ -> Ca | A₂ -> Db |
     *   B | B₁ -> Da | B₂ -> Cb |
     *   C | C₁ -> E  | C₂ -> E  |
     *   D | D₁ -> E  | D₂ -> E  |
     *   E | E₁ -> ε  | E₂ -> ε  |
     *
     * Each cell has at most 1 item represented by an LL1Action containing a
     * terminal, a non-terminal and a production.
     *
     * Using the input sentence
     *
     * ab
     *
     * One can resolve the parsing with the parsing table using
     *
     * matched input | rest of input
     *      analysis | prediction
     *
     *               | ab$
     *               | S$
     *
     *               | ab$
     *            S₁ | aA$ 
     *
     *             a | b$
     *           S₁a | A$
     *
     *             a | b$
     *         S₁aA₂ | Db$
     *
     *             a | b$
     *       S₁aA₂D₂ | Eb$
     *
     *             a | b$
     *     S₁aA₂D₂E₂ | b$
     *
     *            ab | $
     *    S₁aA₂D₂E₂b | $
     *
     *           ab$ | 
     *   S₁aA₂D₂E₂b$ | 
     *
     * the input sentence ab resolves to S₁A₂D₂E₂
     * S -> aA -> aDb -> aEb -> ab -> $
     *
     * The parse tree for the input sentence ab. The tree should be viewed from
     * top to bottom, from left to right.
     *
     *                   S
     *              /        \
     *             |         A
     *             |      /     \
     *             |     D       |
     *             |     |       |
     *             |     E       |
     *             |     |       |
     *             a     ε       b
     */
    public class LL1Parsing : BaseParsing<LL1Action>
    {
        public LL1Parsing(CNFGrammar cnfGrammar)
            : base(cnfGrammar)
        { }

        public override void Classify()
        {
            IEnumerable<Production> nullAmbiguity;

            //A grammar containing a null ambiguity is not valid LL(1)
            if((nullAmbiguity = this.CNFGrammar.ComputeNullAmbiguity()).Any())
            {
                throw new LL1ClassificationException
                (
                    "Grammar contains a null ambiguity on"
                        + string.Join('\n', nullAmbiguity)
                );
            }

            /* Check for first set clashes, check if every non-terminal head has
             * at most 1 of a token in its first set, this step also eliminates 
             * left-recursion.
             * If the grammar contains left-recursion consider left-factoring */
            Dictionary<Token, HashSet<Token>> table
                = new Dictionary<Token, HashSet<Token>>();
            IReadOnlyList<Production> productions = this.CNFGrammar.Productions;
            IReadOnlySet<Token> nonTerminals
                = this.CNFGrammar.ComputeNonTerminals();

            foreach(Token nonTerminal in nonTerminals)
            {
                table.Add(nonTerminal, new HashSet<Token>());
            }

            Token head;
            IReadOnlyList<Token> body;

            foreach(Production production in productions)
            {
                head = production.Head;
                body = production.Body;

                foreach(Token s in this.CNFGrammar.GetFirst(body))
                {
                    if(table[head].Contains(s))
                    {
                        throw new LL1ClassificationException
                        (
                            $"Grammar contains a first set clash on"
                                + $" non-terminal {head.Value} ({s.Value})"
                        );
                    }

                    table[head].Add(s);
                }
            }

            /* Check for first/follow set clashes. That is, check that every
             * nullable production has disjoint first and follow sets. */
            IDictionary<Token, HashSet<Token>> firstSet
                = this.CNFGrammar.ComputeFirstSet();
            IDictionary<Token, HashSet<Token>> followSet
                = this.CNFGrammar.ComputeFollowSet();
            IReadOnlySet<Token> nullable = this.CNFGrammar.ComputeNullable();

            foreach(Token k in nullable)
            {
                if(!firstSet.ContainsKey(k))
                {
                    continue;
                }

                foreach(Token f in firstSet[k])
                {
                    if(followSet[k].Contains(f))
                    {
                        throw new LL1ClassificationException
                        (
                            $"Grammar contains a first/follow clash on"
                                + $" {f.Value}"
                        );
                    }
                }
            }
        }

        /* The parsing table consists of all terminals as columns and all
         * non-terminals as rows, with a productions as a prediction item of the
         * combination of a non-terminal and a terminal from the input sentence.
         */
        public override ParsingTable<LL1Action> CreateParsingTable()
        {
            if(this.ParsingTable is not null)
            {
                return this.ParsingTable;
            }

            ParsingTable<LL1Action> table = new ParsingTable<LL1Action>();
            IDictionary<Token, HashSet<Token>> followSet
                = this.CNFGrammar.ComputeFollowSet();
            IReadOnlyList<Production> productions = this.CNFGrammar.Productions;

            Token head;
            IReadOnlyList<Token> body;

            foreach(Production production in productions)
            {
                head = production.Head;
                body = production.Body;

                //Foreach token in the first set of every token in the body of
                //production add the production as an action for the head
                //terminal and the FIRST token(s)
                foreach(Token s in this.CNFGrammar.GetFirst(body))
                {
                    table.Add(new LL1Action(head, s, production));
                }

                //If the body is nullable, do the same for the followset (if
                //any)
                if(this.CNFGrammar.IsNullable(body))
                {
                    if(!followSet.ContainsKey(head))
                    {
                        continue;
                    }

                    foreach(Token s in followSet[head])
                    {
                        table.Add(new LL1Action(head, s, production));
                    }
                }
            }

            return this.ParsingTable = table;
        }
    }
}