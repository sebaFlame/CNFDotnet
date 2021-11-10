using System;

namespace CNFDotnet.Analysis.Grammar
{
    //A lexer implementation to parse a string
    public class StringLexer : BaseLexer
    {
        private readonly ReadOnlyMemory<char> _grammar;
        private int _position;

        public StringLexer(string grammar)
        {
            this._grammar = grammar.AsMemory();
            this._position = -1;
        }

        protected override bool GetNextChar(out char? ch)
        {
            if(++this._position < this._grammar.Length)
            {
                ch = this._grammar.Span[this._position];
                return true;
            }

            ch = null;
            return false;
        }

        protected override bool PreviousPosition()
        {
            if(this._position == 0)
            {
                return false;
            }

            this._position--;
            return true;
        }
    }
}
