using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNFDotnet.Analysis.Parsing.LL
{
    public class LL1ClassificationException : BaseClassificationException
    {
        public LL1ClassificationException(string message) : base(message)
        { }
    }
}
