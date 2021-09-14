using System;

using CNFDotnet.Analysis.Grammar;

namespace CNFDotnet.Analysis.Parsing.LR
{
    public class Transition
    {
        public State State { get; set; }
        public Kernel Kernel { get; set; }

        public Transition()
        {
        }

        public Transition(State state)
            : this()
        {
            this.State = state;
        }

        public Transition (Kernel kernel)
            : this()
        {
            this.Kernel = kernel;
        }
    }
}
