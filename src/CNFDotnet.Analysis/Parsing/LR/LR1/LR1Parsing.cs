using System;
using System.Collections.Generic;
using System.Linq;

using CNFDotnet.Analysis.Grammar;

namespace CNFDotnet.Analysis.Parsing.LR.LR1
{
    public class LR1Parsing : BaseLRParsing<LR1Action>
    {
        public LR1Parsing(CNFGrammar cnfGrammar)
            : base(cnfGrammar)
        { }

        protected override Kernel CreateKernel ()
        {
            Kernel kernel = new Kernel();
            kernel.Add(new LR1KernelItem(null, 0, new Token(TokenType.EOF)));
            return kernel;
        }

        protected override Kernel CreateClosure (Kernel kernel)
        {
            Token start = this.CNFGrammar.Start.Value;
            Dictionary<Production, HashSet<Token>> used = new Dictionary<Production, HashSet<Token>>();
            Kernel result, added;
            Token[] remaining, lookaheads;
            Token token;

            foreach(Production production in this.CNFGrammar.Productions)
            {
                used.Add(production, new HashSet<Token>());
            }

            result = new Kernel();

            foreach(LR1KernelItem lr1KernelItem in kernel)
            {
                result.Add(new LR1KernelItem(lr1KernelItem.Production, lr1KernelItem.Index, lr1KernelItem.LookAhead));
            }

            do
            {
                added = new Kernel();

                foreach(LR1KernelItem item in result)
                {
                    if(item.Production is null)
                    {

                        if(item.Index == 0)
                        {
                            remaining = new Token[] { start };
                        }
                        else
                        {
                            remaining = Array.Empty<Token>();
                        }
                    }
                    else
                    {
                        remaining = item.Production.Right.Skip(item.Index).ToArray();
                    }

                    if(remaining.Length == 0)
                    {
                        continue;
                    }

                    token = remaining[0];

                    lookaheads = this.CNFGrammar.GetFirst
                        (
                            remaining
                                .Skip(1)
                                .Concat
                                (
                                    new Token[]
                                    { 
                                        item.LookAhead 
                                    }
                                )
                        )
                        .ToArray();

                    foreach(Production production in this.CNFGrammar.Productions)
                    {
                        if(production.Left.Equals(token))
                        {
                            foreach(Token l in lookaheads)
                            {
                                if(!used[production].Contains(l))
                                {
                                    added.Add(new LR1KernelItem(production, 0, l));
                                    used[production].Add(l);
                                }
                            }
                        }
                    }
                }

                foreach(LR1KernelItem lr1KernelItem  in added)
                {
                    result.Add(lr1KernelItem);
                }

            }while(added.Count > 0);

            return result;
        }

        protected override IDictionary<Token, Kernel> CreateTransitions (Kernel closure)
        {
            Dictionary<Token, Kernel> result = new Dictionary<Token, Kernel>();
            Token start = this.CNFGrammar.Start.Value;
            Token? token;

            foreach(LR1KernelItem item in closure)
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

                result[token.Value].Add(new LR1KernelItem(item.Production, item.Index + 1, item.LookAhead));
            }

            return result;
        } 

        public override void Classify ()
        {
            this.ClassifyLR1(this.CreateParsingTable());
        }

        public override IParsingTable<LR1Action> CreateParsingTable ()
        {
            if(this.ParsingTable is not null)
            {
                return this.ParsingTable;
            }

            IList<State> automaton = this.CreateAutomaton();
            ParsingTable<LR1Action> table = new ParsingTable<LR1Action>();
            LR1Action actions;
            Token end = new Token(TokenType.EOF);

            foreach(State state in automaton)
            {
                actions = new LR1Action();

                foreach(KeyValuePair<Token, State> kv in state.Transitions)
                {
                    actions.Add(kv.Key, new LR1ActionItem(kv.Value));
                }

                foreach(LR1KernelItem item in state.Items)
                {
                    if(item.Production is null)
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
                            this.AddReduceAction(actions, item.LookAhead, item.Production);
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
