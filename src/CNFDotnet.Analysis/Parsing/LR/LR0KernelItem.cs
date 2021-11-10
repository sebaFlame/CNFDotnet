using System;
using CNFDotnet.Analysis.Grammar;

namespace CNFDotnet.Analysis.Parsing.LR
{
    public class LR0KernelItem : BaseLR0KernelItem, IEquatable<LR0KernelItem>
    {
        public LR0KernelItem(Production production, int index)
            : base(production, index)
        { }

        public bool Equals(LR0KernelItem other)
            => base.Equals(other);

#nullable enable annotations
        public override bool Equals(object? obj)
            => base.Equals(obj);
#nullable restore annotations

        public override int GetHashCode()
            => base.GetHashCode();
    }
}
