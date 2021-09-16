using System;
using System.Collections.Generic;

using CNFDotnet.Analysis.Grammar;
using CNFDotnet.Analysis.Parsing.LR;

namespace CNFDotnet.Analysis.Parsing.LR.LR0
{
    public class LR0Action : IAction
    {
        public Dictionary<Token, State> Shift { get; private set; }
        public List<Production> Reduce { get; private set; }

        public LR0Action()
        {
            this.Shift = new Dictionary<Token, State>();
            this.Reduce = new List<Production>();
        }
    }
}