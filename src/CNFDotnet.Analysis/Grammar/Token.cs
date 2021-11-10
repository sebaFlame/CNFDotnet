using System;

namespace CNFDotnet.Analysis.Grammar
{
    //Represents a token found in a CNF Grammar consisting of a value and a
    //token type
    public struct Token : IEquatable<Token>
    {
        public string Value => this._value;
        public TokenType TokenType => this._tokenType;

        internal static Token Null => default;

        private readonly string _value;
        private readonly TokenType _tokenType;

        public Token(string value, TokenType tokenType)
        {
            this._value = value;
            this._tokenType = tokenType;
        }

        public Token(char ch, TokenType tokenType)
        {
            this._value = ch.ToString();
            this._tokenType = tokenType;
        }

        public Token(TokenType tokenType)
        {
            this._value = string.Empty;
            this._tokenType = tokenType;
        }

#pragma warning disable CA1309
        //_value can be null
        public bool Equals(Token other)
            => this._tokenType == other._tokenType
                && string.Equals(this._value, other._value);
#pragma warning restore CA1309

        public override int GetHashCode()
            => this._value.GetHashCode()
                + (278901 * (int)this._tokenType);

#nullable enable annotations
        public override bool Equals(object? obj)
        {
            if(obj is not Token token)
            {
                return false;
            }

            return this.Equals(token);
        }
#nullable restore annotations

        public static bool operator ==(in Token left, in Token right)
            => left.Equals(right);

        public static bool operator !=(in Token left, in Token right)
            => !left.Equals(right);

        public static implicit operator Token(string str)
        {
            if(string.Equals(str, "$", StringComparison.Ordinal))
            {
                return new Token(TokenType.EOF);
            }
            else
            {
                return new Token(str, TokenType.STRING);
            }
        }

        public static implicit operator string(Token token)
            => token.ToString();

        public override string ToString()
        {
            if(this._tokenType == TokenType.EOF)
            {
                return "$";
            }

            return this._value;
        }
    }
}
