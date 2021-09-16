using System;
using System.Collections.Generic;

using CNFDotnet.Analysis.Grammar;
using CNFDotnet.Analysis.Parsing;
using CNFDotnet.Analysis.Parsing.LR;

namespace CNFDotnet.Analysis.Parsing.LR.LR0
{
    public class LR0Parsing : BaseLR0Parsing<LR0Action>
    {
        public LR0Parsing (CNFGrammar grammar) 
            : base(grammar)
        { }

        public override ParsingTable<LR0Action> CreateParsingTable ()
        {
            if(this.ParsingTable is not null)
            {
                return this.ParsingTable;
            }

            ParsingTable<LR0Action> table = new ParsingTable<LR0Action>();
            IList<State> automaton = this.CreateAutomaton();
            LR0Action action;

            foreach(State state in automaton)
            {
                action = new LR0Action();

                foreach(Token s in state.Transitions.Keys)
                {
                    action.Shift.Add(s, state.Transitions[s]);
                }

                foreach(LR0KernelItem item in state.Items)
                {
                    if(item.Production is null)
                    {
                        if(item.Index == 1)
                        {
                            action.Reduce.Add(item.Production);
                        }
                    }
                    else
                    {
                        if(item.Index == item.Production.Right.Count)
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

        public override void Classify ()
        {
            ParsingTable<LR0Action> table = this.CreateParsingTable();
            HashSet<Token> terminals = this.CNFGrammar.Terminals;

            foreach(LR0Action action in table)
            {
                if(action.Reduce.Count > 1)
                {
                    throw new LR0ClassificationException("Grammar contains an LR(0) reduce-reduce conflict");
                }

                if(action.Reduce.Count > 0)
                {
                    foreach (Token s in action.Shift.Keys)
                    {
                        if(terminals.Contains(s))
                        {
                            throw new LR0ClassificationException("Grammar contains an LR(0) shift-reduce conflict");
                        }
                    }
                }
            }
        }
    }
}