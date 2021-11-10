using System.Collections.Generic;
using System.Linq;

namespace CNFDotnet.Analysis.Grammar
{
    public class Relation
    {
        //A dictionary signifying a relation between a token and 0 or more
        //other tokens
        public Dictionary<Token, HashSet<Token>> Relations => this._relations;

        private readonly Dictionary<Token, HashSet<Token>> _relations;

        public Relation()
        {
            this._relations = new Dictionary<Token, HashSet<Token>>();
        }

        //Add a (right) relation to the left token
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

        /* Compute all possible relations for each token, while keeping order of
         * the relations (transitive closure using Floyd-Warshall) */
        public static Dictionary<Token, HashSet<Token>> Closure
            (Relation relation)
        {
            HashSet<Token> keys = new HashSet<Token>();
            Dictionary<Token, HashSet<Token>> result
                = new Dictionary<Token, HashSet<Token>>();

            //Copy the relation and build the set of unique keys
            foreach(Token i in relation.Relations.Keys)
            {
                keys.Add(i);

                result.Add(i, new HashSet<Token>());

                foreach(Token j in relation.Relations[i])
                {
                    keys.Add(j);

                    //Add the existing relation 
                    if(relation.Relations[i].Contains(j))
                    {
                        result[i].Add(j);
                    }
                }
            }

            //Initialize all relation hashsets
            foreach(Token i in keys)
            {
                if(!result.ContainsKey(i))
                {
                    result.Add(i, new HashSet<Token>());
                }
            }

            //Perform the transitive closure and add new relations
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

        //Propagate the immediate relation using the (closure of the)
        //propagation relation
        public static Dictionary<Token, HashSet<Token>> Propagate
            (Relation immediate, Relation propagation)
        {
            Dictionary<Token, HashSet<Token>> result
                = new Dictionary<Token, HashSet<Token>>();
            //Compute all possible relations between the propagation set
            Dictionary<Token, HashSet<Token>> closed
                = Relation.Closure(propagation);

            //Add the immediate tokens and their relation to the result list
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

            //Merge the immediate set and the propagation set
            foreach(Token s in closed.Keys)
            {
                foreach(Token t in closed[s])
                {
                    //Only use tokens that exist in the immediate set
                    if(!immediate.Relations.ContainsKey(t))
                    {
                        continue;
                    }

                    //Initialize empty hashset for result set
                    if(!result.ContainsKey(s))
                    {
                        result.Add(s, new HashSet<Token>());
                    }

                    //If t has a relation to s and u has a relation to t, then
                    //t has a relation to s 
                    //t -> s
                    //u -> t
                    //u -> s 
                    foreach(Token u in immediate.Relations[t])
                    {
                        result[s].Add(u);
                    }
                }
            }

            return result;
        }

        //If the graph of the relation has a cycle, return the first cycle we
        //find. Otherwise return an empty list. A cyle is a token with a direct
        //relation on itself (through one or more propagated relations).
        public static IEnumerable<Token> Cycle(Relation relation)
        {
            IEnumerable<Token> dfs(Token k, List<Token> v)
            {
                //If no relations are defined for k, return empty set
                if(!relation.Relations.ContainsKey(k))
                {
                    return Enumerable.Empty<Token>();
                }

                //Find the relations of that token k
                foreach(Token l in relation.Relations[k])
                {
                    //If the relation already exists, a cycle has been detected
                    if(v.Contains(l))
                    {
                        if(l == k)
                        {
                            //Add the start token (k)
                            return new List<Token>(v)
                            {
                                k
                            };
                        }
                        else
                        {
                            //Add the start token k and the current token
                            return new List<Token>(v)
                            {
                                k,
                                l
                            };
                        }
                    }

                    //Recurse further into element l
                    IEnumerable<Token> w = dfs
                    (
                        l,
                        new List<Token>(v)
                        {
                            k
                        }
                    );

                    if(w.Any())
                    {
                        return w;
                    }
                }

                return Enumerable.Empty<Token>();
            }

            IEnumerable<Token> result;

            //Foreach token in a relation
            foreach(Token token in relation.Relations.Keys)
            {
                result = dfs(token, new List<Token>());
                if(result.Any())
                {
                    return result;
                }
            }

            return Enumerable.Empty<Token>();
        }
    }
}
