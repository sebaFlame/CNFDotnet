using System.Collections.Generic;

using CNFDotnet.Analysis.Grammar;

namespace CNFDotnet.Analysis.Parsing.LR.LALR1
{
    /* LALR(1) (Look Ahead LR(0) with a look-ahead of 1) with LR (Left-to-right,
     * Right-most derivation) is a form of deterministic bottom-up parsing with
     * a look-ahead of 1 (1) . Bottom-up meaning start parsing from the first
     * token of the input sentence (a terminal).
     *
     * Consider as an LALR(1) grammar. I removed all choices and numbered every
     * production for easy referenceing.
     *
     * S₁ -> L assign R
     * S₂ -> R
     * L₁ -> * R
     * L₂ -> id
     * R₁ -> L
     *
     * S being the start symbol, space as token seperator, $ representing
     * the end (EOF) and ε (epsilon) representing an empty (nullable) token.
     * Terminals are represented by non-capital letters and non-terminals start
     * with a capital letter.
     *
     * We first construct the LR(1) (!) automaton with • indicating the
     * position, digits on top indicating the state number and lines with a
     * token depicting transitions. I will first list all states, and then all
     * transitions, else it will be a mess. Ususally this is a single diagram.
     * There are no annotions, because there can be multiple actions per state.
     *
     *             0                 1                    2
     * |----------------------| |--------| |---------------------------|
     * | S -> •L assign R [$] | | S• [$] | | S -> L• assign R [$]      |
     * | S -> •R [$]          | +--------+ | R -> L• [$]               |
     * | L -> •* R [$]        |            +---------------------------+
     * | L -> •* R [assign]   |
     * | L -> •id [$]         |        3              10                13
     * | L -> •id [assign]    | |-------------| |-------------| |--------------|
     * | R -> •L [$]          | | S -> R• [$] | | R -> L• [$] | | L -> *R• [$] |
     * | •S [$]               | +-------------+ +-------------+ +--------------+
     * +----------------------|
     *            4                     5                     6
     * |-------------------| |-------------------| |----------------------|
     * | L -> •*R [$]      | | L -> id• [$]      | | S -> L assign •R [$] |
     * | L -> •*R [assign] | | L -> id• [assign] | | L -> •*R [$]         |
     * | L -> *•R [$]      | +-------------------+ | L -> •id [$]         |
     * | L -> *•R [assign] |           7           | R -> •L [$]          |
     * | L -> •id [$]      | |-------------------| +----------------------+
     * | L -> •id [assign] | | L -> *R• [$]      |        12
     * | R -> •L [$]       | | L -> *R• [assign] | |--------------|
     * | R -> •L [assign]  | +-------------------+ | L -> id• [$] |
     * +-------------------+                       +--------------+
     *          8                     9                     11
     * |------------------| |----------------------| |--------------|
     * | R -> L• [$]      | | S -> L assign R• [$] | | L -> •*R [$] |
     * | R -> L• [assign] | +----------------------+ | L -> *•R [$] |
     * +------------------+                          | L -> •id [$] |
     *                                               | R -> •L [$]  |
     *                                               +--------------+
     * 0 -S--> 1      2 -assign-> 6      6 -R--> 9      11 -L--> 10
     * 0 -L--> 2      4 -*------> 4      6 -L--> 10     11 -R--> 13
     * 0 -R--> 3      4 -R------> 7      6 -*--> 11     11 -id-> 12
     * 0 -*--> 4      4 -L------> 8      6 -id-> 12
     * 0 -id-> 5      4 -id-----> 5     11 -*--> 11
     *
     * We'll start at state 0. S is the start token. We start at index 0. The
     * start is always followed by the EOF, so add this as a look-ahead. S can
     * be resolved into 2 productions. These all start with a non-terminal and
     * can be derived further. Because the non-terminal 'L' is followed by a
     * terminal in the original production, this terminal (assign) can be added
     * as the look-ahead. First copy the original look-ahead ($), then add the
     * new look-ahead. L can also be further resolved into 2 productions (with
     * the same look-aheads). R can also be resolved and the following L
     * derivation is already added. One can transtion to state 4 from state 0 by
     * iteration over the terminal '*'. The index gets moved and all look-aheads
     * get copied over into the new state. 'R' resolves back into 'L', so all
     * productions for 'L' get re-added with the look-aheads.
     *
     * From this LR(1) we collapse states into an LR(0) automaton by ignoring
     * the look-aheads. For each (LR(0)) core we can merge the look-aheads into
     * a single set. 
     *             0                 1                    2
     * |----------------------| |--------| |----------------------|
     * | S -> •L assign R [$] | | S• [$] | | S -> L• assign R [$] |
     * | S -> •R [$]          | +--------+ | R -> L• [$]          |
     * | L -> •* R [$,assign] |            +----------------------+
     * | L -> •id [$,assign]  |         3                 
     * | R -> •L [$]          |  |-------------|
     * | •S [$]               |  | S -> R• [$] |
     * +----------------------+  +-------------+
     *            4                     5                         6
     * |---------------------| |---------------------| |----------------------|
     * | L -> •*R [$,assign] | | L -> id• [$,assign] | | S -> L assign •R [$] |
     * | L -> *•R [$,assign] | +---------------------+ | L -> •*R [$]         |
     * | L -> •id [$,assign] |                         | L -> •id [$]         |
     * | R -> •L [$,assign]  |                         | R -> •L [$]          |
     * +---------------------+                         +----------------------+
     *           7                      8                         9
     * |---------------------| |--------------------| |----------------------|
     * | L -> *R• [$,assign] | | R -> L• [$,assign] | | S -> L assign R• [$] |
     * +---------------------+ +--------------------+ +----------------------+
     *
     * 0 -S--> 1      2 -assign-> 6      6 -R--> 9
     * 0 -L--> 2      4 -*------> 4      6 -L--> 8
     * 0 -R--> 3      4 -R------> 7      6 -*--> 4
     * 0 -*--> 4      4 -L------> 8      6 -id-> 5
     * 0 -id-> 5      4 -id-----> 5
     *
     * States 0, 4, 5, 7 and 8 can be collapsed to about half the items. If we
     * ignore the look-aheads we can merge 4 & 11, 5 & 12, 7 & 13 and 8 & 10.
     * Notice states 4 & 11 having the same transitions. The LALR(1) automaton
     * now has 4 less states and also 4 less transitions.
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
     * State | assign *     id    $  |  S     L     R
     * ------+-----------------------+-----------------
     *     0 |       s4    s5        | g1    g2    g3
     *     1 |                    a  |
     *     2 | s6                rR₁ | 
     *     3 |                   rS₂ |
     *     4 |       s4    s5        |       g8    g7
     *     5 | rL₂               rL₂ |
     *     6 |       s4    s5        |       g8    g9
     *     7 | rL₁               rL₁ |
     *     8 | rR₁               rR₁ |
     *     9 |                   rS₁ |
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
     * id assign * id
     *
     *                         0 | id assign * id$ | s5
     *                    0 id 5 |    assign * id$ | rL₂ (L_1 -> id)
     *                         0 |  L assign * id$ | g2
     *                     0 L 2 |    assign * id$ | s6
     *            0 L 2 assign 6 |           * id$ | s4
     *        0 L 2 assign 6 * 4 |             id$ | s5
     *   0 L 2 assign 6 * 4 id 5 |               $ | rL₂ (L_2 -> id)
     *        0 L 2 assign 6 * 4 |              L$ | g8
     *    0 L 2 assign 6 * 4 L 8 |               $ | rR₁ (R_3 -> L_2)
     *        0 L 2 assign 6 * 4 |              R$ | g7
     *    0 L 2 assign 6 * 4 R 7 |               $ | rL₁ (L_4 -> * R_3)
     *            0 L 2 assign 6 |              L$ | g8
     *        0 L 2 assign 6 L 8 |               $ | rR₁ (R_5 -> L_4)
     *            0 L 2 assign 6 |              R$ | g9
     *        0 L 2 assign 6 R 9 |               $ | rS₁ (S_6 -> L_1 assign R_5)
     *                         0 |              S$ | g1
     *                     0 S 1 |               $ | a
     *
     * I added the non-terminals to the input (after reduction) for easier
     * understanding. The input should NEVER be mutated.
     *
     * We can construct this tree by following all reductions on the stack. The
     * subscript digits after the terminals are the positions in the input. The
     * righ-hand side for each item can be found on the stack by removing the
     * state numbers and taking a number of tokens equal to the length of the
     * reduction. Annotate the tokens for easy referencing (eg with index for
     * non-terminals and position for terminals).
     *
     * L_1 -> id₁                                                          ^
     * L_2 -> id₄                                                          |
     * R_3 -> L_2                                                          |
     * L_4 -> *₃ R_3                                                       |
     * R_5 -> L_4                                                          |
     * S_6 -> L_1 assign₂ R_5                                              |
     *
     * If we follow all relations (a grammar!) we can generate this parse tree.
     *
     *                              S (S_6)                           
     *              /                 |               \           
     *             L (L_1)            |                R (R_5)
     *             |                  |                |          
     *             |                  |                L (L_4)
     *             |                  |          /            \   
     *             |                  |         |              R (R_3)
     *             |                  |         |              |  
     *             |                  |         |              L (L_2)
     *             |                  |         |              |  
     *            id₁              assign₂      *₃            id₄
     *
     */
    public class LALR1Parsing : BaseLR1Parsing<LALR1Action, LALR1KernelItem>
    {
        public LALR1Parsing(CNFGrammar cnfGrammar)
            : base(cnfGrammar)
        { }

        protected override LALR1KernelItem CreateKernelItem
            (Production production, int index, IEnumerable<Token> lookAheads)
            => new LALR1KernelItem
            (
                production,
                index,
                lookAheads
            );

        public override IAutomaton<LALR1KernelItem> CreateAutomaton()
        {
            if(this.Automaton is not null)
            {
                return this.Automaton;
            }

            //Create the LR(1) automaton, to base the LALR(1) automaton on
            IAutomaton<LALR1KernelItem> lr1Automaton
                = base.CreateAutomaton();

            //Initialize temporary items for merging
            List<State<LALR1KernelItem>> intermediateAutomaton
                = new List<State<LALR1KernelItem>>();
            Dictionary<State<LALR1KernelItem>, State<LALR1KernelItem>> translation
                = new Dictionary
                    <State<LALR1KernelItem>, State<LALR1KernelItem>>();
            State<LALR1KernelItem> intermediateState;

            //For each state, collapse all same (LR(0) comparison) kernel/items
            //into a single new kernel item
            foreach(State<LALR1KernelItem> lr1State in lr1Automaton)
            {
                intermediateAutomaton.Add
                (
                    intermediateState = new State<LALR1KernelItem>
                    (
                        lr1State.Index,
                        LALR1Parsing.CollapseLookaheads(lr1State.Kernel),
                        LALR1Parsing.CollapseLookaheads(lr1State.Items)
                    )
                );

                //Keep a translation dictionary for easy referencing
                translation.Add(lr1State, intermediateState);
            }

            //Add the transition from the original LR(1) automaton, referencing
            //the new LALR(1) states
            foreach(State<LALR1KernelItem> lr1State in lr1Automaton)
            {
                if(translation.TryGetValue(lr1State, out intermediateState))
                {
                    if(lr1State.Transitions is null)
                    {
                        continue;
                    }

                    intermediateState.Transitions
                        = new Dictionary<Token, State<LALR1KernelItem>>();

                    //Lookup the correct new state, and add as transition
                    foreach(KeyValuePair<Token, State<LALR1KernelItem>> kv
                            in lr1State.Transitions)
                    {
                        intermediateState.Transitions.Add
                        (
                            kv.Key,
                            translation[kv.Value]
                        );
                    }
                }
            }

            HashSet<State<LALR1KernelItem>> used
                = new HashSet<State<LALR1KernelItem>>();
            List<List<State<LALR1KernelItem>>> merge
                = new List<List<State<LALR1KernelItem>>>();
            IKernel<BaseLR0KernelItem> outerKernel, innerKernel;

            //Find all same states and initialize a list for merging
            foreach(State<LALR1KernelItem> outerState in intermediateAutomaton)
            {
                //If it has already been merged, continue
                if(used.Contains(outerState))
                {
                    continue;
                }

                List<State<LALR1KernelItem>> m
                    = new List<State<LALR1KernelItem>>();

                outerKernel = outerState.Kernel;

                //Compare every item with every item
                foreach(State<LALR1KernelItem> innerState
                        in intermediateAutomaton)
                {
                    innerKernel = innerState.Kernel;

                    //A state is considered equal if the kernel is LR(0) same
                    //(excluding look-aheads in comparison)
                    if(!used.Contains(innerState)
                       && outerKernel.OrderlessSequenceEqual(innerKernel))
                    {
                        m.Add(innerState);
                        used.Add(innerState);
                    }
                }

                merge.Add(m);
            }

            Automaton<LALR1KernelItem> states
                = new Automaton<LALR1KernelItem>();
            State<LALR1KernelItem> state;
            IDictionary<Token, State<LALR1KernelItem>> original;

            //Merge all mergeable states into new states
            for(int i = 0; i < merge.Count; i++)
            {
                //Initialize new state
                state = new State<LALR1KernelItem>(states.Count);

                //Merge all states of the merge into the new state
                foreach(State<LALR1KernelItem> mergeState in merge[i])
                {
                    //Create new kernel from the empty (or updated) state
                    state.Kernel = LALR1Parsing.MergeItems
                    (
                        mergeState.Kernel,
                        state?.Kernel
                    );
                    //Create new kernel from the empty (or updated) state
                    state.Items = LALR1Parsing.MergeItems
                    (
                        mergeState.Items,
                        state?.Items
                    );
                }

                //Add the merged state
                states.Add(state);
            }

            //Merge and state index are now the same!
            Dictionary<State<LALR1KernelItem>, int> transitions
                = new Dictionary<State<LALR1KernelItem>, int>();

            //Add the merged index as transition target of the merged states
            for(int i = 0; i < merge.Count; i++)
            {
                //Save the index of the merge (equal to the index of states) for
                //each item of the merge, because all these items are considered
                //equal thus reference state i
                foreach(State<LALR1KernelItem> mergeState in merge[i])
                {
                    transitions.Add(mergeState, i);
                }
            }

            for(int i = 0; i < merge.Count; i++)
            {
                //Find the new state
                state = states[i];

                //Only the transitions of the first item are needed, because all
                //merged items have the same transition set
                original = merge[i][0].Transitions;

                //Initialize new transition list
                state.Transitions
                    = new Dictionary<Token, State<LALR1KernelItem>>();

                foreach(KeyValuePair<Token, State<LALR1KernelItem>> kv
                        in original)
                {
                    //Merge transitions using the original state index,
                    //translating to the new state index (same index as merge)
                    state.Transitions.Add
                    (
                        kv.Key,
                        states[transitions[kv.Value]]
                    );
                }
            }

            this.Automaton = states;

            return states;
        }

        public override void Classify()
            => this.ClassifyLR1(this.CreateParsingTable());

        public override IParsingTable<LALR1Action> CreateParsingTable()
        {
            if(this.ParsingTable is not null)
            {
                return this.ParsingTable;
            }

            //First create the automaton with look-aheads
            IAutomaton<LALR1KernelItem> automaton = this.CreateAutomaton();
            ParsingTable<LALR1Action> table = new ParsingTable<LALR1Action>();
            LALR1Action actions;
            Token end = new Token(TokenType.EOF);

            //Each state in the automaton can be seen as a row in the parsing
            //table
            foreach(State<LALR1KernelItem> state in automaton)
            {
                //Initalize actions (a row in the parsing table)
                actions = new LALR1Action();

                //Each item in the transitions is a shift iterating over a token
                //to a new state
                foreach(KeyValuePair<Token, State<LALR1KernelItem>> kv
                        in state.Transitions)
                {
                    actions.Add
                    (
                        kv.Key, //the token
                        new LR1ActionItem<LALR1KernelItem>(kv.Value)
                    );
                }

                /* A reduction occurs when the end of a production has been
                 * reached */
                foreach(LALR1KernelItem item in state.Items)
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

        /* Collapse all (single look-ahead) LR(1) kernel items into a single
         * LALR(1) kernel item with multiple look-aheads */
        private static Kernel<LALR1KernelItem>
            CollapseLookaheads(IKernel<BaseLR1KernelItem> items)
        {
            LALR1KernelItem lalr1KernelItem;

            Kernel<LALR1KernelItem> result = new Kernel<LALR1KernelItem>();

            foreach(BaseLR1KernelItem item in items)
            {
                //Use LR(0) for comparison (production and index only)
                if(!result.Contains
                   ((BaseLR0KernelItem)item, out lalr1KernelItem))
                {
                    result.Add
                    (
                        lalr1KernelItem = new LALR1KernelItem
                        (
                            item
                        )
                    );
                }

                //Add (single) look-ahead for item
                foreach(Token lookAhead in item.LookAheads)
                {
                    lalr1KernelItem.AddLookAhead(lookAhead);
                }
            }

            return result;
        }

        //Merge multiple kernels into a single one, merging all look-aheads
        private static Kernel<LALR1KernelItem>
            MergeItems(Kernel<LALR1KernelItem> a, Kernel<LALR1KernelItem> b)
        {
            Kernel<LALR1KernelItem> result = new Kernel<LALR1KernelItem>();
            LALR1KernelItem kernelItem;

            //Return new kernel with all items from a 
            if(b is null)
            {
                foreach(LALR1KernelItem outerKernelItem in a)
                {
                    result.Add(new LALR1KernelItem(outerKernelItem));
                }
            }
            //Return new kernel with all items from b
            else if(a is null)
            {
                foreach(LALR1KernelItem outerKernelItem in b)
                {
                    result.Add(new LALR1KernelItem(outerKernelItem));
                }
            }
            else
            {
                //Add all items from a initially
                foreach(LALR1KernelItem outerKernelItem in a)
                {
                    result.Add(new LALR1KernelItem(outerKernelItem));
                }

                foreach(LALR1KernelItem innerKernelItem in b)
                {
                    //Add a new item if it does not exist yet
                    if(!result.Contains
                       ((BaseLR0KernelItem)innerKernelItem, out kernelItem))
                    {
                        result.Add(new LALR1KernelItem(innerKernelItem));
                    }
                    //Else merge all look-aheads
                    else
                    {
                        foreach(Token token in innerKernelItem.LookAheads)
                        {
                            kernelItem.AddLookAhead(token);
                        }
                    }
                }
            }

            return result;
        }

    }
}
