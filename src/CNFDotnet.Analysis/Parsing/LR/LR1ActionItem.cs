using System.Collections.Generic;

using CNFDotnet.Analysis.Grammar;

namespace CNFDotnet.Analysis.Parsing.LR
{
    public class LR1ActionItem<TKernelItem>
        where TKernelItem : BaseKernelItem
    {
        public State<TKernelItem> Shift { get; internal set; }
        public List<Production> Reduce { get; internal set; }

        public LR1ActionItem()
        { }

        public LR1ActionItem(State<TKernelItem> shift)
            : this()
        {
            this.Shift = shift;
        }
    }
}
