using System;
using System.Collections.Generic;

using CNFDotnet.Analysis.Grammar;
using CNFDotnet.Analysis.Parsing;
using CNFDotnet.Analysis.Parsing.LR;

namespace CNFDotnet.Analysis.Parsing.LR
{
    public abstract class BaseLR0Parsing<TAction> : BaseLRParsing<TAction>
        where TAction : class, IAction
    {
        public BaseLR0Parsing (CNFGrammar grammar) 
            : base(grammar)
        { }

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
    }
}