using System.Collections.Generic;

using CNFDotnet.Analysis.Grammar;

namespace CNFDotnet.Analysis.Parsing.LR
{
    public abstract class BaseLR0Parsing<TAction>
        : BaseLRParsing<TAction, LR0KernelItem>
        where TAction : class, IAction
    {
        public BaseLR0Parsing(CNFGrammar grammar)
            : base(grammar)
        { }

        protected override Kernel<LR0KernelItem> CreateInitialKernel()
            => new Kernel<LR0KernelItem>
                {
                    new LR0KernelItem(Production.Null, 0)
                };

        protected override Kernel<LR0KernelItem> CreateClosure
            (Kernel<LR0KernelItem> kernel)
        {
            Token start = this.CNFGrammar.ComputeStartNonTerminal();
            Kernel<LR0KernelItem> result = new Kernel<LR0KernelItem>();
            LR0KernelItem item;
            Token? token;
            HashSet<Production> used = new HashSet<Production>();
            int initialCount;

            //The closure exists of the existing items
            for(int i = 0; i < kernel.Count; i++)
            {
                //TODO: reuse existing kernel item
                result.Add
                (
                    new LR0KernelItem
                    (
                        kernel[i].Production,
                        kernel[i].Index)
                    );
            }

            //Keep returning items while relations are being found
            do
            {
                //Save the current count to check if new items have been added
                initialCount = result.Count;

                //Iterate over the entire (current) result set. Use a for loop
                //using initalCount, because the collections gets modified
                for(int i = 0; i < initialCount; i++)
                {
                    //Find the current token at the index
                    item = result[i];

                    //If the current production is null, the start token is
                    //needed
                    if(item.Production.Equals(Production.Null))
                    {
                        //Only if the index is 0
                        if(item.Index == 0)
                        {
                            token = start;
                        }
                        else
                        {
                            token = null;
                        }
                    }
                    //If the index is still within the body of the production
                    else if(item.Index < item.Production.Body.Count)
                    {
                        token = item.Production.Body[item.Index];
                    }
                    //Else the end of the productions has been found
                    else
                    {
                        token = null;
                    }

                    //If no token has been found, move to the next result item
                    if(!token.HasValue)
                    {
                        continue;
                    }

                    //Find every unused production with token as the head and
                    //initalise a new kernel item at index 0 of that production
                    foreach(Production production
                            in this.CNFGrammar.Productions)
                    {
                        if(!used.Contains(production)
                            && production.Head.Equals(token.Value))
                        {
                            result.Add(new LR0KernelItem(production, 0));
                            used.Add(production);
                        }
                    }
                }
            } while(result.Count > initialCount);

            return result;
        }

        protected override LR0KernelItem CreateTransitionKernelItem
            (LR0KernelItem item)
            => new LR0KernelItem
            (
                item.Production,
                item.Index + 1
            );

    }
}