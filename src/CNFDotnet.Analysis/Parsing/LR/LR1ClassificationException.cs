using System;

namespace CNFDotnet.Analysis.Parsing.LR
{
    public class LR1ClassificationException : BaseClassificationException
    {
        public LR1ClassificationException (string message)
            : base(message)
        { }
    }
}

