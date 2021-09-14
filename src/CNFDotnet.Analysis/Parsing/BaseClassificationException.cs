using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNFDotnet.Analysis.Parsing
{
    public abstract class BaseClassificationException : Exception
    {
        public BaseClassificationException(string message)
            : base(message)
        { }
    }
}
