using System;

using CNFDotnet.Analysis.Grammar;

namespace CNFDotnet.Analysis.Parsing.LR
{
    public abstract class BaseKernelItem : IEquatable<BaseKernelItem>
    {
        public Production Production { get; private set; }
        public int Index { get; private set; }

        protected BaseKernelItem (Production production, int index)
        {
            this.Production = production;
            this.Index = index;
        }

        public abstract bool Equals (BaseKernelItem other);

        public override bool Equals(object? obj)
        {
            if (!(obj is BaseKernelItem kernelItem))
            {
                return false;
            }

            return this.Equals(kernelItem);
        }

        protected abstract int GetKernelItemHashCode();

        public override int GetHashCode() 
        {
            return this.GetKernelItemHashCode();
        }
    }
}

