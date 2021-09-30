using System.Collections.Generic;
using System;

using CNFDotnet.Analysis.Grammar;
using CNFDotnet.Analysis.Parsing;

namespace CNFDotnet.Analysis.Parsing.LR
{
    public abstract class BaseLRParsing<TAction, TKernelItem> : BaseParsing<TAction>
        where TAction : class, IAction
        where TKernelItem : BaseKernelItem
    {
        public IList<State<TKernelItem>> Automaton { get; protected set; }

        protected BaseLRParsing (CNFGrammar cnfGrammar)
            : base (cnfGrammar)
        { }

        #region abstract implementation
        protected abstract Kernel<TKernelItem> CreateKernel ();

        protected abstract Kernel<TKernelItem> CreateClosure (Kernel<TKernelItem> kernel);

        protected abstract IDictionary<Token, Kernel<TKernelItem>> CreateTransitions (Kernel<TKernelItem> closure);
        #endregion

        public virtual IList<State<TKernelItem>> CreateAutomaton ()
        {
            if (this.Automaton is not null)
            {
                return this.Automaton;
            }

            int s = 0, l, i;
            List<State<TKernelItem>> states = new List<State<TKernelItem>>();
            State<TKernelItem> state;
            IDictionary<Token, Kernel<TKernelItem>> transitions;
            Kernel<TKernelItem> kernel;

            states.Add(new State<TKernelItem>(this.CreateKernel(), states.Count));

            while (s < states.Count)
            {
                for (l = states.Count; s < l; s++)
                {
                    state = states[s];

                    state.Items = this.CreateClosure(state.Kernel);
                    transitions = this.CreateTransitions(state.Items);

                    state.Transitions = new Dictionary<Token, State<TKernelItem>>();

                    foreach (Token token in transitions.Keys)
                    {
                        kernel = transitions[token];

                        for (i = 0; i < states.Count; i++)
                        {
                            if (kernel.Equals(states[i].Kernel))
                            {
                                state.Transitions.Add(token, states[i]);
                                break;
                            }
                        }

                        if (i == states.Count)
                        {
                            states.Add(new State<TKernelItem>(kernel, states.Count));
                            state.Transitions.Add(token, states[states.Count - 1]);
                        }
                    }
                }
            }

            this.Automaton = states;

            return states;
        }

        public void ClassifyLR1<T1Action>(IParsingTable<T1Action> table)
            where T1Action : BaseLR1Action<TKernelItem>
        {
            foreach(T1Action action in table)
            {
                foreach(KeyValuePair<Token, LR1ActionItem<TKernelItem>>  kv in action)
                {
                    if(kv.Value.Reduce is not null
                       && kv.Value.Reduce.Count > 1)
                    {
                        throw new LR1ClassificationException("Table contains a reduce-reduce conflict");
                    }

                    if(kv.Value.Shift is not null
                       && kv.Value.Reduce is not null
                       && kv.Value.Reduce.Count > 0)
                    {
                        throw new LR1ClassificationException("Table contains a shift-reduce conflict");
                    }
                }
            }
        }

        protected void AddReduceAction(BaseLR1Action<TKernelItem> actions, Token token, Production production)
        {
            LR1ActionItem<TKernelItem> actionItem;

            if(!actions.TryGetValue(token, out actionItem))
            {
                actions.Add(token, (actionItem = new LR1ActionItem<TKernelItem>()));
            }

            if(actionItem.Reduce is null)
            {
                actionItem.Reduce = new List<Production>();
            }

            actionItem.Reduce.Add(production);
        }

        //public void CollapseLookAheads()
        //{
        //}

        //public void MergeItems ()
        //{
        //}
    }
}