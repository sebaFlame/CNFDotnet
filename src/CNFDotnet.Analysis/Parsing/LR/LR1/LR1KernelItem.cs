using System;

using CNFDotnet.Analysis.Grammar;

namespace CNFDotnet.Analysis.Parsing.LR.LR1
{
    public class LR1KernelItem : BaseKernelItem
    {
        public Token LookAhead { get; private set; }

        public LR1KernelItem(Production production, int index, Token lookAhead)
            : base(production, index)
        {
            this.LookAhead = lookAhead;
        }
        public override bool Equals(BaseKernelItem other)
        {
            if(!(other is LR1KernelItem item))
            {
                return false;
            }

            return item.Index == this.Index
                && object.Equals(item.Production, this.Production)
                && item.LookAhead.Equals(this.LookAhead);
        }

        protected override int GetKernelItemHashCode()
          => HashCode.Combine(this.Production, this.Index, this.LookAhead);

    }
}
