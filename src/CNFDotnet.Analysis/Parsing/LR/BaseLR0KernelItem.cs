using System;

using CNFDotnet.Analysis.Grammar;

namespace CNFDotnet.Analysis.Parsing.LR
{
    /* Represents a production and the position in that production. These are
     * uniquely part of a kernel as single state in an automaton */
    public abstract class BaseLR0KernelItem : IEquatable<BaseLR0KernelItem>
    {
        //A production
        public Production Production { get; private set; }
        //An index (position) in that index
        public int Index { get; private set; }

        protected BaseLR0KernelItem(Production production, int index)
        {
            this.Production = production;
            this.Index = index;
        }

        public bool Equals(BaseLR0KernelItem other)
            => other is not null
            && (object.ReferenceEquals(this, other)
                || (object.Equals(this.Production, other.Production)
                    && this.Index == other.Index));

#nullable enable annotations
        public override bool Equals(object? obj)
        {
            if(obj is not BaseLR0KernelItem kernelItem)
            {
                return false;
            }

            return this.Equals(kernelItem);
        }
#nullable disable annotations

        public override int GetHashCode()
            => HashCode.Combine(this.Production, this.Index);
    }
}

