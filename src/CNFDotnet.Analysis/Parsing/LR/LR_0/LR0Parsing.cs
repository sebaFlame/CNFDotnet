using System;
using System.Collections.Generic;

using CNFDotnet.Analysis.Grammar;
using CNFDotnet.Analysis.Parsing;
using CNFDotnet.Analysis.Parsing.LR;

namespace CNFDotnet.Analysis.Parsing.LR.LR_0
{
    public class LR0Parsing : BaseLRParsing<LR0Action>
    {
        public LR0Parsing (CNFGrammar grammar) 
            : base(grammar)
        {
        }

        protected override Kernel CreateKernel ()
        {
            Kernel kernel = new Kernel();
            kernel.Add(new LR0KernelItem(null, 0));
            return kernel;
        }

        protected override Kernel CreateClosure (Kernel kernel)
        {
            Token start = this.CNFGrammar.Start.Value;
            Kernel result = new Kernel();
            LR0KernelItem item;
            Token? token;
            HashSet<Production> used = new HashSet<Production>();
            Kernel added;

            for(int i = 0; i < kernel.Count; i++)
            {
                result.Add(new LR0KernelItem(kernel[i].Production, kernel[i].Index));
            }

            do
            {
                added = new Kernel();

                for(int i = 0; i < result.Count; i++)
                {
                    item = result[i] as LR0KernelItem;

                    if(item.Production is null)
                    {
                        if(item.Index == 0)
                        {
                            token = start;
                        }
                        else
                        {
                            token = null;
                        }
                    }
                    else if(item.Index < item.Production.Right.Count)
                    {
                        token = item.Production.Right[item.Index];
                    }
                    else
                    {
                        token = null;
                    }

                    if(!token.HasValue)
                    {
                        continue;
                    }

                    foreach(Production production in this.CNFGrammar.Productions)
                    {
                        if(!used.Contains(production)
                            && production.Left == token.Value)
                        {
                            added.Add(new LR0KernelItem(production, 0));
                            used.Add(production);
                        }
                    }
                }

                for(int i = 0; i < added.Count; i++)
                {
                    result.Add(added[i]);
                }

            } while(added.Count > 0);

            return result;
        }

        protected override IDictionary<Token, Kernel> CreateTransitions (Kernel closure)
        {
            Token start = this.CNFGrammar.Start.Value;
            Dictionary<Token, Kernel> result = new Dictionary<Token, Kernel>();
            Token? token;

            foreach (LR0KernelItem item in closure)
            {
                if(item.Production is null)
                {
                    if(item.Index == 0)
                    {
                        token = start;
                    }
                    else
                    {
                        token = null;
                    }
                }
                else if(item.Index < item.Production.Right.Count)
                {
                    token = item.Production.Right[item.Index];
                }
                else
                {
                    token = null;
                }

                if(!token.HasValue)
                {
                    continue;
                }

                if(!result.ContainsKey(token.Value))
                {
                    result.Add(token.Value, new Kernel());
                }

                result[token.Value].Add(new LR0KernelItem(item.Production, item.Index + 1));
            }

            return result;
        }

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