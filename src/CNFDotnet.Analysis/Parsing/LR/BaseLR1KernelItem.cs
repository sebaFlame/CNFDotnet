using System;
using System.Collections.Generic;

using CNFDotnet.Analysis.Grammar;

namespace CNFDotnet.Analysis.Parsing.LR
{
    public abstract class BaseLR1KernelItem : BaseLR0KernelItem,
        IEquatable<BaseLR1KernelItem>
    {
        //The look-ahead(s) of this item
        public abstract IReadOnlySet<Token> LookAheads { get; }

        protected BaseLR1KernelItem
            (Production production, int index)
            : base(production, index)
        { }

        public abstract bool AddLookAhead(Token token);

        public bool Equals(BaseLR1KernelItem other)
            => other is not null
                && (object.ReferenceEquals(this, other)
                    || (object.Equals(this.Production, other.Production)
                        && this.Index == other.Index
                        && this.LookAheads.SetEquals(other.LookAheads)));

#nullable enable annotations
        public override bool Equals(object? obj)
        {
            if(obj is not BaseLR1KernelItem kernelItem)
            {
                return false;
            }

            return this.Equals(kernelItem);
        }
#nullable disable annotations

        public override int GetHashCode()
            => HashCode.Combine(this.Production, this.Index, this.LookAheads);
    }
}
