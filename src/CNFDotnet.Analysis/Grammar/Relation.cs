using System;
using System.Collections.Generic;

namespace CNFDotnet.Analysis.Grammar
{
    public class Relation
    {
        public Dictionary<Token, HashSet<Token>> Relations => this._relations;

        private Dictionary<Token, HashSet<Token>> _relations;

        public Relation()
        {
            this._relations = new Dictionary<Token, HashSet<Token>>();
        }

        public void AddRelation(Token left, Token right)
        {
            HashSet<Token> relations;
            if(!this._relations.TryGetValue(left, out relations))
            {
                relations = new HashSet<Token>();
                this._relations.Add(left, relations);
            }

            relations.Add(right);
        }

        public static Dictionary<Token, HashSet<Token>> Closure(Relation relation)
        {
            HashSet<Token> keys = new HashSet<Token>();
            Dictionary<Token, HashSet<Token>> result = new Dictionary<Token, HashSet<Token>>();

            foreach(Token i in relation.Relations.Keys)
            {
                keys.Add(i);

                result.Add(i, new HashSet<Token>());

                foreach(Token j in relation.Relations[i])
                {
                    keys.Add(j);
                    if(relation.Relations[i].Contains(j))
                    {
                        result[i].Add(j);
                    }
                }
            }

            foreach(Token i in keys)
            {
                if(!result.ContainsKey(i))
                {
                    result.Add(i, new HashSet<Token>());
                }
            }

            foreach(Token k in keys)
            {
                foreach(Token i in keys)
                {
                    foreach(Token j in keys)
                    {
                        if(result[i].Contains(j) 
                            || (result[i].Contains(k)
                                && result[k].Contains(j)))
                        {
                            result[i].Add(j);
                        }
                    }
                }
            }

            return result;
        }

        public static Dictionary<Token, HashSet<Token>> Propagate(Relation immediate, Relation propagation)
        {
            Dictionary<Token, HashSet<Token>> result = new Dictionary<Token, HashSet<Token>>();
            Dictionary<Token, HashSet<Token>> closed = Closure(propagation);

            foreach(Token k in immediate.Relations.Keys)
            {
                result.Add(k, new HashSet<Token>());

                foreach(Token l in immediate.Relations[k])
                {
                    if(immediate.Relations[k].Contains(l))
                    {
                        result[k].Add(l);
                    }
                }
            }

            foreach(Token s in closed.Keys)
            {
                if(!closed.ContainsKey(s))
                {
                    continue;
                } 
               
                if(!result.ContainsKey(s))
                {
                    result.Add(s, new HashSet<Token>()); 
                }

                foreach(Token t in closed[s])
                {
                    if(!immediate.Relations.ContainsKey(t))
                    {
                        continue;
                    }

                    foreach(Token u in immediate.Relations[t])
                    {
                        if(immediate.Relations[t].Contains(u))
                        {
                            result[s].Add(u);
                        }
                    }
                }
            }

            return result;
        }
        
        public static List<Token> Cycle(Relation relation)
        {
            List<Token> dfs(Token k, List<Token> v)
            {
                if(!relation.Relations.ContainsKey(k))
                {
                    return null;
                }

                foreach(Token l in relation.Relations[k])
                {
                    if(v.Contains(l))
                    {
                        v.Add(k);
                        if(l != k)
                        {
                            v.Add(l);
                        }

                        return v;
                    }

                    v.Add(k);
                    return dfs(l, v);
                }

                return null;
            }

            List<Token> result;

            foreach(Token token in relation.Relations.Keys)
            {
                result = dfs(token, new List<Token>());
                if(!(result is null))
                {
                    return result;
                }
            }

            return null;
        }
    }
}
