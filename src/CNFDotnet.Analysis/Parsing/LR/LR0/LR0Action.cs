using System.Collections.Generic;

using CNFDotnet.Analysis.Grammar;

namespace CNFDotnet.Analysis.Parsing.LR.LR0
{
    /* LR(0) can only have a single reduction or multiple shifts (depending on
     * the current token in the input sentence) per action. Provide room for
     * more reducitons so classifications issues can be found */
    public class LR0Action : IAction
    {
        /* During a shift (on a terminal) the input sentence can be shifted 1
         * token to the left and a transition to a new state can happen. If the
         * token is a non-terminal, a goto occurs without shifting the input
         * sentence */
        public Dictionary<Token, State<LR0KernelItem>> Shift
        {
            get;
            private set;
        }

        /* A reductions reduces one ore more tokens (body of a production) to a
         * single non terminal (head of a production) */
        public List<Production> Reduce { get; private set; }

        public LR0Action()
        {
            this.Shift = new Dictionary<Token, State<LR0KernelItem>>();
            this.Reduce = new List<Production>();
        }
    }
}