using System.Text;

//Disable waring about return types not being assigned
#pragma warning disable IDE0058

namespace CNFDotnet.Analysis.Grammar
{
    public abstract class BaseLexer
    {
        protected abstract bool GetNextChar (out char? ch);
        protected abstract bool PreviousPosition ();

        private readonly StringBuilder _cache;

        protected BaseLexer () => this._cache = new StringBuilder();

        public Token Next ()
        {
            char ch;
            char? outCh;

            if (!this.GetNextChar(out outCh))
            {
                return new Token(TokenType.EOF);
            }
            else
            {
                ch = outCh.Value;
            }

            if (IsEndOfLine(ch))
            {
                return new Token(ch, TokenType.EOL);
            }
            else if (IsWhiteSpace(ch))
            {
                return new Token(ch, TokenType.WHITESPACE);
            }
            else if (StartsArrow(ch))
            {
                this._cache.Append(ch);

                if (!this.GetNextChar(out outCh))
                {
                    try
                    {
                        return new Token(this._cache.ToString(), TokenType.STRING);
                    }
                    finally
                    {
                        this._cache.Clear();
                        this.PreviousPosition();
                    }
                }
                else
                {
                    ch = outCh.Value;
                    if (CompletesArrow(ch))
                    {
                        this._cache.Append(ch);
                        try
                        {
                            return new Token(this._cache.ToString(), TokenType.ARROW);
                        }
                        finally
                        {
                            this._cache.Clear();
                        }
                    }
                    else
                    {
                        return this.GetStringToken(ch);
                    }
                }
            }
            else if (IsEmpty(ch))
            {
                return new Token(ch, TokenType.EMPTY);
            }
            else if (IsChoice(ch))
            {
                return new Token(ch, TokenType.CHOICE);
            }
            else
            {
                return this.GetStringToken(ch);
            }
        }

        private Token GetStringToken (char ch)
        {
            char? outCh;

            do
            {
                this._cache.Append(ch);

                if (!this.GetNextChar(out outCh))
                {
                    break;
                }
                else
                {
                    ch = outCh.Value;
                }

            } while (!(IsEndOfLine(ch)
                    || IsWhiteSpace(ch)
                    || IsChoice(ch)));
            try
            {
                return new Token(this._cache.ToString(), TokenType.STRING);
            }
            finally
            {
                this._cache.Clear();
                this.PreviousPosition();
            }
        }

        private static bool IsEndOfLine (char ch) => ch == '\n';
        private static bool IsWhiteSpace (char ch) => char.IsWhiteSpace(ch);
        private static bool IsChoice (char ch) => ch == '|';
        private static bool StartsArrow (char ch) => ch is '-' or '=';
        private static bool CompletesArrow (char ch) => ch == '>';
        private static bool IsEmpty (char ch) => ch == 'ε';
    }
}