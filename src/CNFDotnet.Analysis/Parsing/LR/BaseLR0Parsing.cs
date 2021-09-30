using System;
using System.Collections.Generic;

using CNFDotnet.Analysis.Grammar;
using CNFDotnet.Analysis.Parsing;
using CNFDotnet.Analysis.Parsing.LR;

namespace CNFDotnet.Analysis.Parsing.LR
{
    public abstract class BaseLR0Parsing<TAction> : BaseLRParsing<TAction, LR0KernelItem>
        where TAction : class, IAction
    {
        public BaseLR0Parsing (CNFGrammar grammar) 
            : base(grammar)
        { }

        protected override Kernel<LR0KernelItem> CreateKernel ()
        {
            Kernel<LR0KernelItem> kernel = new Kernel<LR0KernelItem>();
            kernel.Add(new LR0KernelItem(Production.Null, 0));
            return kernel;
        }

        protected override Kernel<LR0KernelItem> CreateClosure (Kernel<LR0KernelItem> kernel)
        {
            Token start = this.CNFGrammar.Start.Value;
            Kernel<LR0KernelItem> result = new Kernel<LR0KernelItem>();
            LR0KernelItem item;
            Token? token;
            HashSet<Production> used = new HashSet<Production>();
            Kernel<LR0KernelItem> added;

            for(int i = 0; i < kernel.Count; i++)
            {
                result.Add(new LR0KernelItem(kernel[i].Production, kernel[i].Index));
            }

            do
            {
                added = new Kernel<LR0KernelItem>();

                for(int i = 0; i < result.Count; i++)
                {
                    item = result[i] as LR0KernelItem;

                    if(item.Production.Equals(Production.Null))
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

        protected override IDictionary<Token, Kernel<LR0KernelItem>> CreateTransitions (Kernel<LR0KernelItem> closure)
        {
            Token start = this.CNFGrammar.Start.Value;
            Dictionary<Token, Kernel<LR0KernelItem>> result = new Dictionary<Token, Kernel<LR0KernelItem>>();
            Token? token;

            foreach (LR0KernelItem item in closure)
            {
                if(item.Production.Equals(Production.Null))
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
                    result.Add(token.Value, new Kernel<LR0KernelItem>());
                }

                result[token.Value].Add(new LR0KernelItem(item.Production, item.Index + 1));
            }

            return result;
        }
    }
}