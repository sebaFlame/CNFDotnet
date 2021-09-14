using System;

using CNFDotnet.Analysis.Grammar;
using CNFDotnet.Analysis.Parsing.LR;

namespace CNFDotnet.Analysis.Parsing.LR.LR_0
{
    public class LR0KernelItem : BaseKernelItem
    {
        public LR0KernelItem (Production production, int index) 
            : base(production, index)
        {
        }

        public override bool Equals (BaseKernelItem other)
        {
            if(other is null)
            {
                return false;
            }

            return object.Equals(this.Production, other.Production)
                && this.Index == other.Index;
        }

        protected override int GetKernelItemHashCode ()
        {
            return HashCode.Combine(this.Production, this.Index);
        }
    }
}
