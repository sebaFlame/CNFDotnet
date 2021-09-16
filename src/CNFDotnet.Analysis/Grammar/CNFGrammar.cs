using System;
using System.Collections.Generic;

//Disable waring about return types not being assigned
#pragma warning disable IDE0058

namespace CNFDotnet.Analysis.Grammar
{
    public class CNFGrammar
    {
        public HashSet<Token> Terminals => this._terminals;
        public HashSet<Token> NonTerminals => this._nonTerminals;
        public IList<Production> Productions => this._productions;
        public Token? Start { get; private set; }
        public Dictionary<Token, HashSet<Token>> FirstSet => this._firstSet;
        public Dictionary<Token, HashSet<Token>> FollowSet => this._followSet;
        public IReadOnlyCollection<Token> Unreachable => this._unreachable;
        public IReadOnlyCollection<Token> Unrealizable => this._unrealizable;
        public IReadOnlyCollection<Token> Nullable => this._nullable;

        private readonly HashSet<Token> _terminals;
        private readonly HashSet<Token> _nonTerminals;
        private readonly List<Production> _productions;
        private readonly HashSet<Token> _tokens;
        private readonly HashSet<Token> _unreachable;
        private readonly HashSet<Token> _unrealizable;
        private readonly HashSet<Token> _nullable;
        private Dictionary<Token, HashSet<Token>> _firstSet;
        private Dictionary<Token, HashSet<Token>> _followSet;

        private CNFGrammar ()
        {
            this._terminals = new HashSet<Token>();
            this._nonTerminals = new HashSet<Token>();
            this._productions = new List<Production>();
            this._tokens = new HashSet<Token>();
            this._unreachable = new HashSet<Token>();
            this._unrealizable = new HashSet<Token>();
            this._nullable = new HashSet<Token>();
        }

        public CNFGrammar (IEnumerable<Token> tokens)
            : this() => this.AddProductions(Production.CreateProductions(tokens));

        public void AddProduction (Production production)
        {
            production.Grammar = this;
            this._productions.Add(production);

            this._tokens.Add(production.Left);
            foreach (Token token in production.Right)
            {
                this._tokens.Add(token);
            }

            this._nonTerminals.Add(production.Left);
        }

        public void AddProductions (IEnumerable<Production> productions)
        {
            foreach (Production production in productions)
            {
                this.AddProduction(production);
            }
        }

        public void ComputeStartNonTerminal () => this.Start = this._productions[0].Left;

        public void ComputeNonTerminals ()
        {
            this._nonTerminals.Clear();

            foreach (Production production in this._productions)
            {
                if (production.Left.TokenType != TokenType.EMPTY)
                {
                    this._nonTerminals.Add(production.Left);
                }
            }
        }

        public void ComputeTerminals ()
        {
            this._terminals.Clear();

            foreach (Token token in this._tokens)
            {
                if (!this._nonTerminals.Contains(token)
                    && token.TokenType != TokenType.EMPTY)
                {
                    this._terminals.Add(token);
                }
            }
        }

        public void ComputeUnreachable ()
        {
            this._unreachable.Clear();

            Relation relation = new Relation();
            Token start = this.Start.Value;

            foreach (Production production in this._productions)
            {
                foreach (Token token in production.Right)
                {
                    relation.AddRelation(production.Left, token);
                }
            }

            Dictionary<Token, HashSet<Token>> closure = Relation.Closure(relation);
            foreach (Token token in this._nonTerminals)
            {
                if (token != start
                    && !(closure.ContainsKey(start)
                        || closure[start].Contains(token)))
                {
                    this._unreachable.Add(token);
                }
            }
        }

        public void ComputeUnrealizable ()
        {
            this._unrealizable.Clear();

            List<Token> added = new List<Token>();
            HashSet<Token> marked = new HashSet<Token>();
            int currentRightPosition;
            do
            {
                added.Clear();

                foreach (Production production in this._productions)
                {
                    for (currentRightPosition = 0; currentRightPosition < production.Right.Count; currentRightPosition++)
                    {
                        if (!marked.Contains(production.Right[currentRightPosition])
                            && this._nonTerminals.Contains(production.Right[currentRightPosition]))
                        {
                            break;
                        }
                    }

                    if (!marked.Contains(production.Left)
                        && currentRightPosition == production.Right.Count)
                    {
                        marked.Add(production.Left);
                        added.Add(production.Left);
                    }
                }
            } while (added.Count > 0);

            foreach (Token nonTerminal in this._nonTerminals)
            {
                if (!marked.Contains(nonTerminal))
                {
                    this._unrealizable.Add(nonTerminal);
                }
            }
        }

        public void ComputeNullable ()
        {
            this._nullable.Clear();

            List<Token> added = new List<Token>();
            int currentRightPosition;

            do
            {
                added.Clear();

                foreach (Production production in this._productions)
                {
                    for (currentRightPosition = 0; currentRightPosition < production.Right.Count; currentRightPosition++)
                    {
                        if (!this._nullable.Contains(production.Right[currentRightPosition]))
                        {
                            break;
                        }
                    }

                    if (!this._nullable.Contains(production.Left)
                        && currentRightPosition == production.Right.Count)
                    {
                        this._nullable.Add(production.Left);
                        added.Add(production.Left);
                    }
                }
            } while (added.Count > 0);
        }

        public IReadOnlyList<Token> ComputeFirstCycle ()
        {
            Relation relation = new Relation();
            int j, k;

            foreach (Production production in this._productions)
            {
                for (j = 0; j < production.Right.Count; j++)
                {
                    if (!this._nonTerminals.Contains(production.Right[j]))
                    {
                        continue;
                    }

                    for (k = 0; k < production.Right.Count; k++)
                    {
                        if (j == k)
                        {
                            continue;
                        }

                        if (!this._nonTerminals.Contains(production.Right[k]))
                        {
                            break;
                        }

                        if (!this._nullable.Contains(production.Right[k]))
                        {
                            break;
                        }
                    }

                    if (k == production.Right.Count)
                    {
                        relation.AddRelation(production.Left, production.Right[j]);
                    }
                }
            }

            return Relation.Cycle(relation);
        }

        public void ComputeFirstSet ()
        {
            Relation immediate, propagation;
            int j;

            immediate = new Relation();
            propagation = new Relation();

            foreach (Production production in this._productions)
            {
                for (j = 0; j < production.Right.Count; j++)
                {
                    if (!this._nullable.Contains(production.Right[j]))
                    {
                        break;
                    }
                }

                if (j < production.Right.Count
                    && !this._nonTerminals.Contains(production.Right[j]))
                {
                    immediate.AddRelation(production.Left, production.Right[j]);
                }
            }

            foreach (Production production in this._productions)
            {
                foreach (Token token in production.Right)
                {
                    if (this._nonTerminals.Contains(token))
                    {
                        propagation.AddRelation(production.Left, token);
                    }

                    if (!this._nullable.Contains(token))
                    {
                        break;
                    }
                }
            }

            this._firstSet = Relation.Propagate(immediate, propagation);
        }

        public void ComputeFollowSet ()
        {
            Relation immediate, propagation;

            immediate = new Relation();
            propagation = new Relation();

            immediate.AddRelation(this.Start.Value, new Token(TokenType.EOF));

            foreach (Production production in this._productions)
            {
                for (int j = 0; j < production.Right.Count - 1; j++)
                {
                    if (!this._nonTerminals.Contains(production.Right[j]))
                    {
                        continue;
                    }

                    for (int k = j + 1; k < production.Right.Count; k++)
                    {
                        if (!this._nonTerminals.Contains(production.Right[k]))
                        {
                            immediate.AddRelation(production.Right[j], production.Right[k]);
                            break;
                        }

                        foreach (Token s in this._firstSet[production.Right[k]])
                        {
                            immediate.AddRelation(production.Right[j], s);
                        }

                        if (!this._nullable.Contains(production.Right[k]))
                        {
                            break;
                        }
                    }
                }
            }

            foreach (Production production in this._productions)
            {
                for (int j = production.Right.Count - 1; j >= 0; j--)
                {
                    if (this._nonTerminals.Contains(production.Right[j]))
                    {
                        propagation.AddRelation(production.Right[j], production.Left);
                    }

                    if (!this._nullable.Contains(production.Right[j]))
                    {
                        break;
                    }
                }
            }

            this._followSet = Relation.Propagate(immediate, propagation);
        }

        public IEnumerable<Token> GetFirst (IEnumerable<Token> symbols)
        {
            foreach (Token s in symbols)
            {
                if (s.TokenType == TokenType.EOF)
                {
                    yield return s;
                    yield break;
                }
                else if (this._terminals.Contains(s))
                {
                    yield return s;
                    yield break;
                }
                else if (this._nonTerminals.Contains(s))
                {
                    foreach (Token k in this._firstSet[s])
                    {
                        yield return k;
                    }

                    if (!this._nullable.Contains(s))
                    {
                        yield break;
                    }
                }
                else
                {
                    throw new InvalidOperationException($"Unexpected symbol {s.Value}");
                }
            }
        }

        public bool IsNullable (IList<Token> symbols)
        {
            for (int i = 0; i < symbols.Count; i++)
            {
                if (this._nonTerminals.Contains(symbols[i]))
                {
                    if (!this._nullable.Contains(symbols[i]))
                    {
                        return false;
                    }
                }
                else if (this._terminals.Contains(symbols[i]))
                {
                    return false;
                }
                else
                {
                    throw new InvalidOperationException($"Unexpected symbol {symbols[i].Value}");
                }
            }

            return true;
        }
    }
}
