using System;
using System.Collections;
using System.Collections.Generic;

using CNFDotnet.Analysis.Grammar;

namespace CNFDotnet.Analysis.Parsing.LR.LR1
{
    public class LR1KernelItem : BaseLR1KernelItem, IEquatable<LR1KernelItem>
    {
        public override IReadOnlySet<Token> LookAheads => this._singleTokenSet;

        private readonly SingleTokenSet _singleTokenSet;

        public LR1KernelItem(Production production, int index)
            : base(production, index)
        {
            this._singleTokenSet = new SingleTokenSet();
        }

        public LR1KernelItem(Production production, int index, Token lookAhead)
            : base(production, index)
        {
            this._singleTokenSet = new SingleTokenSet(lookAhead);
        }

        public LR1KernelItem
            (Production production, int index, IEnumerable<Token> lookAheads)
            : base(production, index)
        {
            if(lookAheads is SingleTokenSet singleTokenSet)
            {
                this._singleTokenSet = new SingleTokenSet(singleTokenSet.Item);
            }
            else
            {
                this._singleTokenSet = new SingleTokenSet();

                foreach(Token lookAhead in lookAheads)
                {
                    if(!this.AddLookAhead(lookAhead))
                    {
                        throw new InvalidOperationException
                            ("A look-ahead has already been assigned");
                    }
                }
            }
        }

        public override bool AddLookAhead(Token token)
        {
            if(!this._singleTokenSet.Add(token))
            {
                throw new InvalidOperationException
                    ("A look-ahead has already been assigned");
            }

            return true;
        }

        public bool Equals(LR1KernelItem other)
            => base.Equals(other);

#nullable enable annotations
        public override bool Equals(object? obj)
            => base.Equals(obj);
#nullable restore annotations

        public override int GetHashCode()
          => HashCode.Combine(this.Production, this.Index, this.LookAheads);
    }

    internal class SingleTokenSet : IReadOnlySet<Token>
    {
        public int Count => 1;

        internal Token Item { get; set; }

        public SingleTokenSet()
        {
            this.Item = Token.Null;
        }

        public SingleTokenSet(Token item)
        {
            this.Item = item;
        }

        public bool Add(Token item)
        {
            if(this.Item.Equals(Token.Null))
            {
                this.Item = item;
                return true;
            }

            return false;
        }

        public bool Contains(Token item) => this.Item.Equals(item);
        IEnumerator IEnumerable.GetEnumerator()
            => new SingleValueEnumerator(this.Item);

        public bool SetEquals(IEnumerable<Token> other)
        {
            if(other is SingleTokenSet singleItemSet)
            {
                return this.Item.Equals(singleItemSet.Item);
            }
            else if(other is HashSet<Token> hashSet)
            {
                return hashSet.SetEquals(this);
            }
            else
            {
                throw new NotSupportedException("This case is not supported");
            }
        }

        #region redundant methods
        public IEnumerator<Token> GetEnumerator()
            => new SingleValueEnumerator(this.Item);
        public bool IsProperSubsetOf(IEnumerable<Token> other)
            => throw new NotImplementedException();
        public bool IsProperSupersetOf(IEnumerable<Token> other)
            => throw new NotImplementedException();
        public bool IsSubsetOf(IEnumerable<Token> other)
            => throw new NotImplementedException();
        public bool IsSupersetOf(IEnumerable<Token> other)
            => throw new NotImplementedException();
        public bool Overlaps(IEnumerable<Token> other)
            => throw new NotImplementedException();
        #endregion

        private struct SingleValueEnumerator : IEnumerator<Token>, IEnumerator
        {
            public Token Current => this._current;
            object IEnumerator.Current => this._current;

            private Token _current;
            private readonly Token _item;

            public SingleValueEnumerator(Token item)
            {
                this._item = item;
                this._current = default;
            }

            public bool MoveNext()
            {
                if(this._current.Equals(Token.Null))
                {
                    this._current = this._item;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public void Dispose() => this._current = default;
            public void Reset() => this._current = default;
        }
    }
}
