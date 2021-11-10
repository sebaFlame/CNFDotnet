using System;

namespace CNFDotnet.Analysis.Parsing
{
    //The base class representing parser classification errors
    public abstract class BaseClassificationException : Exception
    {
        public BaseClassificationException(string message)
            : base(message)
        { }
    }
}
