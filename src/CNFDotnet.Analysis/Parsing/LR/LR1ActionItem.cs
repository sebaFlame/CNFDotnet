using System.Collections.Generic;

using CNFDotnet.Analysis.Grammar;

namespace CNFDotnet.Analysis.Parsing.LR
{
    public class LR1ActionItem
    {
        public State Shift { get; internal set; }
        public List<Production> Reduce { get; internal set; }

        public LR1ActionItem()
        { }

        public LR1ActionItem(State shift)
            : this()
        {
            this.Shift = shift;
        }
    }
}
