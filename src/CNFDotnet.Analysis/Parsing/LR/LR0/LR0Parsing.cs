using System.Collections.Generic;

using CNFDotnet.Analysis.Grammar;

namespace CNFDotnet.Analysis.Parsing.LR.LR0
{
    /* LR(0) (Left-to-right, Right-most derivation) is a form of deterministic
     * bottom-up parsing with a look-ahead of 0 (0) . Bottom-up meaning start
     * parsing from the first token of the input sentence (a terminal).
     *
     * Consider as an LR(0) grammar. I removed all choices and numbered every
     * production for easy referenceing.
     *
     * A₁ -> B
     * A₂ -> x C
     * A₃ -> y A
     * B₁ -> C B
     * C₁ -> r
     *
     * A being the start symbol, space as token seperator, $ representing
     * the end (EOF) and ε (epsilon) representing an empty (nullable) token.
     * Terminals are represented by non-capital letters and non-terminals by
     * capital letters.
     *
     * We first construct the LR(0) automaton with • indicating the position,
     * digits on top indicating the state number and lines with a token
     * depicting transitions. I will first list all states, and then all
     * transitions, else it will be a mess. Ususally this is a single diagram.
     * The annotations are explained after the current figure.
     *
     *       0        1 a     2 rA₁          3           4             5
     * |----------| |----| |---------| |----------| |----------| |----------|
     * | •A       | | A• | | A -> B• | | A -> x•C | | A -> y•A | | B -> C•B |
     * | A -> •B  | +----+ +---------+ | C -> •r  | | A -> •B  | | B -> •CB |
     * | A -> •xC |                    +----------+ | A -> •xC | | C -> •r  |
     * | A -> •yA |                                 | A -> •yA | +----------+
     * | B -> •CB |                                 | B -> •CB |
     * | C -> •r  |                                 | C -> •r  |
     * +----------+                                 +----------+ 
     *    6 rC₁       7 rA₂         8 rA₃        9 rB₁
     * |---------| |----------| |----------| |----------|
     * | C -> r• | | A -> xC• | | A -> yA• | | B -> CB• |
     * +---------+ +----------+ +----------+ +----------+
     *
     * 0 -A-> 1 g1     5 -C-> 5 g5     4 -x-> 3 s3
     * 0 -r-> 6 s6     5 -r-> 6 s6     4 -y-> 4 s4
     * 0 -C-> 5 g5     5 -B-> 9 g9     4 -r-> 6 s6
     * 0 -B-> 2 g2     4 -C-> 5 g5     3 -r-> 6 s6
     * 0 -y-> 4 s4     4 -B-> 2 g2     3 -C-> 7 g7
     * 0 -x-> 3 s3     4 -A-> 8 g8
     * 
     * We'll start with state 0. A is the start token. We start at index 0
     * (before the start token). A (a non-terminal) can be resolved into 3
     * items, which then also start at index 0. B can also be resolved, once
     * again at index 0. C can then again be resolved. No more items can be
     * resolved and our first state is complete. Now we move the index forward
     * by 1 and derive new states from all items in state 0. States 1, 2, 3, 4,
     * 4, 6 are directly derived from state 0. If the index is at the end of the
     * production, no more states can be derived. Each transition gets denoted
     * by the token the index moves over.
     *
     * From this automaton (states and transitions) a parsing table can be
     * constructed. When the position is at the end of a production a reduction
     * is called, else a goto is called. A goto can be preceded by a shift (of
     * the input) if the current input token is a terminal. A shift gets denoted
     * by an 's' and a number of the new state. A reduction gets denoted by an
     * 'r' and an identifier for a production. A goto (without shift) is denoted
     * by a 'g' and a number of the new state. An 'a' means the input has been
     * accepted (a reduction to the start token). A reduction in LR(0) is not
     * dependent on the current input. Every annotated item in the automaton can
     * be found in the parsing table. Every empty cell is considered a parsing
     * error.
     *
     * State |  x     y     r  |  A     B     C  
     * ------+-----------------+-----------------
     *    0  | s3    s4    s6  | g1    g2    g5
     *    1  |  a     a     a  |
     *    2  | rA₁   rA₁   rA₁ |
     *    3  |             s6  |             g7
     *    4  | s3    s4    s6  | g8    g2    g5
     *    5  |             s6  |       g9    g5
     *    6  | rC₁   rC₁   rC₁ |
     *    7  | rA₂   rA₂   rA₂ |
     *    8  | rA₃   rA₃   rA₃ |
     *    9  | rB₁   rB₁   rB₁ |
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
     * yxr
     * 
     *               0 | yxr     | s4  (state 0 at y, shift and goto state 4)
     *           0 y 4 | xr      | s3  (state 4 at x, shift and goto state 3)
     *       0 y 4 x 3 | r       | s6  (state 3 at r, shift and goto state 6)
     *   0 y 4 x 3 r 6 |         | rC₁ (state 6, reduce by 2 to C)
     *       0 y 4 x 3 | C       | g7  (state 3 at C, goto state 7)
     *   0 y 4 x 3 C 7 |         | rA₂ (state 7, reduce by 4 to A)
     *           0 y 4 | A       | g8  (state 4 at A, goto state 8)
     *       0 y 4 A 8 |         | rA₃ (state 8, reduce by 4 to A)
     *               0 | A       | g1  (state 0 at A, goto state 1)
     *           0 A 1 |         | a   (state 1, accept input)
     *
     * I added the non-terminals to the input (after reduction) for easier
     * understanding. The input should NEVER be mutated.
     *
     * The parsing can be represented with this parse tree. The tree
     * should be viewed from bottom to top, from right to left.
     *
     *                         A                          ^
     *                  /            \                    |
     *                 |             A                    |
     *                 |        /           \             |
     *                 |       |             C            |
     *                 |       |             |            |
     *                 y       x             r            |
     */
    public class LR0Parsing : BaseLR0Parsing<LR0Action>
    {
        public LR0Parsing(CNFGrammar grammar)
            : base(grammar)
        { }

        public override void Classify()
        {
            //The parsing table has to be constructed to find any issues
            ParsingTable<LR0Action> table = this.CreateParsingTable();
            IReadOnlySet<Token> terminals = this.CNFGrammar.ComputeTerminals();

            foreach(LR0Action action in table)
            {
                //LR(0) does not allow for multiple reductions
                if(action.Reduce.Count > 1)
                {
                    throw new LR0ClassificationException
                    (
                        "Grammar contains an LR(0) reduce-reduce conflict"
                    );
                }

                //LR(0) does not allow for a reduction AND a shift
                if(action.Reduce.Count > 0)
                {
                    foreach(Token s in action.Shift.Keys)
                    {
                        /* A shift can only happen on a terminal -  skip false
                         * positives -  else it's considered a goto, which is
                         * a valid action */
                        if(terminals.Contains(s))
                        {
                            throw new LR0ClassificationException
                            (
                                "Grammar contains an LR(0) shift-reduce"
                                    + " conflict"
                            );
                        }
                    }
                }
            }
        }

        public override ParsingTable<LR0Action> CreateParsingTable()
        {
            if(this.ParsingTable is not null)
            {
                return this.ParsingTable;
            }

            ParsingTable<LR0Action> table = new ParsingTable<LR0Action>();
            IAutomaton<LR0KernelItem> automaton = this.CreateAutomaton();
            LR0Action action;

            /* Each state in the automaton can be seen as a row in the parsing 
             * table */
            foreach(State<LR0KernelItem> state in automaton)
            {
                /* Initialise a new action (it can be considered a row in the 
                 * parsing table) */
                action = new LR0Action();

                /* Each transition is either a shift (on a terminal) or a goto
                 * (on a non-terminal) */
                foreach(Token s in state.Transitions.Keys)
                {
                    action.Shift.Add(s, state.Transitions[s]);
                }

                /* A reduction occurs when the end of a production has been
                 * reached */
                foreach(LR0KernelItem item in state.Items)
                {
                    /* The null production (start token), has a length of 0. If
                     * the end of this "production" has been reached the input
                     * can be accepted */
                    if(item.Production.Equals(Production.Null))
                    {
                        if(item.Index == 1)
                        {
                            action.Reduce.Add(item.Production);
                        }
                    }
                    /* If the end of a production body has been reached, a
                     * reduction to the head (a non-terminal) occurs */
                    else
                    {
                        if(item.Index == item.Production.Body.Count)
                        {
                            action.Reduce.Add(item.Production);
                        }
                    }
                }

                table.Add(action);
            }

            this.ParsingTable = table;

            return table;
        }
    }
}