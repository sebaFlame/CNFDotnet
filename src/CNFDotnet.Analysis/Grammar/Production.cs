using System;
using System.Collections.Generic;
using System.Linq;

namespace CNFDotnet.Analysis.Grammar
{
    public class Production : IEquatable<Production>
    {
        public Token Left => this._left;
        public IList<Token> Right => this._right;

        internal CNFGrammar Grammar { get; set; }

        private Token _left;
        private IList<Token> _right;

        private Production (Token? left)
        {
            if (!left.HasValue)
            {
                throw new InvalidOperationException("No valid left token found");
            }

            if (this._left.TokenType != TokenType.STRING)
            {
                throw new InvalidOperationException("Can only use string tokens as left-hand token");
            }

            this._left = left.Value;
        }

        private Production (Token left)
        {
            if(this._left.TokenType != TokenType.STRING)
            {
                throw new InvalidOperationException("Can only use string tokens as left-hand token");
            }

            this._left = left;
        }

        public Production(Token left, params Token[] right)
            : this(left, right as IList<Token>)
        { }

        public Production(Token left, IList<Token> right)
            : this(left)
        {
            if(right.Any(x => x.TokenType != TokenType.STRING))
            {
                throw new InvalidOperationException("Can only use string tokens as rigt-handed tokens");
            }

            this._right = right;
        }

        public Production(Token? left, IList<Token> right)
            :this(left)
        {
            if(right.Any(x => x.TokenType != TokenType.STRING))
            {
                throw new InvalidOperationException("Can only use string tokens as rigt-handed tokens");
            }

            this._right = right;
        }

        public static Production CreateProduction(IEnumerable<Token> tokens)
        {
            List<Token> left = new List<Token>();
            List<Token> right = new List<Token>();
            List<Token> lrTokens = left;

            foreach(Token token in lrTokens)
            {
                if(token.TokenType == TokenType.STRING)
                {
                    lrTokens.Add(token);
                }
                else if(token.TokenType == TokenType.EMPTY)
                {
                    continue;
                }
                else if(token.TokenType == TokenType.ARROW)
                {
                    if(object.ReferenceEquals(lrTokens, right))
                    {
                        throw new InvalidOperationException("Syntax error");
                    }

                    lrTokens = right;
                }
                else if(token.TokenType == TokenType.WHITESPACE)
                {
                    continue;
                }
                else if(token.TokenType == TokenType.CHOICE
                    || token.TokenType == TokenType.EOL
                    || token.TokenType == TokenType.EOF)
                {
                    break;
                }
            }

            return new Production(left.Single(), right);
        }

        public static IEnumerable<Production> CreateProductions(IEnumerable<Token> tokens)
        {
            List<Token> right = new List<Token>();
            List<Token> left = new List<Token>();

            List<Token> lrTokens = left;

            foreach(Token token in tokens)
            {
                if(token.TokenType == TokenType.STRING)
                {
                    lrTokens.Add(token);
                }
                else if(token.TokenType == TokenType.EMPTY)
                {
                    continue;
                }
                else if(token.TokenType == TokenType.WHITESPACE)
                {
                    continue;
                }
                else if(token.TokenType == TokenType.ARROW)
                {
                    if(object.ReferenceEquals(lrTokens, right))
                    {
                        throw new InvalidOperationException("Syntax error");
                    }

                    lrTokens = right;
                }
                else if(token.TokenType == TokenType.CHOICE)
                {
                    yield return new Production(left.Single(), right);
                    right = new List<Token>();
                    lrTokens = right;
                }
                else if(token.TokenType == TokenType.EOL)
                {
                    yield return new Production(left.Single(), right);

                    left.Clear();
                    right = new List<Token>();
                    lrTokens = left;
                }
                else if(token.TokenType == TokenType.EOF)
                {
                    yield return new Production(left.Single(), right);
                    break;
                }
            }
        }

        public bool Equals(Production other)
        {
            return this._left.Equals(other._left)
                && this._right.SequenceEqual(other._right);
        }

        public override bool Equals(object? obj)
        {
            if(obj is not Production other)
            {
                return false;
            }

            return this.Equals(other);
        }

        public override int GetHashCode()
            => HashCode.Combine(this._left, this._right);

        public override string ToString()
        {
            return string.Concat
            (
                this._left,
                " -> ",
                this._right.Count > 0 
                    ? string.Join(" ", this._right)
                    : "ε"
            );
        }
    }
}
