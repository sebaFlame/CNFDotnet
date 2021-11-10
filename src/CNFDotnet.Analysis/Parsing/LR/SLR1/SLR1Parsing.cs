using System.Collections.Generic;

using CNFDotnet.Analysis.Grammar;

namespace CNFDotnet.Analysis.Parsing.LR.SLR1
{
    /* SLR(1) (Simple Left-to-right, Right-most derivation) is a form of
     * deterministic bottom-up parsing with a single look-ahead of (0) .
     * Bottom-up meaning start parsing from the first token of the input
     * sentence (a terminal).
     * SLR(1) is derived from the LR(0) automaton with the FOLLOW sets used as
     * look-aheads. A smaller parsing table should be able to get constructed in
     * comparison to LR(0) or even LALR(1).
     *
     * Consider as an SLR(1) grammar. I removed all choices and numbered every
     * production for easy referenceing.
     *
     * A₁ -> a B
     * A₂ -> a A C
     * A₃ -> ε
     * B₁ -> d B
     * B₂ -> e
     * C₁ -> c A
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
     * There are no annotions, because there can be multiple actions per state.
     *
     *       0          1        2             3             4
     * |-----------| |----| |-----------| |----------| |-----------|
     * | •A        | | A• | | A -> a•B  | | A -> aB• | | A -> aA•C |
     * | A -> •aB  | +----+ | A -> a•AC | +----------+ | C -> •cA  |
     * | A -> •aAC |        | B -> •dB  |              +-----------+
     * | A -> •    |        | B -> •e   |      10
     * +-----------+        | A -> •aB  | |----------|
     *                      | A -> •aAC | | C -> cA• |
     *                      | A -> •    | +----------+
     *                      +-----------+
     *      5            6            7             8             9
     * |----------| |---------| |-----------| |-----------| |----------|
     * | B -> d•B | | B -> e• | | A -> aAC• | | C -> c•A  | | B -> dB• |
     * | B -> •dB | +---------+ +-----------+ | A -> •aB  | +----------+
     * | B -> •e  |                           | A -> •aAC |
     * +----------+                           | A -> •    |
     *                                        +-----------+
     *
     * 0 -A-> 1     4 -C-> 7
     * 0 -a-> 2     4 -c-> 8
     * 2 -a-> 2     5 -d-> 5
     * 2 -B-> 3     5 -B-> 9
     * 2 -A-> 4     5 -e-> 6
     * 2 -d-> 5     8 -a-> 2
     * 2 -e-> 6     8 -A-> 10
     *
     * We'll start with state 0. A is the start token. We start at index 0
     * (before the start token). A (a non-terminal) can be resolved into 3
     * items, which then also start at index 0. Notice the empty production.
     * Now we move the index forward by 1 and derive new states from all items
     * in state 0. States 1 and 2 are directly derived from state 0. If the
     * index is at the end of the production, no more states can be derived.
     * Each transition gets denoted by the token the index moves over.
     *
     * From this automaton (states and transitions) a parsing table can be
     * constructed using the FOLLOW set of a token. The table represents a
     * series on actions dependant on the current token (a terminal) in the
     * input sentence. There are 3 types of actions: a shift where the input
     * sentence gets shifted and a transition to a new state occurs, a goto
     * where a transition to a new state occurse and a reduction where one or
     * more input tokens get reduced to a single token (usually followed by a
     * goto). One extra state is the accept state, where the input gets accepted
     * (denoted by 'a'). Shift are denoted by 's' followed by the number of the
     * new state. Reductions get denoted by a 'r' followed by the identifier of
     * a production. A goto gets denoted by a 'g' followed by the number of the
     * new state. Every empty cell is considered a parsing error.
     *
     * State |  a     d     e     c     $  |  A     B     C  
     * ------+-----------------------------+-----------------
     *     0 | s2                rA₃   rA₃ | g1
     *     1 |                          a  |
     *     2 | s2    s5    s6    rA₃   rA₃ | g4    g3
     *     3 |                   rA₁   rA₁ | 
     *     4 |                   s8        |             g7
     *     5 |       s5    s6              |       g9
     *     6 |                   rB₂   rB₂ |
     *     7 |                   rA₂   rA₂ | 
     *     8 | s2                rA₃   rA₃ | g10
     *     9 |                   rB₁   rB₁ | 
     *    10 |                   rC₁   rC₁ |
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
     * ade
     *
     *                 0 | ade$     | s2 (state 0, at a, shift and goto state 2)
     *             0 a 2 | de$      | s5 (state 2, at d, shift and goto state 5)
     *         0 a 2 d 5 | e$       | s6 (state 5, at e, shift and goto state 6)
     *     0 a 2 d 5 e 6 | $        | rB₂ (state 6, at $, reduce by 2 to B)
     *         0 a 2 d 5 | B$       | g9 (state 5, at B, goto 9)
     *     0 a 2 d 5 B 9 | $        | rB₁ (state 9, at $, reduce by 4 to B)
     *             0 a 2 | B$       | g3 (state 2, at B, goto 3)
     *         0 a 2 B 3 | $        | rA₁ (state 3, at $, reduce by 4 to A)
     *                 0 | A$       | g1 (state 0, goto 1)
     *             0 A 1 | $        | a (state 1, at $, accept input)
     *
     * I added the non-terminals to the input (after reduction) for easier
     * understanding. The input should NEVER be mutated.
     *
     * The parsing can be represented with the this parse tree. The tree
     * should be viewed from bottom to top, from right to left.
     *
     *                          A                                   ^
     *              /                    \                          |
     *             |                      B                         |
     *             |           /                   \                |
     *             |          |                     B               |
     *             |          |                     |               |
     *             a          d                     e               |
     */
    public class SLR1Parsing : BaseLR0Parsing<SLR1Action>
    {
        public SLR1Parsing(CNFGrammar cnfGrammar)
            : base(cnfGrammar)
        { }

        //The parsing table has to be constructed to find any classification
        //issues
        public override void Classify()
            => this.ClassifyLR1(this.CreateParsingTable());

        public override IParsingTable<SLR1Action> CreateParsingTable()
        {
            if(this.ParsingTable is not null)
            {
                return this.ParsingTable;
            }

            //Construct the LR(0) automaton
            IAutomaton<LR0KernelItem> automaton = this.CreateAutomaton();
            //Constrcut the follow set
            IDictionary<Token, HashSet<Token>> followSet
                = this.CNFGrammar.ComputeFollowSet();
            Token end = new Token(TokenType.EOF);
            SLR1Action actions;
            ParsingTable<SLR1Action> table = new ParsingTable<SLR1Action>();

            /* Each state in the automaton can be seen as a row in the parsing 
             * table */
            foreach(State<LR0KernelItem> state in automaton)
            {
                /* Initialise a new action (it can be considered a row in the 
                 * parsing table) */
                actions = new SLR1Action();

                /* Each transition is either a shift (on a terminal) or a goto
                 * (on a non-terminal) */
                foreach(KeyValuePair<Token, State<LR0KernelItem>> kv
                        in state.Transitions)
                {
                    actions.Add
                    (
                        kv.Key,
                        new LR1ActionItem<LR0KernelItem>(kv.Value)
                    );
                }

                /* A reduction occurs when the end of a production has been
                 * reached */
                foreach(LR0KernelItem item in state.Items)
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
                        //If the end of the production has been reached
                        if(item.Index == item.Production.Body.Count)
                        {
                            if(!followSet.ContainsKey(item.Production.Head))
                            {
                                continue;
                            }

                            /* If the end of the production has been reached,
                             * reduce to the follow set of the head (a
                             * non-terminal), the look-ahead. This should reduce
                             * parsing table size. */
                            foreach(Token token
                                    in followSet[item.Production.Head])
                            {
                                this.AddReduceAction
                                (
                                    actions,
                                    item.Production,
                                    token
                                );
                            }
                        }
                    }
                }

                table.Add(actions);
            }

            return this.ParsingTable = table;
        }
    }
}

