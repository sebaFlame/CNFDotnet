using System;
using System.Collections.Generic;
using System.Linq;

using CNFDotnet.Analysis.Grammar;

namespace CNFDotnet.Analysis.Parsing.LR.LALR1
{
    public class LALR1KernelItem : BaseKernelItem
    {
        public IList<Token> LookAheads { get; private set; }

        public LALR1KernelItem(Production production, int index, IList<Token> lookaheads)
            : base(production, index)
        {
            this.LookAheads = lookaheads;
        }

        public LALR1KernelItem(Production production, int index)
            : this(production, index, new List<Token>())
        { }

        public override bool Equals(BaseKernelItem other)
        {
            if(!(other is LALR1KernelItem item))
            {
                return false;
            }

            //Use LR0 comparison
            return item.Index == this.Index
                && object.Equals(item.Production, this.Production);
        }

        protected override int GetKernelItemHashCode()
          => HashCode.Combine(this.Production, this.Index, this.LookAheads);

    }
}
