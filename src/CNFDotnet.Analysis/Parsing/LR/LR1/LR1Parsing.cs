using System.Collections.Generic;

using CNFDotnet.Analysis.Grammar;

namespace CNFDotnet.Analysis.Parsing.LR.LR1
{
    /* LR(1) (Left-to-right, Right-most derivation) is a form of deterministic
     * bottom-up parsing with a look-ahead of 1 (1) . Bottom-up meaning start
     * parsing from the first token of the input sentence (a terminal).
     *
     * Consider as an LR(1) grammar. I removed all choices and numbered every
     * production for easy referenceing.
     *
     * S₁ -> a a A 
     * S₂ -> a b B
     * A₁ -> C a
     * A₂ -> D b
     * B₁ -> C b
     * B₂ -> D a
     * C₁ -> E
     * D₁ -> E
     * E₁ -> ε
     *
     * S being the start symbol, space as token seperator, $ representing
     * the end (EOF) and ε (epsilon) representing an empty (nullable) token.
     * Terminals are represented by non-capital letters and non-terminals by
     * capital letters.
     *
     * We first construct the LR(1) automaton with • indicating the position,
     * digits on top indicating the state number and lines with a token
     * depicting transitions. I will first list all states, and then all
     * transitions, else it will be a mess. Ususally this is a single diagram.
     * There are no annotions, because there can be multiple actions per state.
     *
     *         0              1             2                       3
     * |---------------| |--------| |---------------|       |---------------|
     * | S -> •aaA [$] | | S• [$] | | S -> a•aA [$] |       | S -> aa•A [$] |
     * | S -> •abB [$] | +--------+ | S -> a•bB |$] |       | A -> •Ca [$]  |
     * | •S [$]        |            +---------------+       | A -> •Db [$]  |
     * +---------------+                                    | C -> •E [a]   |
     *        4                 5                  6        | D -> •E [b]   |
     * |---------------| |---------------| |--------------| | E -> • [a]    |
     * | S -> ab•B [$] | | S -> aaA• [$] | | A -> C•a [$] | | E -> • [b]    |
     * | B -> •Cb [$]  | +---------------+ +--------------+ +---------------+
     * | B -> •Da [$]  |        7                 8                9
     * | C -> •E [b]   | |--------------| |-------------| |---------------|
     * | D -> •E [a]   | | A -> D•b [$] | | C -> E• [a] | | S -> abB• [$] |
     * | E -> • [b]    | +--------------+ | D -> E• [b] | +---------------+
     * | E -> • [a]    |                  +-------------+
     * +---------------+
     *       10                 11              12             13
     * |--------------| |--------------| |-------------| |--------------|
     * | B -> C•b [$] | | B -> D•a [$] | | C -> E• [b] | | A -> Ca• [$] |
     * +--------------+ +--------------+ | D -> E• [a] | +--------------+
     *                                   +-------------+
     *      14                 15               16
     * |--------------| |--------------| |--------------|
     * | A -> Db• [$] | | B -> Cb• [$] | | B -> Da• [$] |
     * +--------------+ +--------------+ +--------------+
     *
     * 0 -S-> 1      3 -A-> 5      4 -B-> 9      6 -a-> 13
     * 0 -a-> 2      3 -C-> 6      4 -C-> 10     7 -b-> 14
     * 2 -a-> 3      3 -D-> 7      4 -D-> 11    10 -b-> 15
     * 2 -b-> 4      3 -E-> 8      4 -E-> 12    11 -a-> 16
     * 
     * We'll start at state 0. S is the start token. We start at index 0. S can
     * be resolved into 2 productions. These all start with a terminal (after
     * index 0, so nothing more can be derived. By default the start token can
     * be followed by EOF, so we use this as the look-ahead for the first and
     * all its derived items. Let's fast forward to state 3. One can transition
     * to state 3 from state 2 by iterating over the terminal a. We can derive
     * several items from the current position (before non-terminal A) in
     * production S₁. A can be resolved into 2 items (with the same look-ahead).
     * From there C and D can be resolved to E. Here we notice that a token (a &
     * b) follow. We use these a look-ahead for the newly derived items.
     *
     * From this automaton (states and transitions) a parsing table can be
     * constructed. The table represents a series on actions dependant on the
     * current token (a terminal) in the input sentence. There are 3 types of
     * actions: a shift where the input sentence gets shifted and a transition
     * to a new state occurs, a goto where a transition to a new state occurse
     * and a reduction where one or more input tokens get reduced to a single
     * token (usually followed by a goto). One extra state is the accept state,
     * where the input gets accepted (denoted by 'a'). Shift are denoted by 's'
     * followed by the number of the new state. Reductions get denoted by a 'r'
     * followed by the identifier of a production. A goto gets denoted by a 'g'
     * followed by the number of the new state. Every empty cell is considered a
     * parsing error.
     *
     * State |  a     b     $  |  S     A     B     C     D     E
     * ------+-----------------+-----------------------------------
     *     0 | s2              | g1
     *     1 |              a  |
     *     2 | s3    s4        |
     *     3 | rE₁   rE₁       |       g5          g6    g7    g8
     *     4 | rE₁   rE₁       |             g9    g10   g11   g12
     *     5 |             rS₁ |
     *     6 | s13             |
     *     7 |       s14       |
     *     8 | rC₁   rD₁       |
     *     9 |             rS₂ |
     *    10 |       s15       |
     *    11 | s16             |
     *    12 | rD₁   rC₁       |
     *    13 |             rA₁ |
     *    14 |             rA₂ |
     *    15 |             rB₁ |
     *    16 |             rB₂ |
     *
     * One can  reslove the parsing using a single stack containing recognised
     * input and the states which have been used.
     *
     *                   stack | rest of input   | action
     *
     * The stack can only contain states and terminals. Parsing always starts in
     * state 0. A reduction removes as many tokens/states from the stack as the
     * length of the production (reduce the stack by n*2 items) and replaces
     * it with the head of that production (a non-terminal).
     *
     * Using the input sentence
     *
     * abb
     *
     *                   0 | abb$ | s2 (state 0, at a, shift and goto state 2)
     *               0 a 2 | bb$  | s4 (state 2, at b, shift and goto state 4)
     *           0 a 2 b 4 | b$   | rE₁ (state 4, at b, reduce by 0 to E)
     *           0 a 2 b 4 | Eb$  | g12 (state 4, at E, goto 12)
     *      0 a 2 b 4 E 12 | b$   | rC₁ (state 12, at b reduce by 2 to C)
     *           0 a 2 b 4 | Cb$  | g10 (state 4, at C, goto 10)
     *      0 a 2 b 4 C 10 | b$   | s15 (state 10, at b, shift and goto 15)
     * 0 a 2 b 4 C 10 b 15 | $    | rB₁ (state 15, at $, reduce by 4 to B)
     *           0 a 2 b 4 | B$   | g9 (state 4, at B, goto 9)
     *       0 a 2 b 4 B 9 | $    | rS₂ (state 9, at $, reduce by 6 to S)
     *                   0 | S$   | g1 (state 0, at S, goto 1)
     *               0 S 1 | $    | a (state 1, at $, accept input)
     *
     * I added the non-terminals to the input (after reduction) for easier
     * understanding. The input should NEVER be mutated.
     *
     * The parsing can be represented with the this parse tree. The tree
     * should be viewed from bottom to top, from right to left.
     *
     *                             S
     *          /                  |                 \                    ^
     *         |                   |                  B                   |
     *         |                   |         /                \           |
     *         |                   |        C                  |          |
     *         |                   |        |                  |          |
     *         |                   |        E                  |          |
     *         |                   |        |                  |          |
     *         a                   b        ε                  b          |
     *
     */
    public class LR1Parsing : BaseLR1Parsing<LR1Action, LR1KernelItem>
    {
        public LR1Parsing(CNFGrammar cnfGrammar)
            : base(cnfGrammar)
        { }

        protected override LR1KernelItem CreateKernelItem
            (Production production, int index, IEnumerable<Token> lookAheads)
            => new LR1KernelItem(production, index, lookAheads);

        public override IParsingTable<LR1Action> CreateParsingTable()
        {
            if(this.ParsingTable is not null)
            {
                return this.ParsingTable;
            }

            //First create the automaton with look-aheads
            IAutomaton<LR1KernelItem> automaton = this.CreateAutomaton();
            ParsingTable<LR1Action> table = new ParsingTable<LR1Action>();
            LR1Action actions;
            Token end = new Token(TokenType.EOF);

            //Each state in the automaton can be seen as a row in the parsing
            //table
            foreach(State<LR1KernelItem> state in automaton)
            {
                //Initalize actions (a row in the parsing table)
                actions = new LR1Action();

                //Each item in the transitions is a shift iterating over a token
                //to a new state
                foreach(KeyValuePair<Token, State<LR1KernelItem>> kv
                        in state.Transitions)
                {
                    actions.Add
                    (
                        kv.Key, //The token
                        new LR1ActionItem<LR1KernelItem>(kv.Value) //the state
                    );
                }

                /* A reduction occurs when the end of a production has been
                 * reached */
                foreach(LR1KernelItem item in state.Items)
                {
                    /* The null production (start token), has a length of 0. If
                     * the end of this "production" has been reached at the end
                     * of the input ($) the input can be accepted */
                    if(item.Production.Equals(Production.Null))
                    {
                        if(item.Index == 1)
                        {
                            this.AddReduceAction(actions, item.Production, end);
                        }
                    }
                    else
                    {
                        /* If the end of a production body has been reached, a
                        * reduction to the head (a non-terminal) occurs */
                        if(item.Index == item.Production.Body.Count)
                        {
                            this.AddReduceAction
                            (
                                actions,
                                item.Production,
                                item.LookAheads
                            );
                        }
                    }
                }

                table.Add(actions);
            }

            this.ParsingTable = table;

            return table;
        }
    }
}
