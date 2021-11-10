using System;
using System.Collections.Generic;
using System.Linq;

using CNFDotnet.Analysis.Grammar;

namespace CNFDotnet.Analysis.Parsing.LR
{
    public abstract class BaseLR1Parsing<TAction, TKernelItem>
        : BaseLRParsing<TAction, TKernelItem>
        where TAction : BaseLR1ActionDictionary<TKernelItem>
        where TKernelItem : BaseLR1KernelItem, IEquatable<TKernelItem>
    {
        public BaseLR1Parsing(CNFGrammar cnfGrammar)
            : base(cnfGrammar)
        { }

        protected TKernelItem CreateKernelItem
            (
                Production production,
                int index,
                params Token[] lookAheads
            )
            => this.CreateKernelItem
            (
                production,
                index,
                (IEnumerable<Token>)lookAheads
            );

        protected abstract TKernelItem CreateKernelItem
            (
                Production production,
                int index,
                IEnumerable<Token> lookAheads
            );

        protected override Kernel<TKernelItem> CreateInitialKernel()
            => new Kernel<TKernelItem>
            {
                this.CreateKernelItem
                (
                    Production.Null,
                    0,
                    new Token(TokenType.EOF)
                )
            };

        protected override Kernel<TKernelItem> CreateClosure
            (Kernel<TKernelItem> kernel)
        {
            Token start = this.CNFGrammar.ComputeStartNonTerminal();
            IReadOnlySet<Token> nonTerminals =
                this.CNFGrammar.ComputeNonTerminals();
            Dictionary<Production, HashSet<Token>> used
                = new Dictionary<Production, HashSet<Token>>();
            Kernel<TKernelItem> result;
            //These are arrays because they get used multiple times
            Token[] remaining, lookaheads;
            Token token;
            int initialCount;
            TKernelItem item;

            //Ensure every token can only be used once as look-ahead 
            foreach(Production production in this.CNFGrammar.Productions)
            {
                used.Add(production, new HashSet<Token>());
            }

            result = new Kernel<TKernelItem>();

            //The closure exists of the existing items
            foreach(TKernelItem lr1KernelItem in kernel)
            {
                //TODO: reuse existing kernel item
                result.Add
                (
                    this.CreateKernelItem
                    (
                        lr1KernelItem.Production,
                        lr1KernelItem.Index,
                        lr1KernelItem.LookAheads
                    )
                );
            }

            //Keep returning items while relations are being found
            do
            {
                //Save the current count to check if new items have been added
                initialCount = result.Count;

                //Iterate over the entire (current) result set. The collection
                //gets modified (appended)!
                for(int i = 0; i < initialCount; i++)
                {
                    item = result[i];

                    //Find the current remaining tokens from the current index

                    //If it's the null "production"
                    if(item.Production.Equals(Production.Null))
                    {
                        //And the index is 0
                        if(item.Index == 0)
                        {
                            //Only the start symbol remains
                            remaining = new Token[] { start };
                        }
                        else
                        {
                            //Else the end has been reached
                            remaining = Array.Empty<Token>();
                        }
                    }
                    //Grab the remaining tokens
                    else
                    {
                        remaining = item.Production.Body
                            .Skip(item.Index)
                            .ToArray();
                    }

                    if(remaining.Length == 0)
                    {
                        continue;
                    }

                    //Get the first token of the remaining body
                    token = remaining[0];

                    //Ensure this is a non-terminal, so a look-ahead can occur
                    if(!nonTerminals.Contains(token))
                    {
                        continue;
                    }

                    /* Compute the first set of all tokens that follow the first
                     * token of the remaining set and the look-ahead of the
                     * current item.
                     * If the second item of remaining is a terminal, return it.
                     * If the second item of remaining is a non-terminal, return
                     * the FIRST of this non-terminal. If this non-terminal is
                     * nullable, continue with the rest of the remaining set. */
                    lookaheads = this.CNFGrammar.GetFirst
                        (
                            remaining
                                .Skip(1)
                                .Concat
                                (
                                    item.LookAheads
                                )
                        )
                        .ToArray();

                    //Find all productions for the current first token of the
                    //remaining set
                    foreach(Production production
                            in this.CNFGrammar.Productions)
                    {
                        if(!production.Head.Equals(token))
                        {
                            continue;
                        }

                        //And add the look-aheads found from the remaining
                        //token(s) as look-aheads for that production (if that
                        //look-ahead has not been used for that production
                        //before)
                        foreach(Token l in lookaheads)
                        {
                            if(!used[production].Contains(l))
                            {
                                result.Add
                                (
                                    this.CreateKernelItem
                                    (
                                        production,
                                        0,
                                        l
                                    )
                                );
                                used[production].Add(l);
                            }
                        }
                    }
                }
            } while(result.Count > initialCount);

            return result;
        }

        //Move position 1 forward in the current production/look-ahead
        protected override TKernelItem CreateTransitionKernelItem
            (TKernelItem item)
            => this.CreateKernelItem
            (
                item.Production,
                item.Index + 1,
                item.LookAheads
            );

        public override void Classify()
            => this.ClassifyLR1(this.CreateParsingTable());
    }
}
