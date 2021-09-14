using System;
using System.Collections.Generic;

using CNFDotnet.Analysis.Grammar;

namespace CNFDotnet.Analysis.Parsing.LR
{
    public class State
    {
        public Kernel Kernel { get; private set; }
        public Kernel Items { get; set; }
        public IDictionary<Token, State> Transitions { get; set; }
        public int Index { get; private set; }

        public State (Kernel kernel, int index)
        {
            this.Kernel = kernel;
            this.Index = index;
        }
    }
}
