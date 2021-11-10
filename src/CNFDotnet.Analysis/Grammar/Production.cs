/* Copyright 2021 sebaFlame
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to
 * deal in the Software without restriction, including without limitation the
 * rights to use, copy, modify, merge, publish, distribute, sublicense, and/or
 * sell copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
 * IN THE SOFTWARE.
 *
 * CNFDotnet is derived from grammophone by mdaines
 *  https://github.com/mdaines/grammophone
 * Which is based on Context Free Grammar Checker by Robin Cockett
 *  https://smlweb.cpsc.ucalgary.ca/
 *
 * Created by sebaFlame 2021/09/30 */

using System;
using System.Collections.Generic;
using System.Linq;

namespace CNFDotnet.Analysis.Grammar
{
    public class Production : IEquatable<Production>
    {
        //The head token which is always a non-terminal
        public Token Head => this._head;
        //A list of tokens the head non-terminal consists of
        public IReadOnlyList<Token> Body => this._body;
        //The index of this production for easier debugging & table generation
        public int Index { get; private set; }

        //A static "null" production so it can be used in certain contexts
        //(like dictionaries)
        internal static Production Null { get; }

        private readonly Token _head;
        private readonly IReadOnlyList<Token> _body;

        static Production()
        {
            Null = new Production
            (
                new Token(string.Empty, TokenType.STRING),
                -1
            );
        }

        private Production(Token? head, int index)
        {
            if(!head.HasValue)
            {
                throw new InvalidOperationException
                (
                    "No valid head token found"
                );
            }

            if(this._head.TokenType != TokenType.STRING)
            {
                throw new InvalidOperationException
                (
                    "Can only use string tokens as head token"
                );
            }

            this._head = head.Value;
            this.Index = index;
        }

        private Production(Token head, int index)
        {
            if(this._head.TokenType != TokenType.STRING)
            {
                throw new InvalidOperationException
                (
                    "Can only use string tokens as head token"
                );
            }

            this._head = head;
            this.Index = index;
        }

        public Production(Token head, IReadOnlyList<Token> body, int index)
            : this(head, index)
        {
            if(body.Any(x => x.TokenType != TokenType.STRING))
            {
                throw new InvalidOperationException
                (
                    "Can only use string tokens as rigt-handed tokens"
                );
            }

            this._body = body;
        }

        public Production(Token? head, IReadOnlyList<Token> body, int index)
            : this(head, index)
        {
            if(body.Any(x => x.TokenType != TokenType.STRING))
            {
                throw new InvalidOperationException
                (
                    "Can only use string tokens as rigt-handed tokens"
                );
            }

            this._body = body;
        }

        /* Create a single (!) production for a list of tokens
         * A production ends on an EOL, EOF or CHOICE (|)
         * It considers everything left from the arrow as head
         * non-terminal */
        public static Production CreateProduction
            (IEnumerable<Token> tokens, int index = 0)
        {
            List<Token> head = new List<Token>();
            List<Token> body = new List<Token>();
            List<Token> lrTokens = head;

            foreach(Token token in tokens)
            {
                if(token.TokenType == TokenType.STRING)
                {
                    lrTokens.Add(token);
                }
                //Do not add to body, if production end follows, 
                //body will be empty
                else if(token.TokenType == TokenType.EMPTY)
                {
                    continue;
                }
                //Basic validation of an arrow
                else if(token.TokenType == TokenType.ARROW)
                {
                    if(object.ReferenceEquals(lrTokens, body))
                    {
                        throw new InvalidOperationException("Syntax error");
                    }

                    lrTokens = body;
                }
                //Whitespace gets ignored
                else if(token.TokenType == TokenType.WHITESPACE)
                {
                    continue;
                }
                //End of the current production
                else if(token.TokenType is
                    TokenType.CHOICE
                    or TokenType.EOL
                    or TokenType.EOF)
                {
                    break;
                }
            }

            return new Production(head.Single(), body, index);
        }

        /* Create multiple productions from a list of tokens
         * A production ends on an EOL, EOF or CHOICE (|)
         * It considers everything left from the arrow as head
         * non-terminal.
         * The list of productions (grammar) ends on an EOF */
        public static IEnumerable<Production> CreateProductions
            (IEnumerable<Token> tokens)
        {
            List<Token> body = new List<Token>();
            List<Token> head = new List<Token>();
            int index = 0;

            List<Token> lrTokens = head;

            foreach(Token token in tokens)
            {
                if(token.TokenType == TokenType.STRING)
                {
                    lrTokens.Add(token);
                }
                //Do not add to body, if production end follows, 
                //body will be empty
                else if(token.TokenType == TokenType.EMPTY)
                {
                    continue;
                }
                //Whitespace gets ignored
                else if(token.TokenType == TokenType.WHITESPACE)
                {
                    continue;
                }
                //Basic validation of an arrow
                else if(token.TokenType == TokenType.ARROW)
                {
                    if(object.ReferenceEquals(lrTokens, body))
                    {
                        throw new InvalidOperationException("Syntax error");
                    }

                    lrTokens = body;
                }
                //End of current production has been reached, create a new
                //body list for the current head token(s)
                else if(token.TokenType == TokenType.CHOICE)
                {
                    yield return new Production(head.Single(), body, index++);
                    body = new List<Token>();
                    lrTokens = body;
                }
                //End of the current production, reset head and body tokens
                else if(token.TokenType == TokenType.EOL)
                {
                    yield return new Production(head.Single(), body, index++);

                    head.Clear();
                    body = new List<Token>();
                    lrTokens = head;
                }
                //End of the grammar
                else if(token.TokenType == TokenType.EOF)
                {
                    yield return new Production(head.Single(), body, index++);
                    break;
                }
            }
        }

        public bool Equals(Production other)
            => ReferenceEquals(other, this)
                || (this._head.Equals(other._head)
                    && this._body.SequenceEqual(other._body));

#nullable enable
        public override bool Equals(object? obj)
        {
            if(obj is not Production other)
            {
                return false;
            }

            return this.Equals(other);
        }
#nullable restore

        public override int GetHashCode()
            => HashCode.Combine(this._head, this._body);

        /* String representation of this production in Chomsky Normal Form
         * NT -> T NT */
        public override string ToString()
        {
            return string.Concat
            (
                this._head,
                " -> ",
                this._body.Count > 0
                    ? string.Join(" ", this._body)
                    : "ε"
            );
        }
    }
}
