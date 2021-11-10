using System;
using System.Collections.Generic;

using CNFDotnet.Analysis.Grammar;

namespace CNFDotnet.Analysis.Parsing.LR.LALR1
{
    public class LALR1KernelItem
        : BaseLR1KernelItem, IEquatable<LALR1KernelItem>
    {
        public override IReadOnlySet<Token> LookAheads => this._lookAheads;

        private readonly HashSet<Token> _lookAheads;

        public LALR1KernelItem
        (
            Production production,
            int index
        )
            : base(production, index)
        {
            this._lookAheads = new HashSet<Token>();
        }

        public LALR1KernelItem
        (
            Production production,
            int index,
            IEnumerable<Token> lookAheads
        )
            : base(production, index)
        {
            if(lookAheads is HashSet<Token> hashSet)
            {
                this._lookAheads = new HashSet<Token>(hashSet);
            }
            else
            {
                this._lookAheads = new HashSet<Token>();
                foreach(Token lookAhead in lookAheads)
                {
                    this._lookAheads.Add(lookAhead);
                }
            }
        }

        public LALR1KernelItem
        (
            Production production,
            int index,
            HashSet<Token> lookaheads
        )
            : base(production, index)
        {
            this._lookAheads = lookaheads;
        }

        public LALR1KernelItem
        (
            BaseLR0KernelItem lr0KernelItem,
            HashSet<Token> lookAheads
        )
            : this(lr0KernelItem.Production, lr0KernelItem.Index, lookAheads)
        {
            this._lookAheads = lookAheads;
        }

        public LALR1KernelItem(BaseLR0KernelItem lr0KernelItem)
            : this(lr0KernelItem.Production, lr0KernelItem.Index)
        { }

        public LALR1KernelItem(LALR1KernelItem lalr1KernelItem)
            : this
            (
                lalr1KernelItem.Production,
                lalr1KernelItem.Index,
                new HashSet<Token>(lalr1KernelItem.LookAheads)
            )
        { }

        public override bool AddLookAhead(Token token)
            => this._lookAheads.Add(token);

        public bool Equals(LALR1KernelItem other)
            => base.Equals(other);

#nullable enable annotations
        public override bool Equals(object? obj)
            => base.Equals(obj);
#nullable restore annotations

        public override int GetHashCode()
          => HashCode.Combine(this.Production, this.Index, this.LookAheads);

    }
}
