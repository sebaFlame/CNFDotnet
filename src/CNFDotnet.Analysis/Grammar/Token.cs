using System;

namespace CNFDotnet.Analysis.Grammar
{
    public struct Token : IEquatable<Token>
    {
        public string Value => this._value;
        public TokenType TokenType => this._tokenType;

        private string _value;
        private TokenType _tokenType;

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

        public bool Equals(Token other)
            => this._tokenType == other._tokenType
                && string.Equals(this._value, other._value);

        public override int GetHashCode()
            => this._value.GetHashCode()
                + (278901 * (int)this._tokenType);

        public override bool Equals(object? obj)
        {
            if(obj is not Token token)
            {
                return false;
            }

            return this.Equals(token);
        }

        public static bool operator ==(in Token left, in Token right)
            => left.Equals(right);

        public static bool operator !=(in Token left, in Token right)
            => !left.Equals(right);

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
