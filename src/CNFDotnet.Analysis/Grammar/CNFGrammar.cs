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

namespace CNFDotnet.Analysis.Grammar
{
    public class CNFGrammar
    {
        /* A list of all the productions
         * A production is a sentence consisting of a left-handed (head)
         * non-terminal described by 0 or more terminals or
         * non-terminals on the right side (body) */
        public IReadOnlyList<Production> Productions => this._productions;

        private readonly List<Production> _productions;
        private readonly HashSet<Token> _tokens;
        /* A list of all terminal tokens
        * A terminal is the smallest common denominator,
        * it can not be split into other non-terminals or terminals */
        private IReadOnlySet<Token> _terminals;
        /*  A list of all non-terminal tokens
        *  A non-terminal consists of 0 or more terminals
        *  or other non-terminals */
        private IReadOnlySet<Token> _nonTerminals;
        //The non-terminal considered to be the start of the grammar
        private Token? _start;
        //A list of tokens which can not be reached after parsing the grammar
        private IReadOnlySet<Token> _unreachable;
        //A list of non-terminals which can not be produced
        private IReadOnlySet<Token> _unrealizable;
        //A list of nullable non-terminals
        private IReadOnlySet<Token> _nullable;
        //Each production has a set of terminals as a first token
        private IDictionary<Token, HashSet<Token>> _firstSet;
        //Each production has a set of terminals following the first set
        private IDictionary<Token, HashSet<Token>> _followSet;

        private CNFGrammar()
        {
            this._productions = new List<Production>();
            this._tokens = new HashSet<Token>();
        }

        //Create a grammar from a list of tokens
        public CNFGrammar(IEnumerable<Token> tokens)
            : this()
        {
            this.AddProductions(Production.CreateProductions(tokens));
        }

        //Add a production to this grammar
        public void AddProduction(Production production)
        {
            this._productions.Add(production);

            this._tokens.Add(production.Head);
            foreach(Token token in production.Body)
            {
                this._tokens.Add(token);
            }
        }

        //Add productions to this grammar
        public void AddProductions(IEnumerable<Production> productions)
        {
            foreach(Production production in productions)
            {
                this.AddProduction(production);
            }
        }

        //Compute the start token from the first added production
        public Token ComputeStartNonTerminal()
        {
            if(this._start is null)
            {
                this._start = this._productions[0].Head;
            }

            return this._start.Value;
        }

        //Get all (single) head tokens from the productions to compute
        //all non-terminals
        public IReadOnlySet<Token> ComputeNonTerminals()
        {
            if(this._nonTerminals is not null)
            {
                return this._nonTerminals;
            }

            HashSet<Token> nonTerminals = new HashSet<Token>();

            foreach(Production production in this._productions)
            {
                if(production.Head.TokenType != TokenType.EMPTY)
                {
                    nonTerminals.Add(production.Head);
                }
            }

            return this._nonTerminals = nonTerminals;
        }

        //Every non non-terminal token is considered a terminal
        public IReadOnlySet<Token> ComputeTerminals()
        {
            if(this._terminals is not null)
            {
                return this._terminals;
            }

            HashSet<Token> terminals = new HashSet<Token>();
            IReadOnlySet<Token> nonTerminals = this.ComputeNonTerminals();

            foreach(Token token in this._tokens)
            {
                if(!nonTerminals.Contains(token)
                    && token.TokenType != TokenType.EMPTY)
                {
                    terminals.Add(token);
                }
            }

            return this._terminals = terminals;
        }

        //Compute the non-terminals which can not be reached from the start
        //token
        public IReadOnlySet<Token> ComputeUnreachable()
        {
            if(this._unreachable is not null)
            {
                return this._unreachable;
            }

            HashSet<Token> unreachable = new HashSet<Token>();
            IReadOnlySet<Token> nonTerminals = this.ComputeNonTerminals();

            Relation relation = new Relation();
            Token start = this.ComputeStartNonTerminal();

            //For each production every body token is in a relation
            //with the head token
            foreach(Production production in this._productions)
            {
                foreach(Token token in production.Body)
                {
                    relation.AddRelation(production.Head, token);
                }
            }

            //Compute all relations by computing the closure, of all immediate
            //production relationships
            Dictionary<Token, HashSet<Token>> closure
                = Relation.Closure(relation);

            //If a non-terminal has no relation to start, it is considered
            //unreachable
            foreach(Token token in nonTerminals)
            {
                if(token == start)
                {
                    continue;
                }

                if(!closure[start].Contains(token))
                {
                    unreachable.Add(token);
                }
            }

            return this._unreachable = unreachable;
        }

        //Compute the non-terminals which can not be realized, by means of other
        //tokens. This can consider recursion as unrealizable!
        public IReadOnlySet<Token> ComputeUnrealizable()
        {
            if(this._unrealizable is not null)
            {
                return this._unrealizable;
            }

            HashSet<Token> unrealizable = new HashSet<Token>();
            List<Production> productions = this._productions;
            IReadOnlySet<Token> nonTerminals = this.ComputeNonTerminals();

            List<Token> added = new List<Token>();
            HashSet<Token> marked = new HashSet<Token>();
            int currentRightPosition;

            //Check every body non-terminal for an existing production
            do
            {
                added.Clear();

                foreach(Production production in productions)
                {
                    //Break at the first unmarked body non-terminal
                    for(currentRightPosition = 0;
                        currentRightPosition < production.Body.Count;
                        currentRightPosition++)
                    {
                        if(!marked.Contains
                            (
                                production.Body[currentRightPosition]
                            )
                            && nonTerminals.Contains
                                (
                                    production.Body[currentRightPosition])
                                 )
                        {
                            break;
                        }
                    }

                    //If the head of this production is unmarked & all of
                    //the tokens in the body are marked non-terminals
                    //or terminals, mark the head and continue the loop with the
                    //newly added marked head non-terminal
                    if(!marked.Contains(production.Head)
                        && currentRightPosition == production.Body.Count)
                    {
                        marked.Add(production.Head);
                        added.Add(production.Head);
                    }
                }
            } while(added.Count > 0);

            //Every unmarked non-terminal is unrealizable
            foreach(Token nonTerminal in nonTerminals)
            {
                if(!marked.Contains(nonTerminal))
                {
                    unrealizable.Add(nonTerminal);
                }
            }

            return this._unrealizable = unrealizable;
        }

        //Return all productions with a head that also resolves to ε. This is
        //considered an ambiguity because a non-terminal can only resolve to ε
        //OR a series of (non-)terminals, not both.
        public IEnumerable<Production> ComputeNullAmbiguity()
        {
            IReadOnlySet<Token> nonTerminals = this.ComputeNonTerminals();
            IList<Production> productions = this._productions;
            IReadOnlySet<Token> nullable = this.ComputeNullable();
            int j;
            Production found;

            //For each non-terminal
            foreach(Token nonTerminal in nonTerminals)
            {
                found = null;

                //Look through the productions of this non-terminal
                foreach(Production production in productions)
                {
                    if(!production.Head.Equals(nonTerminal))
                    {
                        continue;
                    }

                    //An empty production is nullable
                    if(production.Body.Count == 0)
                    {
                        if(found is not null)
                        {
                            yield return production;
                        }
                        else
                        {
                            found = production;
                        }

                        continue;
                    }

                    //A production is nullable if all of its symbols are
                    //nullable
                    for(j = 0; j < production.Body.Count; j++)
                    {
                        if(!nullable.Contains(production.Body[j]))
                        {
                            break;
                        }
                    }

                    if(j == production.Body.Count)
                    {
                        if(found is not null)
                        {
                            yield return production;
                        }
                        else
                        {
                            found = production;
                        }
                    }
                }
            }
        }

        //Compute all nullable non-terminals
        public IReadOnlySet<Token> ComputeNullable()
        {
            if(this._nullable is not null)
            {
                return this._nullable;
            }

            HashSet<Token> nullable = new HashSet<Token>();
            IList<Production> productions = this._productions;
            List<Token> added = new List<Token>();
            int currentRightPosition;

            do
            {
                added.Clear();

                foreach(Production production in productions)
                {
                    //Break at the first unmarked body token
                    for(currentRightPosition = 0;
                        currentRightPosition < production.Body.Count;
                        currentRightPosition++)
                    {
                        if(!nullable.Contains
                            (
                                production.Body[currentRightPosition])
                            )
                        {
                            break;
                        }
                    }

                    //If all body tokens were nullable, the head
                    //is nullable, and continue the loop to verify with
                    //the newly added nullable
                    if(!nullable.Contains(production.Head)
                        && currentRightPosition == production.Body.Count)
                    {
                        nullable.Add(production.Head);
                        added.Add(production.Head);
                    }
                }
            } while(added.Count > 0);

            return this._nullable = nullable;
        }

        //Return the first cycle if there is one
        public IEnumerable<Token> ComputeFirstCycle()
        {
            IReadOnlySet<Token> nonTerminals = this.ComputeNonTerminals();
            IReadOnlySet<Token> nullable = this.ComputeNullable();
            Relation relation = new Relation();
            int j, k;

            foreach(Production production in this._productions)
            {
                for(j = 0; j < production.Body.Count; j++)
                {
                    //If the token is a terminal, skip it
                    if(!nonTerminals.Contains(production.Body[j]))
                    {
                        continue;
                    }

                    //Only add the relation between the head and j
                    //if all other tokens are terminals or nullable
                    for(k = 0; k < production.Body.Count; k++)
                    {
                        if(j == k)
                        {
                            continue;
                        }

                        if(!nonTerminals.Contains(production.Body[k]))
                        {
                            break;
                        }

                        if(!nullable.Contains(production.Body[k]))
                        {
                            break;
                        }
                    }

                    if(k == production.Body.Count)
                    {
                        relation.AddRelation
                        (
                            production.Head,
                            production.Body[j]
                        );
                    }
                }
            }

            return Relation.Cycle(relation);
        }

        //Compute the first body terminal for all non-terminals
        public IDictionary<Token, HashSet<Token>> ComputeFirstSet()
        {
            if(this._firstSet is not null)
            {
                return this._firstSet;
            }

            IReadOnlySet<Token> nonTerminals = this.ComputeNonTerminals();
            IList<Production> productions = this._productions;
            IReadOnlySet<Token> nullable = this.ComputeNullable();

            Relation immediate, propagation;
            int j;

            immediate = new Relation();

            //For each token in the body of a production, add the
            //first non-terminal after a sequence of nullable tokens
            foreach(Production production in productions)
            {
                for(j = 0; j < production.Body.Count; j++)
                {
                    if(!nullable.Contains(production.Body[j]))
                    {
                        break;
                    }
                }

                //If the first non-nullable symbol is a terminal, add it to the
                //immediate set of the head
                if(j < production.Body.Count
                    && !nonTerminals.Contains(production.Body[j]))
                {
                    immediate.AddRelation
                    (
                        production.Head,
                        production.Body[j]
                    );
                }
            }

            propagation = new Relation();

            //Add all nullable non-terminals in the body and the
            //next non-terminal (if any)
            foreach(Production production in productions)
            {
                foreach(Token token in production.Body)
                {
                    if(nonTerminals.Contains(token))
                    {
                        propagation.AddRelation(production.Head, token);
                    }

                    if(!nullable.Contains(token))
                    {
                        break;
                    }
                }
            }

            //Propagate the relations
            return this._firstSet
                    = Relation.Propagate(immediate, propagation);
        }

        //Compute all tokens which can follow a particular token
        public IDictionary<Token, HashSet<Token>> ComputeFollowSet()
        {
            if(this._followSet is not null)
            {
                return this._followSet;
            }

            IReadOnlySet<Token> nonTerminals = this.ComputeNonTerminals();
            IList<Production> productions = this._productions;
            IReadOnlySet<Token> nullable = this.ComputeNullable();
            IDictionary<Token, HashSet<Token>> firstSet
                = this.ComputeFirstSet();

            Relation immediate, propagation;

            immediate = new Relation();
            propagation = new Relation();

            //EOF is always in the followset of Start
            immediate.AddRelation
            (
                this.ComputeStartNonTerminal(),
                new Token(TokenType.EOF)
            );

            /* Add the first set of every non-terminal k following every
             * non-terminal j to the immediate follow set of j until the first
             * terminal has been reached or until the non-terminal k is not 
             * nullable
             *
             * Given a production X -> ... A β, follow(A) includes first(β),
             * except for the empty string. */
            foreach(Production production in productions)
            {
                for(int j = 0; j < production.Body.Count - 1; j++)
                {
                    //Skip terminals
                    if(!nonTerminals.Contains(production.Body[j]))
                    {
                        continue;
                    }

                    //Add the first set of the tokens following token j
                    for(int k = j + 1; k < production.Body.Count; k++)
                    {
                        //If the symbol is a terminal, add it and stop adding
                        if(!nonTerminals.Contains(production.Body[k]))
                        {
                            immediate.AddRelation
                            (
                                production.Body[j],
                                production.Body[k]
                            );

                            break;
                        }

                        //If the token is a non-terminal, add each token in the
                        //first set of that non-terminal
                        foreach(Token s in firstSet[production.Body[k]])
                        {
                            immediate.AddRelation(production.Body[j], s);
                        }

                        //Stop adding if the non-terminal is not nullable
                        if(!nullable.Contains(production.Body[k]))
                        {
                            break;
                        }
                    }
                }
            }

            //Given a production B -> ... A β where β is nullable, follow(A)
            //includes follow(B)
            foreach(Production production in productions)
            {
                //Scan from the end of the body of the production to the
                //beginning...
                for(int j = production.Body.Count - 1; j >= 0; j--)
                {
                    //If the symbol is a non-terminal, add the head as a
                    //relation of that non-terminal
                    if(nonTerminals.Contains(production.Body[j]))
                    {
                        propagation.AddRelation
                        (
                            production.Body[j],
                            production.Head
                        );
                    }

                    //Stop adding if the token is not nullable
                    if(!nullable.Contains(production.Body[j]))
                    {
                        break;
                    }
                }
            }

            return this._followSet
                    = Relation.Propagate(immediate, propagation);
        }

        //Compute the first set from a range of tokens
        public IEnumerable<Token> GetFirst(IEnumerable<Token> tokens)
        {
            IReadOnlySet<Token> nonTerminals = this.ComputeNonTerminals();
            IReadOnlySet<Token> terminals = this.ComputeTerminals();
            IReadOnlySet<Token> nullable = this.ComputeNullable();
            IDictionary<Token, HashSet<Token>> firstSet = this.ComputeFirstSet();

            foreach(Token s in tokens)
            {
                //First of EOF remains EOF
                if(s.TokenType == TokenType.EOF)
                {
                    yield return s;
                    yield break;
                }
                //First is always the first terminal
                else if(terminals.Contains(s))
                {
                    yield return s;
                    yield break;
                }
                else if(nonTerminals.Contains(s))
                {
                    if(firstSet.ContainsKey(s))
                    {
                        //Return the entire first set for s
                        foreach(Token k in firstSet[s])
                        {
                            yield return k;
                        }
                    }

                    //If the non-terminal s is not nullable, the entire first
                    //set has been found, else continue with any remaining (non)
                    //terminals
                    if(!nullable.Contains(s))
                    {
                        yield break;
                    }
                }
                else
                {
                    throw new InvalidOperationException
                    (
                        $"Unexpected symbol {s.Value}"
                    );
                }
            }
        }

        //Verify if a range of tokens is nullable
        public bool IsNullable(IReadOnlyList<Token> tokens)
        {
            IReadOnlySet<Token> nonTerminals = this.ComputeNonTerminals();
            IReadOnlySet<Token> terminals = this.ComputeTerminals();
            IReadOnlySet<Token> nullable = this.ComputeNullable();

            for(int i = 0; i < tokens.Count; i++)
            {
                //If a non-terminal is found which is not nullable, the range is
                //considered not nullable
                if(nonTerminals.Contains(tokens[i]))
                {
                    if(!nullable.Contains(tokens[i]))
                    {
                        return false;
                    }
                }
                //A terminal is considered a non-nullable value
                else if(terminals.Contains(tokens[i]))
                {
                    return false;
                }
                else
                {
                    throw new InvalidOperationException
                    (
                        $"Unexpected symbol {tokens[i].Value}"
                    );
                }
            }

            return true;
        }
    }
}
