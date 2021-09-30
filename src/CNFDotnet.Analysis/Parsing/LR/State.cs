using System;
using System.Collections.Generic;

using CNFDotnet.Analysis.Grammar;

namespace CNFDotnet.Analysis.Parsing.LR
{
    public class State<TKernelItem>
        where TKernelItem : BaseKernelItem
    {
        public Kernel<TKernelItem> Kernel { get; internal set; }
        public Kernel<TKernelItem> Items { get; internal set; }
        public IDictionary<Token, State<TKernelItem>> Transitions { get; internal set; }
        public int Index { get; internal set; }

        internal State(int index)
        {
            this.Index = index;
        }

        public State (Kernel<TKernelItem> kernel, int index)
        {
            this.Kernel = kernel;
            this.Index = index;
        }

        public State 
        (
            int index, 
            Kernel<TKernelItem> kernel,
            Kernel<TKernelItem> items,
            IDictionary<Token, State<TKernelItem>> transitions
        )
        {
            this.Index = index;
            this.Kernel = kernel;
            this.Items = items;
            this.Transitions = transitions;
        }

        public State 
        (
            int index, 
            Kernel<TKernelItem> kernel,
            Kernel<TKernelItem> items
        )
        {
            this.Index = index;
            this.Kernel = kernel;
            this.Items = items;
        }

        public override int GetHashCode()
            => this.Index;
    }
}
