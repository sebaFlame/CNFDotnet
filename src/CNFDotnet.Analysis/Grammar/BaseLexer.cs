using System.Text;

namespace CNFDotnet.Analysis.Grammar
{
    // Grammar lexer base class
    public abstract class BaseLexer
    {
        // Get the next char
        protected abstract bool GetNextChar(out char? ch);
        // Return to the previous position
        protected abstract bool PreviousPosition();

        private readonly StringBuilder _cache;

        protected BaseLexer()
        {
            this._cache = new StringBuilder();
        }

        public Token Next()
        {
            char ch;
            char? outCh;

            // If no new character is found return an EOF
            if(!this.GetNextChar(out outCh))
            {
                return new Token(TokenType.EOF);
            }
            else
            {
                ch = outCh.Value;
            }

            if(BaseLexer.IsEndOfLine(ch))
            {
                return new Token(ch, TokenType.EOL);
            }
            else if(BaseLexer.IsWhiteSpace(ch))
            {
                return new Token(ch, TokenType.WHITESPACE);
            }
            // Check if the current character might be the start of an arrow
            // Currently "-" & "=" are supported
            else if(BaseLexer.StartsArrow(ch))
            {
                this._cache.Append(ch);

                // If no next character is found, return as a string type
                if(!this.GetNextChar(out outCh))
                {
                    try
                    {
                        return new Token
                        (
                            this._cache.ToString(),
                            TokenType.STRING
                        );
                    }
                    finally
                    {
                        this._cache.Clear();
                        this.PreviousPosition();
                    }
                }
                // If a new character is found
                else
                {
                    ch = outCh.Value;
                    // Check if it can complete an arrow
                    // Currently only ">" is supported
                    if(BaseLexer.CompletesArrow(ch))
                    {
                        this._cache.Append(ch);
                        try
                        {
                            return new Token
                            (
                                this._cache.ToString(),
                                TokenType.ARROW
                            );
                        }
                        finally
                        {
                            this._cache.Clear();
                        }
                    }
                    // Else return a new STRING token
                    else
                    {
                        return this.GetStringToken(ch);
                    }
                }
            }
            else if(BaseLexer.IsEmpty(ch))
            {
                return new Token(ch, TokenType.EMPTY);
            }
            else if(BaseLexer.IsChoice(ch))
            {
                return new Token(ch, TokenType.CHOICE);
            }
            else
            {
                return this.GetStringToken(ch);
            }
        }

        // Create a STRING token from the current cache, the character passed
        // and any non EOF, EOL, CHOICE that follows
        private Token GetStringToken(char ch)
        {
            char? outCh;

            do
            {
                this._cache.Append(ch);

                if(!this.GetNextChar(out outCh))
                {
                    break;
                }
                else
                {
                    ch = outCh.Value;
                }

            } while(!(BaseLexer.IsEndOfLine(ch)
                    || BaseLexer.IsWhiteSpace(ch)
                    || BaseLexer.IsChoice(ch)));
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

        private static bool IsEndOfLine(char ch) => ch == '\n';
        private static bool IsWhiteSpace(char ch) => char.IsWhiteSpace(ch);
        private static bool IsChoice(char ch) => ch == '|';
        private static bool StartsArrow(char ch) => ch is '-' or '=';
        private static bool CompletesArrow(char ch) => ch == '>';
        private static bool IsEmpty(char ch) => ch == 'ε';
    }
}