using System.Collections.Generic;
using System;

using CNFDotnet.Analysis.Grammar;
using CNFDotnet.Analysis.Parsing;

namespace CNFDotnet.Analysis.Parsing.LR
{
    public abstract class BaseLRParsing<TAction> : BaseParsing<TAction>
        where TAction : class, IAction
    {
        public IList<State> Automaton { get; private set; }

        protected BaseLRParsing (CNFGrammar cnfGrammar)
            : base (cnfGrammar)
        { }

        #region abstract implementation
        protected abstract Kernel CreateKernel ();

        protected abstract Kernel CreateClosure (Kernel kernel);

        protected abstract IDictionary<Token, Kernel> CreateTransitions (Kernel closure);
        #endregion

        protected IList<State> CreateAutomaton ()
        {
            if (this.Automaton is not null)
            {
                return this.Automaton;
            }

            int s = 0, l, i;
            List<State> states = new List<State>();
            State state;
            IDictionary<Token, Kernel> transitions;
            Kernel kernel;

            states.Add(new State(this.CreateKernel(), states.Count));

            while (s < states.Count)
            {
                for (l = states.Count; s < l; s++)
                {
                    state = states[s];

                    state.Items = this.CreateClosure(state.Kernel);
                    transitions = this.CreateTransitions(state.Items);

                    state.Transitions = new Dictionary<Token, State>();

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
                            states.Add(new State(kernel, states.Count));
                            state.Transitions.Add(token, states[states.Count - 1]);
                        }
                    }
                }
            }

            this.Automaton = states;

            return states;
        }

        //public void ClassifyLR_1()
        //{
        //}

        //public void AddReduceAction()
        //{
        //}

        //public void CollapseLookAheads()
        //{
        //}

        //public void MergeItems ()
        //{
        //}
    }
}