using System;
using System.Collections.Generic;

using CNFDotnet.Analysis.Grammar;

namespace CNFDotnet.Analysis.Parsing.LR
{
    public class LR1ActionItem<TKernelItem>
        where TKernelItem : BaseLR0KernelItem, IEquatable<TKernelItem>
    {
        /* During a shift (on a terminal) the input sentence can be shifted 1
         * token to the left and a transition to a new state can happen. If the
         * token is a non-terminal, a goto occurs without shifting the input
         * sentence */
        public State<TKernelItem> Shift { get; internal set; }

        /* A reductions reduces one ore more tokens (body of a production) to a
         * single non terminal (head of a production) */
        public List<Production> Reduce { get; internal set; }

        public LR1ActionItem()
        { }

        //Initialize a shift as action
        public LR1ActionItem(State<TKernelItem> shift)
            : this()
        {
            this.Shift = shift;
        }
    }
}
