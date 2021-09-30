using System;
using System.Collections.Generic;

using CNFDotnet.Analysis.Grammar;
using CNFDotnet.Analysis.Parsing.LR.LR1;

namespace CNFDotnet.Analysis.Parsing.LR.LALR1
{
    public class LALR1Parsing : BaseLRParsing<LALR1Action, LALR1KernelItem>
    {
        protected override Kernel<LALR1KernelItem> CreateKernel () => throw new NotImplementedException();
        protected override Kernel<LALR1KernelItem> CreateClosure (Kernel<LALR1KernelItem> kernel) => throw new NotImplementedException();
        protected override IDictionary<Token, Kernel<LALR1KernelItem>> CreateTransitions (Kernel<LALR1KernelItem> closure) => throw new NotImplementedException();

        public LALR1Parsing(CNFGrammar cnfGrammar)
            : base(cnfGrammar)
        { }

        public override IList<State<LALR1KernelItem>> CreateAutomaton()
        {
            if(this.Automaton is not null)
            {
                return this.Automaton;
            }

            LR1Parsing lr1Parsing = new LR1Parsing(this.CNFGrammar);
            IList<State<LR1KernelItem>> lr1Automaton = lr1Parsing.CreateAutomaton();
            List<State<LALR1KernelItem>> intermediateAutomaton = new List<State<LALR1KernelItem>>();
            Dictionary<State<LR1KernelItem>, State<LALR1KernelItem>> translation = new Dictionary<State<LR1KernelItem>, State<LALR1KernelItem>>();
            State<LALR1KernelItem> intermediateState;

            foreach(State<LR1KernelItem> lr1State in lr1Automaton)
            {
                intermediateAutomaton.Add
                (
                    intermediateState = new State<LALR1KernelItem>
                    (
                        lr1State.Index,
                        this.CollapseLookaheads(lr1State.Kernel),
                        this.CollapseLookaheads(lr1State.Items)
                    )
                );

                translation.Add(lr1State, intermediateState);
            }

            foreach(State<LR1KernelItem> lr1State in lr1Automaton)
            {
                if(translation.TryGetValue(lr1State, out intermediateState))
                {
                    if(lr1State.Transitions is null)
                    {
                        continue;
                    }

                    intermediateState.Transitions = new Dictionary<Token, State<LALR1KernelItem>>();
                    foreach(KeyValuePair<Token, State<LR1KernelItem>> kv in lr1State.Transitions)
                    {
                        intermediateState.Transitions.Add(kv.Key, translation[kv.Value]);
                    }
                }
            }

            HashSet<State<LALR1KernelItem>> used = new HashSet<State<LALR1KernelItem>>();
            List<List<State<LALR1KernelItem>>> merge = new List<List<State<LALR1KernelItem>>>();

            foreach(State<LALR1KernelItem> outerState in intermediateAutomaton)
            {
                if(used.Contains(outerState))
                {
                    continue;
                }

                List<State<LALR1KernelItem>> m = new List<State<LALR1KernelItem>>();

                foreach(State<LALR1KernelItem> innerState in intermediateAutomaton)
                {
                    if(!used.Contains(innerState)
                       && outerState.Kernel.Equals(innerState.Kernel))
                    {
                        m.Add(innerState);
                        used.Add(innerState);
                    }
                }

                merge.Add(m);
            }

            Dictionary<State<LALR1KernelItem>, int> transitions = new Dictionary<State<LALR1KernelItem>, int>();

            for(int i = 0; i < merge.Count; i++)
            {
                foreach(State<LALR1KernelItem> mergeState in merge[i])
                {
                    transitions.Add(mergeState, i);
                }
            }

            List<State<LALR1KernelItem>> states = new List<State<LALR1KernelItem>>();
            State<LALR1KernelItem> state = null;
            IDictionary<Token, State<LALR1KernelItem>> original;

            for(int i = 0; i < merge.Count; i++)
            {
                state = new State<LALR1KernelItem>(states.Count);

                foreach(State<LALR1KernelItem> mergeState in merge[i])
                {
                    state.Kernel = this.MergeItems(mergeState.Kernel, state?.Kernel);
                    state.Items = this.MergeItems(mergeState.Items, state?.Items);
                }


                states.Add(state);
            }

            for(int i = 0; i < merge.Count; i++)
            {
                state = states[i];
                original = merge[i][0].Transitions;
                state.Transitions = new Dictionary<Token, State<LALR1KernelItem>>();

                foreach(KeyValuePair<Token, State<LALR1KernelItem>> kv in original)
                {
                    state.Transitions.Add(kv.Key, states[transitions[kv.Value]]);
                }
            }

            this.Automaton = states;

            return states;
        }

        public override void Classify ()
        {
            this.ClassifyLR1(this.CreateParsingTable());
        }

        public override IParsingTable<LALR1Action> CreateParsingTable ()
        {
            if(this.ParsingTable is not null)
            {
                return this.ParsingTable;
            }

            Token end = new Token(TokenType.EOF);
            LALR1Action actions;
            ParsingTable<LALR1Action> table = new ParsingTable<LALR1Action>();
            IList<State<LALR1KernelItem>> automaton = this.CreateAutomaton();
            
            foreach(State<LALR1KernelItem> state in automaton)
            {
                actions = new LALR1Action();

                foreach(KeyValuePair<Token, State<LALR1KernelItem>> kv in state.Transitions)
                {
                    actions.Add(kv.Key, new LR1ActionItem<LALR1KernelItem>(kv.Value));
                }

                foreach(LALR1KernelItem item in state.Items)
                {
                    if(item.Production.Equals(Production.Null))
                    {
                        if(item.Index == 1)
                        {
                            this.AddReduceAction(actions, end, item.Production);
                        }
                    }
                    else
                    {
                        if(item.Index == item.Production.Right.Count)
                        {
                            foreach(Token token in item.LookAheads)
                            {
                                this.AddReduceAction(actions, token, item.Production);
                            }
                        }
                    }
                }

                table.Add(actions);
            }

            this.ParsingTable = table;

            return table;
        }

        private Kernel<LALR1KernelItem> CollapseLookaheads(Kernel<LR1KernelItem> items)
        {
            Production p;
            int x;
            Token l;

            Dictionary<Production, Dictionary<int, List<Token>>> table = new Dictionary<Production, Dictionary<int, List<Token>>>();

            foreach(LR1KernelItem lr1KernelItem in items)
            {
                p = lr1KernelItem.Production;
                x = lr1KernelItem.Index;
                l = lr1KernelItem.LookAhead;

                if(!table.ContainsKey(p))
                {
                    table.Add(p, new Dictionary<int, List<Token>>());
                }

                if(!table[p].ContainsKey(x))
                {
                    table[p].Add(x, new List<Token>());
                }

                table[p][x].Add(l);
            }

            Kernel<LALR1KernelItem> result = new Kernel<LALR1KernelItem>();
            LALR1KernelItem kernelItem;

            foreach(Production production in table.Keys)
            {
                foreach(int index in table[production].Keys)
                {
                    result.Add(new LALR1KernelItem(production, index, table[production][index]));
                }
            }

            return result;
        }

        private Kernel<LALR1KernelItem> MergeItems(Kernel<LALR1KernelItem> a, Kernel<LALR1KernelItem> b)
        {
            Kernel<LALR1KernelItem> result = new Kernel<LALR1KernelItem>();
            LALR1KernelItem kernelItem;

            foreach(LALR1KernelItem outerKernelItem in a)
            {
                kernelItem = new LALR1KernelItem(outerKernelItem.Production, outerKernelItem.Index);

                foreach(Token token in outerKernelItem.LookAheads)
                {
                    kernelItem.LookAheads.Add(token);
                }

                if(b is null)
                {
                    result.Add(kernelItem);
                    continue;
                }

                foreach(LALR1KernelItem innerKernelItem in b)
                {
                    if(outerKernelItem.Equals(innerKernelItem))
                    {
                        foreach(Token token in innerKernelItem.LookAheads)
                        {
                            if(!kernelItem.LookAheads.Contains(token))
                            {
                                kernelItem.LookAheads.Add(token);
                            }
                        }
                    }
                }

                result.Add(kernelItem);
            }

            return result;
        }
    }
}
