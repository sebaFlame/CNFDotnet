using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNFDotnet.Analysis.Parsing.LR.LR_0
{
    public class LR0ClassificationException : BaseClassificationException
    {
        public LR0ClassificationException(string message) 
            : base(message)
        { }
    }
}
